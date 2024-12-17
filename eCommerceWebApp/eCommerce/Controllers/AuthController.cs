using System.Threading.Tasks;
using eCommerce.Models;
using eCommerce.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using AspNetCore.Identity.MongoDbCore.Models;

namespace eCommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Update LastLoginAt on first login
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // Generate JWT token for immediate login
                var token = GenerateJwtToken(user);
                
                return Ok(new { 
                    message = "Registration successful",
                    token = token,
                    userId = user.Id,
                    email = user.Email,
                    userName = user.UserName
                });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // First find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid login attempt" });

            // Use the username for sign in
            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);

            if (result.Succeeded)
            {
                // Update LastLoginAt timestamp
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                var token = GenerateJwtToken(user);
                return Ok(new { token });
            }

            return Unauthorized(new { message = "Invalid login attempt" });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully" });
        }

        private string GenerateJwtToken(AppUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(user.Id) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.UserName))
                throw new ArgumentException("User properties cannot be null", nameof(user));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var jwtKey = _configuration["JwtKey"] ?? throw new InvalidOperationException("JWT key is not configured");
            var jwtIssuer = _configuration["JwtIssuer"] ?? throw new InvalidOperationException("JWT issuer is not configured");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(7);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtIssuer,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
