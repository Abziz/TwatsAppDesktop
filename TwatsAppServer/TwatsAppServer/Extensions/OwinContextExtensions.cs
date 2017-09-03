using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwatsAppServer.Extensions
{
    public static class OwinContextExtensions
    {
        public static string GetUserId(this IOwinContext ctx)
        {            
            var claim = ctx.Authentication.User.Claims.FirstOrDefault(c => c.Type == "UserId");
            return claim?.Value;
        }
    }
}