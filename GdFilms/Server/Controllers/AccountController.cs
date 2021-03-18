using GdFilms.Shared.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GdFilms.Server.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager, 
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("crear")]
        public async Task<ActionResult<UserTokenDTO>> CreateUser([FromBody] UserInfoDTO model)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return BuildToken(model, new List<string>());
            }
            else
            {
                return BadRequest("Usuario o contraseña inválido");
            }
        }

        [HttpPost("ingresar")]
        public async Task<ActionResult<UserTokenDTO>> LoginUser([FromBody] UserInfoDTO model)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);
                return BuildToken(model, roles);
            }
            else
            {
                return BadRequest("Inicio de sesión inválido");
            }
        }

        [HttpGet("renovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserTokenDTO>> Renew()
        {
            var userInfoDTO = new UserInfoDTO()
            {
                Email = HttpContext.User.Identity.Name,
            };
            var user = await _userManager.FindByEmailAsync(userInfoDTO.Email);
            var roles = await _userManager.GetRolesAsync(user);
            return BuildToken(userInfoDTO, roles);
        }

        private UserTokenDTO BuildToken(UserInfoDTO model, IList<string> roles)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, model.Email),
                new Claim(ClaimTypes.Name, model.Email),
                new Claim("myValue", "What I want"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(1);
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds);
            return new UserTokenDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
