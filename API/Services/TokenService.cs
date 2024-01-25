using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            // Initialize the SymmetricSecurityKey with the secret key from configuration
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            _userManager = userManager;
        }

        // Method to create a JWT for the provided AppUser
        public async Task<string> CreateToken(AppUser user)
        {
            // Creating a list of claims for a user, including UserId and UserName.
            var claims = new List<Claim>
            {
                // Adding a claim with the user's unique identifier (UserId).
                new Claim(JwtRegisteredClaimNames.NameId,user.Id.ToString()),
                // Adding a claim with the user's unique name (UserName).
                new Claim(JwtRegisteredClaimNames.UniqueName,user.UserName)
            };
            //Retrieve the roles associated with the user using UserManager.
            var roles = await _userManager.GetRolesAsync(user);
            // Adding roles to the user's claims.
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            // Creating SigningCredentials using a specified key (_key) and signing algorithm (HmacSha512Signature).
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // Create a SecurityTokenDescriptor with token details
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            /// return the token as a string.
            return tokenHandler.WriteToken(token);
        }
    }
}