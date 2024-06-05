using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APIImage.Data;
using APIImage.Models;
using Microsoft.EntityFrameworkCore;

namespace APIImage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ImageContext _context;

        public AuthController(IConfiguration configuration, ImageContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserLogin userLogin)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Username == userLogin.Username);
            if (userExists)
            {
                return BadRequest("User already exists");
            }

            var user = new User
            {
                Username = userLogin.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(userLogin.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User created");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == userLogin.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
            {
                return Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }
    }
}
