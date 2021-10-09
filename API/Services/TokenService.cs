using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public string CreateToken(AppUser user)
        {
            //claims
            var claims = new List<Claim>
           {
               new Claim(JwtRegisteredClaimNames.NameId,user.UserName)
           };

            //credentials
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            
            //tokendescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            //token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            //token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // WriteToken
            return tokenHandler.WriteToken(token);
        }
    }
}
