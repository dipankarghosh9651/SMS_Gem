using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using SMS_Gem.Security;

namespace SMS.Security
{
    public class JwtAuthAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader == null || !authHeader.Scheme.Equals("Bearer", System.StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(authHeader.Parameter))
            {
                Reject(actionContext, "Missing or invalid Bearer token.");
                return;
            }

            var principal = JwtTokenHelper.ValidateToken(authHeader.Parameter);
            if (principal == null)
            {
                Reject(actionContext, "Token has expired or signature is invalid.");
                return;
            }

            // Set current user context for the request thread and HttpContext
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;

                // Extract and store claims in HttpContext for downstream repository access
                var branchClaim = principal.FindFirst("Branch");
                if (branchClaim != null)
                {
                    HttpContext.Current.Items["BranchCode"] = branchClaim.Value;
                }
            }
        }

        private void Reject(HttpActionContext actionContext, string message)
        {
            actionContext.Response = actionContext.Request.CreateResponse(
                HttpStatusCode.Unauthorized,
                new { Success = false, Message = message }
            );
        }
    }
}



//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;

//namespace SMS_Gem.Controllers
//{
//    public class JwtAuthAttribute : ApiController
//    {
//        // GET api/<controller>
//        public IEnumerable<string> Get()
//        {
//            return new string[] { "value1", "value2" };
//        }

//        // GET api/<controller>/5
//        public string Get(int id)
//        {
//            return "value";
//        }

//        // POST api/<controller>
//        public void Post([FromBody] string value)
//        {
//        }

//        // PUT api/<controller>/5
//        public void Put(int id, [FromBody] string value)
//        {
//        }

//        // DELETE api/<controller>/5
//        public void Delete(int id)
//        {
//        }
//    }
//}