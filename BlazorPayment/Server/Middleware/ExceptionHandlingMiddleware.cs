using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPayment.Server.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
               await next(context);
            }
            catch(NotFoundException exc)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            catch (Exception exc)
            {
                throw;
            }
        }
    }
}
