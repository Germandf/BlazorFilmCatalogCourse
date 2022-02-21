using BlazorFilmCatalogCourse.Server.Models;
using BlazorFilmCatalogCourse.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorFilmCatalogCourse.Server.Controllers
{
    [ApiController]
    [Route("api/votos")]
    public class VoteController : ControllerBase
    {
        private ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;

        public VoteController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Vote(FilmVote filmVote)
        {
            var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
            var currentVote = await _context.FilmVotes
                .FirstOrDefaultAsync(x => x.FilmId == filmVote.FilmId && x.UserId == user.Id);
            if(currentVote == null)
            {
                filmVote.UserId = user.Id;
                filmVote.VoteDate = DateTime.Today;
                _context.Add(filmVote);
                await _context.SaveChangesAsync();
            }
            else
            {
                currentVote.Vote = filmVote.Vote;
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }
    }
}
