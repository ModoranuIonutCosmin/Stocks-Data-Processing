using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StocksProccesing.Relational.Model;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StocksProcessing.API.Auth
{
    public static class JwtExtensionMethods
    {
        public static string GenerateJwtToken(this ApplicationUser user,
            string secret, string issuer, string audience)
        {
            var claims = new[]
            {
                // Unique ID for this token
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),

                // The username using the Identity name so it fills out the HttpContext.User.Identity.Name value
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),

                // Add user Id so that UserManager.GetUserAsync can find the user based on Id
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                signingCredentials: credentials,
                expires: DateTime.Now.AddMonths(3)
                );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
