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
                        new Claim("userName", user.UserName),
                        new Claim("userId", user.Id.ToString()),
                        new Claim("firstName",user.FirstName),
                        new Claim("lastName",user.LastName)
                    };
                    ClaimsIdentity oAutIdentity = new ClaimsIdentity(claims, Startup.OAuthOptions.AuthenticationType);
                    context.Validated(new AuthenticationTicket(oAutIdentity, new AuthenticationProperties(){ }));
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

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (var claim in context.Identity.Claims)
            {
                context.AdditionalResponseParameters.Add(claim.Type, claim.Value);
            }
            return Task.FromResult<object>(null);
        }
    }
}