using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Library.DataServiceApi
{

    
    public  abstract  class AuthorizedApiController : ControllerBase, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var meta = ((dynamic)context.ActionDescriptor).EndpointMetadata as IList<object>;
            if (meta != null && meta.Any(em => em.GetType() == typeof(AllowAnonymousAttribute)))
            {
                return;
            }

            if (context.HttpContext.Request.Headers["Authorization"].Count == 0)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;;
                context.Result = new ObjectResult(new { error = "Not Authorized" })
                {
                    StatusCode = 403
                };
                return;
            }
            
        }
    }
}
