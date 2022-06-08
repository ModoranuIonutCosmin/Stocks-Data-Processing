using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Stocks.General.Entities;
using StocksProccesing.Relational.Model;

namespace StocksFinalSolution.BusinessLogic.Features.Authentication.ExtensionMethods;

public static class JwtExtensionMethods
{
    public static string GenerateJwtToken(this ApplicationUser user,
        string secret, string issuer, string audience, Subscription subscription)
    {
        var claims = new[]
        {
            // Unique ID for this token
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),

            // The username using the Identity name so it fills out the HttpContext.User.Identity.Name value
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),

            // Add user Id so that UserManager.GetUserAsync can find the user based on Id
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim("subscriptionLevel", subscription?.Type.ToString() ?? "none"),
            new Claim("subscriptionStatus", subscription?.Status ?? "none"),
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            signingCredentials: credentials,
            expires: DateTime.Now.AddMonths(3)
        );


        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}