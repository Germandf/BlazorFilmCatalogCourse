using AutoMapper;
using GdFilms.Server.Helpers;
using GdFilms.Server.Models;
using GdFilms.Shared.DTOs;
using GdFilms.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GdFilms.Server.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class FilmController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILocalFileService _localFileService;
        private readonly IMapper _mapper;
        private readonly string _containerName = "peliculas";

        public FilmController(
            ApplicationDbContext context, 
            UserManager<IdentityUser> userManager,
            ILocalFileService localFileService, 
            IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _localFileService = localFileService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<HomePageDTO>> Get()
        {
            var limit = 6;
            var today = DateTime.Today;
            var filmsOnBillboard = await _context.Films
                .Where(x => x.OnBillboard)
                .Take(limit)
                .OrderByDescending(x => x.ReleaseDate)
                .ToListAsync();
            var upcomingReleases = await _context.Films
                .Where(x => x.ReleaseDate > today)
                .OrderBy(x => x.ReleaseDate)
                .Take(limit)
                .ToListAsync();
            var response = new HomePageDTO()
            {
                FilmsOnBillboard = filmsOnBillboard,
                UpcomingReleases = upcomingReleases
            };
            return response;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<FilmDetailsDTO>> Get(int id)
        {
            var film = await _context.Films
                .Where(x => x.Id == id)
                .Include(x => x.FilmGenres).ThenInclude(x => x.Genre)
                .Include(x => x.FilmActors).ThenInclude(x => x.Person)
                .FirstOrDefaultAsync();
            if (film == null) return NotFound();
            var averageVote = 0.0;
            var userVote = 0;
            if(await _context.FilmVotes.AnyAsync(x => x.FilmId == id))
            {
                averageVote = await _context.FilmVotes.Where(x => x.FilmId == id).AverageAsync(x => x.Vote);
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                    var userVoteDB = await _context.FilmVotes
                        .FirstOrDefaultAsync(x => x.FilmId == id && x.UserId == user.Id);
                    if (userVoteDB != null)
                    {
                        userVote = userVoteDB.Vote;
                    }
                }
            }
            film.FilmActors = film.FilmActors.OrderBy(x => x.Order).ToList();
            var model = new FilmDetailsDTO();
            model.Film = film;
            model.Genres = film.FilmGenres.Select(x => x.Genre).ToList();
            model.People = film.FilmActors.Select(x =>
                new Person
                {
                    Id = x.Person.Id,
                    Name = x.Person.Name,
                    Photo = x.Person.Photo,
                    Character = x.Character
                }).ToList();
            model.AverageVote = averageVote;
            model.UserVote = userVote;
            return model;
        }

        [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Film>>> Get([FromQuery] FilterFilmsParams filterFilmsParams)
        {
            var filmsQueryable = _context.Films.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filterFilmsParams.Title))
            {
                filmsQueryable = filmsQueryable
                    .Where(x => x.Title.ToLower().Contains(filterFilmsParams.Title.ToLower()));
            }
            if (filterFilmsParams.OnBillboard)
            {
                filmsQueryable = filmsQueryable.Where(x => x.OnBillboard);
            }
            if (filterFilmsParams.UpcomingReleases)
            {
                filmsQueryable = filmsQueryable.Where(x => x.ReleaseDate >= DateTime.Today);
            }
            if (filterFilmsParams.GenreId != 0)
            {
                filmsQueryable = filmsQueryable
                    .Where(x => x.FilmGenres.Select(y => y.GenreId).Contains(filterFilmsParams.GenreId));
            }
            // TODO: Feature voting
            await HttpContext.InsertPaginationParamsOnResponse(filmsQueryable, filterFilmsParams.AmountToShow);
            var films = await filmsQueryable.Paginate(filterFilmsParams.PaginationDTO).ToListAsync();
            return films;
        }

        [HttpGet("editar/{id}")]
        public async Task<ActionResult<EditFilmDTO>> GetBeforePut(int id)
        {
            var filmActionResult = await Get(id);
            if (filmActionResult.Result is NotFoundResult) return NotFound();
            var editFilmDTO = filmActionResult.Value;
            var selectedGenresIds = editFilmDTO.Genres
                .Select(x => x.Id).ToList();
            var notSelectedGenres = await _context.Genres
                .Where(x => !selectedGenresIds.Contains(x.Id)).ToListAsync();
            return new EditFilmDTO()
            {
                Film = editFilmDTO.Film,
                People = editFilmDTO.People,
                SelectedGenres = editFilmDTO.Genres,
                NotSelectedGenres = notSelectedGenres
            };
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Film film)
        {
            if (!string.IsNullOrEmpty(film.Image))
            {
                var content = Convert.FromBase64String(film.Image);
                film.Image = await _localFileService.Save(content, ".jpg", _containerName);
            }
            if (film.FilmActors != null)
            {
                for (int i = 0; i < film.FilmActors.Count; i++)
                {
                    film.FilmActors[i].Order = i + 1;
                }
            }
            _context.Add(film);
            await _context.SaveChangesAsync();
            return film.Id;
        }

        [HttpPut]
        public async Task<ActionResult> Put(Film film)
        {
            var filmDB = await _context.Films.FirstOrDefaultAsync(x => x.Id == film.Id);
            if (filmDB == null) return NotFound();
            filmDB = _mapper.Map(film, filmDB);
            if (!string.IsNullOrWhiteSpace(film.Image))
            {
                var filmImage = Convert.FromBase64String(film.Image);
                filmDB.Image = await _localFileService.Edit(filmImage, "jpg", "peliculas", filmDB.Image);
            }
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"delete from FilmGenres WHERE FilmId = {film.Id}; delete from FilmActors WHERE FilmId = {film.Id}");
            if (film.FilmActors != null)
            {
                for (int i = 0; i < film.FilmActors.Count; i++)
                {
                    film.FilmActors[i].Order = i + 1;
                }
            }
            filmDB.FilmActors = film.FilmActors;
            filmDB.FilmGenres = film.FilmGenres;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _context.Films.AnyAsync(x => x.Id == id);
            if (!exists) return NotFound();
            _context.Films.Remove(new Film { Id = id });
            await _context.SaveChangesAsync();
            return NoContent();
        }

        public class FilterFilmsParams
        {
            public int Page { get; set; } = 1;
            public int AmountToShow { get; set; } = 10;
            public PaginationDTO PaginationDTO
            {
                get { return new PaginationDTO() { Page = Page, AmountToShow = AmountToShow }; }
            }
            public string Title { get; set; }
            public int GenreId { get; set; }
            public bool OnBillboard { get; set; }
            public bool UpcomingReleases { get; set; }
            public bool MostVoted { get; set; }
        }
    }
}
