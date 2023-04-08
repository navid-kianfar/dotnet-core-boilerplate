using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace MyApplication.Endpoint.Controllers;

[Authorize]
public abstract class BaseController : Controller
{
    public UserAuthDto? UserAuth
    {
        get
        {
            if (!(User.Identity?.IsAuthenticated ?? false)) return null;
            return new UserAuthDto
            {
                TokenId = Guid.Parse(User.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value),
                UserId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),
                Username = User.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value
            };
        }
    }

    public class UserAuthDto
    {
        public Guid TokenId { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}