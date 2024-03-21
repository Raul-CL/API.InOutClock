using API.InOutClock.API.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.InOutClock.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AuthenticationController(UserManager<IdentityUser> userManager, IOptions<JwtConfig> jwtConfig)
        {
            _userManager = userManager;
            _jwtConfig = jwtConfig.Value;
        }
    }
}
