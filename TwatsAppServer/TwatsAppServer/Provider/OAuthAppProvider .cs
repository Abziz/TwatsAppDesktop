using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TwatsAppCore.Services;

namespace TwatsAppServer.Provider
{
    public class OAuthAppProvider : OAuthAuthorizationServerProvider
    {
        

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            return Task.Factory.StartNew(() =>
            {
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                var userService = new UserService();
                var user = userService.FindByCredentials(context.UserName, context.Password).Result;
                if (user != null)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim("UserId", user.Id.ToString())
                    };
                    ClaimsIdentity oAutIdentity = new ClaimsIdentity(claims, Startup.OAuthOptions.AuthenticationType);
                    context.Validated(new AuthenticationTicket(oAutIdentity, new AuthenticationProperties() { }));
                }
                else
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                }
            });
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
            {
                context.Validated();
            }
            return Task.FromResult<object>(null);
        }
    }
}