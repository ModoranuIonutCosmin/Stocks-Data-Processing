using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace StocksProcessing.API.Attributes;

public class AuthorizeTokenAttribute : AuthorizeAttribute
{
    public AuthorizeTokenAttribute()
    {
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
    }
}