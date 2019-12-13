using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using DllService.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DllService.Common
{
    public class CommonExceptionHandlerMiddleware
    {
        public async Task Invoke(HttpContext httpContext)
        {
            var ex = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (ex == null) return;
            httpContext.Response.Clear();
            httpContext.Response.ContentType = @"application/json";
            GlobalException exception;
            if (ex is GlobalException)
            {
                exception = (GlobalException)ex;
            }
            else
            {
                exception = new GlobalException(ex.Message);
            }
            httpContext.Response.StatusCode = (int)exception.Status;
            using (var writer = new StreamWriter(httpContext.Response.Body))
            {
                var resp = new ErrorResp(exception.Message);
                new JsonSerializer().Serialize(writer, resp);
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }
        private class ErrorResp
        {
            public ErrorResp(string message)
            {
                Message = message;
            }

            public string Message { get; set; }
        }
    }
    

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseCommonExceptionHandler(this IApplicationBuilder builder)
        {
            builder.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = new CommonExceptionHandlerMiddleware().Invoke
            });
            return builder;
            //return builder.UseMiddleware<CommonExceptionHandlerMiddleware>();
        }
    }
}
