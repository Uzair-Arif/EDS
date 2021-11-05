using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EDS.Security
{
    public class JWTAuthenticationManager : IJWTAuthenticationManager
    {

        private readonly SignInManager<IdentityUser> signInManager;
        IDictionary<string, string> users = new Dictionary<string, string>
        {
            { "test1", "password1" },
            { "test2", "password2" }
        };

        private readonly string tokenKey;

        public JWTAuthenticationManager(string tokenKey, SignInManager<IdentityUser> signInManager)
        {
            this.tokenKey = tokenKey;
            this.signInManager = signInManager;
        }

        public async Task<string> Authenticate(string email, string password, bool rememberMe)
        {

            var result = await signInManager.PasswordSignInAsync(
                    email, password, rememberMe, false);

            if (!result.Succeeded)
            {
                return null;
            }

           
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(tokenKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
