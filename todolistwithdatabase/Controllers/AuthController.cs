using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using todolistwithdatabase.Models;
using todolistwithdatabase.Models.Dto;

namespace todolistwithdatabase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public ActionResult<User> Register(UserDTO request)
        {
            if (_context.Users.FirstOrDefault(u => u.Username.ToLower() == request.Username.ToLower()) != null)
            {
                ModelState.AddModelError("ERROR - ", "This username is taken.");
                return BadRequest(ModelState);
            }
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.Role = request.Role;
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(user);
        }

        [HttpPost("login")]
        public ActionResult<User> Login(UserDTO request)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == request.Username);

            if (user == null)
            {
                return BadRequest("User not found");
            }
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Wrong Password");
            }

            string token = CreateToken(user);

            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
