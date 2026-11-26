using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TaskAuthenticationAuthorization.Models
{
    public static class UserPrincipalFactory
    {
        public const string CustomerIdClaim = "customerId";

        public static ClaimsPrincipal Create(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            if (user.Role == Roles.Buyer)
            {
                claims.Add(new Claim(BuyerTypes.ClaimType, user.BuyerType ?? BuyerTypes.None));
            }
            if (user.CustomerId.HasValue)
            {
                claims.Add(new Claim(CustomerIdClaim, user.CustomerId.Value.ToString()));
            }
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }
    }
}
