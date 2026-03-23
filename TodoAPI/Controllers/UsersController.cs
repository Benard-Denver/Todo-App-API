using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using TodoAPI.DbModels;
using TodoAPI.Models;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;
using BCrypt.Net;
using TodoAPI.Services;

namespace TodoAPI.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private TodoContext dB = new TodoContext();
        private readonly JwtService _jwtService;
        public UsersController(JwtService jwtService) 
        {
            _jwtService = jwtService;
        }

        [HttpPost("signup")]
        public ActionResult CreateUser([FromBody] UserModel newUser)
        {
            if (string.IsNullOrWhiteSpace(newUser.Username) || string.IsNullOrWhiteSpace(newUser.Password))
            {
                return BadRequest("Username and Password required");
            }

            var existingUser = dB.Users.FirstOrDefault(u => u.Username == newUser.Username);

            if (existingUser != null)
            {
                return BadRequest("User already exists");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUser.Password);

            User user = new User
            {
                Username = newUser.Username,
                Password = hashedPassword,
            };

            var token = _jwtService.GenerateToken(user.Username);
            dB.Users.Add(user);
            dB.SaveChanges();

            return Ok(new { username = user.Username, token});
        }

        [HttpPost("login")]
        public ActionResult LoginUser([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return Ok(new { success = false, message = "Username and Password required" });

            var user = dB.Users.FirstOrDefault(u => u.Username == request.Username);

            if (user == null)
            {
                return Ok(new { success = false, message = "Invalid User or Password" });
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

            if (!isPasswordValid)
            {
                return Ok(new { success = false, message = "Invalid Username or Password" });
            }


            var token = _jwtService.GenerateToken(user.Username);

            return Ok(new { success = true, token = token, username = user.Username, message = "Login Successful" });
        }
    }
}
