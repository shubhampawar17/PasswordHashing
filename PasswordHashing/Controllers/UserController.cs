using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PasswordHashing.Data;
using PasswordHashing.DTOs;
using PasswordHashing.Models;

namespace PasswordHashing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly PasswordContext _passwordContext;
        public UserController(PasswordContext passwordContext)
        {
            _passwordContext = passwordContext;
        }
        [HttpPost("create_user")]
        public IActionResult CreateUser(UserDto userDto)
        {
            if (string.IsNullOrEmpty(userDto.UserName) || string.IsNullOrEmpty(userDto.Password))
            {
                return BadRequest("User and Password are required");
            }
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            var user = new User
            {
                UserName = userDto.UserName,
                PasswordHash = passwordHash,
                Email = userDto.Email
            };

            _passwordContext.Add(user);
            _passwordContext.SaveChanges();

            return Ok("user created");
        }
        [HttpPost("Login")]
        public IActionResult Login(LoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.UserName) && string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Username and password are required");
            }
            var user = _passwordContext.Users.FirstOrDefault(u => u.UserName == loginDto.UserName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return BadRequest("Invalid Credentials");
            }
            return Ok("Login Successfully");
        }
    }
}
