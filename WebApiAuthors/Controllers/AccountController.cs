using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAuthors.DTOs;
using WebApiAuthors.Services;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly HashService _hashService;
        private readonly IDataProtector _dataProtector;

        public AccountController(UserManager<IdentityUser> userManager, IConfiguration configuration,
                                 SignInManager<IdentityUser> signInManager, IDataProtectionProvider dataProtectionProvider,
                                 HashService hashService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;

            //For Encryption Examples
            _dataProtector = dataProtectionProvider.CreateProtector("unique_value_maybe_secret");
            _hashService = hashService;
        }

        #region
        //Coding example to create a hash
        [HttpGet("hash/{plaintext}")]
        public ActionResult CreateHash(string plaintext)
        {
            var restul1 = _hashService.Hash(plaintext);
            var restul2 = _hashService.Hash(plaintext);

            return Ok(new
            {
                textoPlano = plaintext,
                hash1 = restul1,
                hash2 = restul2
            });
        }

        //Coding examples to Enctyption
        [HttpGet("encrypt")]
        public ActionResult Encrypt()
        {
            var plainText = "Welcome";
            var ciphertext = _dataProtector.Protect(plainText);
            var decryptedText = _dataProtector.Unprotect(ciphertext);

            return Ok(new
            {
                textoPlano = plainText,
                textoCifrado = ciphertext,
                textoDesencriptado = decryptedText
            });
        }

        //Encription with expiration date to payload to decrypt
        [HttpGet("encryptByTime")]
        public ActionResult EncryptByTime()
        {
            var timeLimitedProtector = _dataProtector.ToTimeLimitedDataProtector();

            var plainText = "Test Message";
            var ciphertext = timeLimitedProtector.Protect(plainText, lifetime: TimeSpan.FromSeconds(5));
            Thread.Sleep(6000);
            var decryptedText = timeLimitedProtector.Unprotect(ciphertext);

            return Ok(new
            {
                textoPlano = plainText,
                textoCifrado = ciphertext,
                textoDesencriptado = decryptedText
            });
        }
        #endregion

        /// <summary>
        /// Method to Register a New User
        /// </summary>
        /// <param name="userCredentials">UserCredentials object with data</param>
        /// <returns></returns>
        [HttpPost("register")] // api/account/register
        public async Task<ActionResult<AuthenticationResponse>> Register(UserCredentials userCredentials)
        {
            var user = new IdentityUser { UserName = userCredentials.Email, Email = userCredentials.Email };
            var result = await _userManager.CreateAsync(user, userCredentials.Password);

            if (result.Succeeded)
            {
                return await BuildToken(userCredentials);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        /// <summary>
        /// Method to login an existent User
        /// </summary>
        /// <param name="userCredentials">UserCredentials object with data</param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponse>> Login(UserCredentials userCredentials)
        {
            var result = await _signInManager.PasswordSignInAsync(userCredentials.Email, userCredentials.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await BuildToken(userCredentials);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

        /// <summary>
        /// Method used to renew atoken if expires
        /// </summary>
        /// <returns></returns>
        [HttpGet("RenewToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AuthenticationResponse>> Renew()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;

            var userCredentials = new UserCredentials()
            {
                Email = email,
            };

            return await BuildToken(userCredentials);
        }

        /// <summary>
        /// Method to create a new token
        /// </summary>
        /// <param name="userCredentials">UserCredentials object with data</param>
        /// <returns></returns>
        private async Task<AuthenticationResponse> BuildToken(UserCredentials userCredentials)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentials.Email)
            };

            var user = await _userManager.FindByEmailAsync(userCredentials.Email);
            var claimsDb = await _userManager.GetClaimsAsync(user);

            claims.AddRange(claimsDb);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTkey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims, expires: expiration, signingCredentials: creds);

            return new AuthenticationResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration
            };
        }

        /// <summary>
        /// Method to give admin privileges to an User
        /// </summary>
        /// <param name="adminEditDTO">AdminEditDTO with data</param>
        /// <returns></returns>
        [HttpPost("BecomeAdmin")]
        public async Task<ActionResult> BecomeAdmin(AdminEditDTO adminEditDTO)
        {
            var user = await _userManager.FindByEmailAsync(adminEditDTO.Email);
            await _userManager.AddClaimAsync(user, new Claim("isAdmin", "1"));
            return NoContent();
        }

        /// <summary>
        /// Method to remove admin privileges to an User
        /// </summary>
        /// <param name="adminEditDTO">AdminEditDTO with data</param>
        /// <returns></returns>
        [HttpPost("RemoveAdmin")]
        public async Task<ActionResult> RemoveAdmin(AdminEditDTO adminEditDTO)
        {
            var user = await _userManager.FindByEmailAsync(adminEditDTO.Email);
            await _userManager.RemoveClaimAsync(user, new Claim("isAdmin", "1"));
            return NoContent();
        }
    }
}
