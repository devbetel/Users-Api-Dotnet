using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using users_api_dotnet.Models;

namespace users_api_dotnet.services
{
    public class TokenService
    {
         private readonly UserManager<IdentityUser<int>> _userManager;
         private readonly IConfiguration _configuration;

        public TokenService(UserManager<IdentityUser<int>> userManager, IConfiguration configuration = null)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<String> CreateToken(string username)
        {
            
            var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            throw new ArgumentException($"User '{username}' not found.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("0asdjas09djsa09djasdjsadajsd09asjd09sajcnzxn");
        var now = DateTime.UtcNow;

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, "regular"),
            new Claim("loginTime", now.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = now.AddHours(8),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    }

}