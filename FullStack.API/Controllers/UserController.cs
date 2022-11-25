using FullStack.API.Data;
using FullStack.API.Helpers;
using FullStack.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FullStack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly FullStackDbContext _fullStackDbContext;
        public UserController(FullStackDbContext fullStackDbContext)
        {
            _fullStackDbContext = fullStackDbContext;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userobj)

        {
            if (userobj == null)
                return BadRequest();

            var user = await _fullStackDbContext.Users.FirstOrDefaultAsync(x => x.Email == userobj.Email);
            if(user == null)
                return NotFound(new {Message ="User Not Found"});

            if(!PasswordHasher.VerifyPassword(userobj.Password, user.Password))
            {
                return BadRequest(new {Message = "Password is Incorrect"});
            }

            user.Token = CreateJWT(user);

            return Ok(new
            {
                Token = user.Token,
                Message = "Login Success!"
            });
             
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userobj)
        {
            if (userobj == null)
                return BadRequest();

            // Check Email
            if (await CheckEmailExistAsync(userobj.Email))
                return BadRequest(new { Message = "Email Already Exist" });

            userobj.Password = PasswordHasher.HashPassword(userobj.Password);
            userobj.Role = "User";
            userobj.Token = "";

            await _fullStackDbContext.Users.AddAsync(userobj);
            await _fullStackDbContext.SaveChangesAsync();

            return Ok(new
            {
                Message = "User Registered!"
            });
        }

        private Task<bool> CheckEmailExistAsync(string email)
            => _fullStackDbContext.Users.AnyAsync(x => x.Email == email);


        private string CreateJWT(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysecret...");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);



        }

    }
}
