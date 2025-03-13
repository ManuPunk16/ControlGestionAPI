using ControlGestionAPI.Models;
using ControlGestionAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ControlGestionAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IUserService userService, IRoleService roleService, IConfiguration configuration)
        {
            _authService = authService;
            _userService = userService;
            _roleService = roleService;
            _configuration = configuration;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Username) ||
                string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) ||
                string.IsNullOrEmpty(request.Area))
            {
                return BadRequest(new { message = "Please fill in all required fields!" });
            }

            if (await _userService.UsernameExists(request.Username))
            {
                return BadRequest(new { message = "Username is already in use!" });
            }

            var user = new User
            {
                Name = request.Name,
                Username = request.Username,
                Email = request.Email,
                Area = request.Area,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Roles = new List<ObjectId>()
            };

            if (request.Roles != null && request.Roles.Count > 0)
            {
                foreach (var roleId in request.Roles)
                {
                    var role = await _roleService.GetRoleById(roleId);
                    if (role == null)
                    {
                        return BadRequest(new { message = $"Role with ID {roleId} not found." });
                    }
                    user.Roles.Add(ObjectId.Parse(role.Id));
                }
            }
            else
            {
                var defaultRole = await _roleService.GetRoleByName("user");
                if (defaultRole == null)
                {
                    return StatusCode(500, new { message = "Failed to find default role!" });
                }
                user.Roles.Add(ObjectId.Parse(defaultRole.Id));
            }

            var savedUser = await _userService.CreateUser(user);
            if (savedUser == null)
            {
                return StatusCode(500, new { message = "Failed to create user" });
            }

            return Ok(new { message = "User was registered successfully!" });
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Signin([FromBody] SigninRequest request)
        {
            if (string.IsNullOrEmpty(request.Username))
            {
                return BadRequest(new { message = "Username is required!" });
            }

            var user = await _userService.GetUserByUsername(request.Username);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password!" });
            }

            var passwordIsValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!passwordIsValid)
            {
                return Unauthorized(new { message = "Invalid username or password!" });
            }

            var accessToken = _authService.GenerateJwtToken(user);
            var refreshToken = _authService.GenerateRefreshToken(user);
            var refreshTokenExpirationDays = _configuration.GetValue<int>("Jwt:RefreshTokenExpirationDays");

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromDays(refreshTokenExpirationDays)
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

            var roles = user.PopulatedRoles.Select(role => role.Name).ToList();

            return Ok(new SigninResponse
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.Username,
                Email = user.Email,
                Area = user.Area,
                Roles = roles,
                AccessToken = accessToken
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized();
            }

            var principal = _authService.VerifyRefreshToken(refreshToken);
            if (principal == null)
            {
                return Forbid("Invalid refresh token.");
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return BadRequest("Invalid refresh token.");
            }

            string userId = userIdClaim.Value;

            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound();
            }

            var accessToken = _authService.GenerateJwtToken(user);
            var newRefreshToken = _authService.GenerateRefreshToken(user);

            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromDays(_authService.GetRefreshTokenExpirationDays())
            });

            return Ok(new { accessToken });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("refreshToken");
            return NoContent();
        }
    }

    public class SignupRequest
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Area { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; }
    }

    public class SigninRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class SigninResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Area { get; set; }
        public List<string> Roles { get; set; }
        public string AccessToken { get; set; }
    }
}