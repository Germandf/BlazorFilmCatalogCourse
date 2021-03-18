using GdFilms.Client.Shared;
using GdFilms.Server.Helpers;
using GdFilms.Server.Models;
using GdFilms.Shared.DTOs;
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
    [Route("api/usuarios")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class UserController : ControllerBase
    {
        private ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public UserController(
            ApplicationDbContext context, 
            UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = _context.Users.AsQueryable();
            await HttpContext.InsertPaginationParamsOnResponse(queryable, paginationDTO.AmountToShow);
            return await queryable.Paginate(paginationDTO)
                .Select(x => new UserDTO { Email = x.Email, UserId = x.Id }).ToListAsync();
        }

        [HttpGet("roles")]
        public async Task<ActionResult<List<RoleDTO>>> Get()
        {
            return await _context.Roles
                .Select(x => new RoleDTO { Name = x.Name, Id = x.Id }).ToListAsync();
        }

        [HttpPost("agregarRol")]
        public async Task<ActionResult> AddUserRole(EditRoleDTO editRoleDTO)
        {
            var user = await _userManager.FindByIdAsync(editRoleDTO.UserId);
            await _userManager.AddToRoleAsync(user, editRoleDTO.RoleId);
            return NoContent();
        }

        [HttpPost("removerRol")]
        public async Task<ActionResult> RemoveUserRole(EditRoleDTO editRoleDTO)
        {
            var user = await _userManager.FindByIdAsync(editRoleDTO.UserId);
            await _userManager.RemoveFromRoleAsync(user, editRoleDTO.RoleId);
            return NoContent();
        }
    }
}
