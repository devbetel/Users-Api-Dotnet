using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MinhaApiRest.Models;
using Org.BouncyCastle.Ocsp;
using users_api_dotnet.Models;
using users_api_dotnet.services;

namespace users_api.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class userController : ControllerBase
    {
        private TokenService _tokenService;
        private readonly UserManager<IdentityUser<int>> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;


        private SignInManager<IdentityUser<int>> _signInManager;
        public userController(TokenService tokenService, SignInManager<IdentityUser<int>> signInManager, UserManager<IdentityUser<int>> userManager = null, IHttpContextAccessor httpContextAccessor = null)
        {
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            var userIdentity = new IdentityUser<int> { UserName = user.Username, Email = user.Email };
            if (IsValidEmail(userIdentity.Email))
            {
                var result = await _userManager.CreateAsync(userIdentity, user.Password);


                if (result.Succeeded)
                {
                    var addToRoleResult = await _userManager.AddToRoleAsync(userIdentity, "regular");
                    await _signInManager.SignInAsync(userIdentity, isPersistent: false);

                    return Ok("User registered and authenticated successfully.");
                }
            }
            return BadRequest();


        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var userSystem = await _userManager.FindByEmailAsync(model.EmailOrUserName) ??
                       await _userManager.FindByNameAsync(model.EmailOrUserName);

            if (userSystem != null)
            {
                var result = await _signInManager.PasswordSignInAsync(userSystem, model.Password, false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var identityUser = _signInManager
                        .UserManager
                        .Users
                        .FirstOrDefault(user =>
                        user.NormalizedUserName == userSystem.UserName.ToUpper());


                    var token = await _tokenService.CreateToken(identityUser.UserName);

                    return Ok(token);

                }
            }

            return BadRequest("Authentication failed.");
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok("User logged out successfully.");
        }

        [HttpGet]
        [Authorize]
        [Route("generate-password-reset")]

        public async Task<IActionResult> GeneratePasswordResetTokenAsync()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            var userId = userIdClaim.Value;



            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var tokenPassword = await _userManager.GeneratePasswordResetTokenAsync(user);
            return Ok(tokenPassword);

        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ChangePasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.OldPassword);
            if (!passwordValid)
            {
                return BadRequest("Old password invalid.");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password changed with sucess.");
            }
            else
            {
                return BadRequest("Error when resetting password ");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                MailAddress mailAddress = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }



    }
}