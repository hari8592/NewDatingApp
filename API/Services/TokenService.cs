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
using Microsoft.AspNetCore.Identity;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;
        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            _userManager = userManager;
        }
        public async Task<string> CreateToken(AppUser user)
        {
            //claims
            var claims = new List<Claim>
           {
               new Claim(JwtRegisteredClaimNames.NameId,user.Id.ToString()),
               new Claim(JwtRegisteredClaimNames.UniqueName,user.UserName),
           };

            //roles
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

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
