using Microsoft.AspNetCore.Http;
using Neptuo;
using Neptuo.Exceptions.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Common.Diagnostics
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly IExceptionHandler exceptionHandler;

        public ExceptionMiddleware(IExceptionHandler exceptionHandler)
        {
            Ensure.NotNull(exceptionHandler, "exceptionHandler");
            this.exceptionHandler = exceptionHandler;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                exceptionHandler.Handle(e);
                throw;
            }
        }
    }
}
