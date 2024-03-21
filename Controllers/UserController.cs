using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MinhaApiRest.Models;
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

        private SignInManager<IdentityUser<int>> _signInManager;
        public userController(TokenService tokenService, SignInManager<IdentityUser<int>> signInManager, UserManager<IdentityUser<int>> userManager = null)
        {
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userManager = userManager;
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


                    Token token = _tokenService.CreateToken(identityUser, _signInManager
                    .UserManager.GetRolesAsync(identityUser).Result.FirstOrDefault());

                    return Ok(token.Value);

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