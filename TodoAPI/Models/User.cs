using Microsoft.AspNetCore.Identity;

namespace TodoAPI.Models
{
    public class UserModel
    {
       public int UserId { get; set; }
       public required string Username { get; set; }
       public required string Password { get; set; }
    }

    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
