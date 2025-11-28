//NetCoreAPI-main/Web/Auxiliar/AuthorizeAttribute.cs

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Web.Auxiliar
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.Items["User"];
            if (user == null)
            {
                context.Result = new JsonResult(new { message = "Token inválido o no proporcionado." })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
        }
    }
}
