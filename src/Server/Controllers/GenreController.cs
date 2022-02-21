using BlazorFilmCatalogCourse.Server.Models;
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
    [Route("api/generos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class GenreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenreController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Genre>>> Get()
        {
            return await _context.Genres.ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Genre>> Get(int id)
        {
            return await _context.Genres.FirstOrDefaultAsync(x => x.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Genre genre)
        {
            _context.Add(genre);
            await _context.SaveChangesAsync();
            return genre.Id;
        }

        [HttpPut]
        public async Task<ActionResult> Put(Genre genre)
        {
            _context.Attach(genre).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _context.Genres.AnyAsync(x => x.Id == id);
            if (!exists) return NotFound();
            _context.Genres.Remove(new Genre { Id = id });
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
