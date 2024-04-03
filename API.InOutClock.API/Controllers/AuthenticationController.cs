using API.InOutClock.API.Configurations;
using API.InOutClock.Data;
using API.InOutClock.Shared.Auth;
using API.InOutClock.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.InOutClock.API.Controllers
{    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtConfig _jwtConfig;

        public AuthenticationController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JwtConfig> jwtConfig)
        {
            _userManager = userManager;
            _jwtConfig = jwtConfig.Value;
            
            _roleManager = roleManager;
        }
        
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(errors);
            }
            
            //Validamos si el usuario ya existe
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                return BadRequest(new AuthResult()
                {
                    Success = false,
                    Errors = new List<string>() { "El usuario ya existe" }
                });
            }

            //Crear usuario
            var user = new IdentityUser()
            {
                Email = request.Email,
                UserName = request.Email                
            };            
            
            var isCreated = await _userManager.CreateAsync(user, request.Password);

            //Si el usuario fue creado exitosamente, generamos el token
            if (isCreated.Succeeded) 
            {
                //Asignar rol al usuario
                if(request.IsAdmin)
                {
                    //await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _userManager.AddToRoleAsync(user, "Admin");                    
                }
                else
                {   await _roleManager.CreateAsync(new IdentityRole("User"));
                    await _userManager.AddToRoleAsync(user, "User");
                }

                var token = GenerateJwtToken(user, request.IsAdmin);
                return Ok(new AuthResult()
                {
                    Success = true,
                    Token = token
                });
            }//Si no, devolvemos los errores
            else
            {
                var errors = new List<string>();
                foreach (var item in isCreated.Errors) 
                    errors.Add(item.Description);

                return BadRequest(new AuthResult()
                {
                    Success = false,
                    Errors = errors
                });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(errors);
            }

            //Validamos si el usuario existe
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            var isAdmin = await _userManager.IsInRoleAsync(existingUser, "Admin");

            if (existingUser == null)
            {
                return BadRequest(new AuthResult()
                {
                    Errors = new List<string>() { "Usuario y/o contraseña incorrectos" },
                    Success = false
                });
            }

            //Validamos si la contraseña es correcta, CheckPasswordAsync devuelve un booleano comparando contraseñas
            var isCorrect = await _userManager.CheckPasswordAsync(existingUser, request.Password);

            if (!isCorrect)
            {
                return BadRequest(new AuthResult()
                {
                    Success = false,
                    Errors = new List<string>() { "Usuario y/o contraseña incorrectos" }
                });
            }

            //Si el usuario existe y la contraseña es correcta, generamos el token
            var token = GenerateJwtToken(existingUser, isAdmin);

            return Ok(new AuthResult()
            {
                Success = true,
                Token = token
            });
        }

        private string GenerateJwtToken(IdentityUser user, bool isAdmin)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
          
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new ClaimsIdentity(new []
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //Identificador unico del token
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()) //Fecha de creacion del token                    
                })),
                Expires = DateTime.UtcNow.AddYears(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            if (isAdmin)
            {
                tokenDescriptor.Subject.AddClaim(new Claim("Role", "Admin"));
            }
            else
            {
                tokenDescriptor.Subject.AddClaim(new Claim("Role", "User"));
            }
                

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
        }   
    }
}
