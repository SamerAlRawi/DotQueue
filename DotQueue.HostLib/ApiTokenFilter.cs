using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace DotQueue.HostLib
{
    internal class ApiTokenFilter : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (TokenValidationProvider.CheckAuthorization)
            {
                return IsValidToken(actionContext);
            }
            return true;
        }

        private static bool IsValidToken(HttpActionContext actionContext)
        {
            try
            {
                var tokenHeaderValue = actionContext.Request
                    .Headers
                    .First(h => h.Key == "Api-Token")
                    .Value
                    .First();
                return TokenValidationProvider.Validator.IsValidToken(tokenHeaderValue);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
