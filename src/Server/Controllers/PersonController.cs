using AutoMapper;
using BlazorFilmCatalogCourse.Server.Helpers;
using BlazorFilmCatalogCourse.Server.Models;
using BlazorFilmCatalogCourse.Shared.DTOs;
using BlazorFilmCatalogCourse.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorFilmCatalogCourse.Server.Controllers
{
    [ApiController]
    [Route("api/personas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class PersonController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILocalFileService _localFileService;
        private readonly IMapper _mapper;
        private readonly string _containerName = "personas";

        public PersonController(ApplicationDbContext context, ILocalFileService localFileService, IMapper mapper)
        {
            _context = context;
            _localFileService = localFileService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Person>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = _context.People.AsQueryable();
            await HttpContext.InsertPaginationParamsOnResponse(queryable, paginationDTO.AmountToShow);
            return await queryable.Paginate(paginationDTO).ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Person>> Get(int id)
        {
            var person = await _context.People.FirstOrDefaultAsync(x => x.Id == id);
            if (person == null) return NotFound();
            return person;
        }

        [HttpGet("buscar/{name}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Person>>> Get(string name)
        {
            if(string.IsNullOrWhiteSpace(name)) return new List<Person>();
            name = name.ToLower();
            return await _context.People
                .Where(x => x.Name.ToLower().Contains(name)).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Person person)
        {
            if (!string.IsNullOrEmpty(person.Photo))
            {
                var content = Convert.FromBase64String(person.Photo);
                person.Photo = await _localFileService.Save(content, ".jpg", _containerName);
            }
            _context.Add(person);
            await _context.SaveChangesAsync();
            return person.Id;
        }

        [HttpPut]
        public async Task<ActionResult> Put(Person person)
        {
            var personDB = await _context.People.FirstOrDefaultAsync(x => x.Id == person.Id);
            if (personDB == null) return NotFound();
            personDB = _mapper.Map(person, personDB);
            if (!string.IsNullOrWhiteSpace(person.Photo))
            {
                var photo = Convert.FromBase64String(person.Photo);
                personDB.Photo = await _localFileService.Edit(photo, "jpg", "personas", personDB.Photo);
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _context.People.AnyAsync(x => x.Id == id);
            if (!exists) return NotFound();
            _context.People.Remove(new Person { Id = id });
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
