using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DemoAPI2.Models;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace DemoAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        public UserController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User[]>> GetAll()
        {
            return Ok(await _context.Users.ToListAsync());
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id){
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Not found");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDTO request)
        {
            var checkUser = await _context.Users.FirstOrDefaultAsync(x => request.Username == x.Username);
            if (checkUser == null)
            {
                return NotFound("User does not exist");
            }
            if (!VerifyPassword(request.Password, checkUser.PasswordHash, checkUser.PasswordSalt))
            {
                return BadRequest("Wrong password");
            }
            string token = CreateToken(checkUser);
            return Ok(new {username = checkUser.Username, token = token});
        }
        [HttpPost("register")]
        public async Task<ActionResult> Register(UserDTO request)
        {
            var checkUser = await _context.Users.FirstOrDefaultAsync(x => request.Username == x.Username);
            if (checkUser != null)
            {
                return BadRequest("User already exists");
            }
            CreateHashPassword(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = new User();
            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok("Register successful");
        }
        private void CreateHashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Secret").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
