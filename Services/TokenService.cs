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
        
        public Token CreateToken(IdentityUser<int> user, string role)
        {
            
            Claim[] userRights = new Claim[]
            {
                new Claim("username", user.UserName),
                new Claim("id", user.Id.ToString()),
                new Claim("role", role),
                new Claim("LoginTime", DateTime.UtcNow.ToString()),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("0asdjas09djsa09djasdjsadajsd09asjd09sajcnzxn")
                );

            var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: userRights,
                signingCredentials: credenciais,
                expires: DateTime.UtcNow.AddHours(8)
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return new Token(tokenString);
        }
    }
}