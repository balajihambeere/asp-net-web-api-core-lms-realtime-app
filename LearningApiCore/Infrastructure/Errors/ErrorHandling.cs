namespace LearningApiCore.Infrastructure.Errors
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using System;
    using System.Net;
    using System.Threading.Tasks;
    public class ErrorHandling
    {
        private readonly RequestDelegate handle;

        public ErrorHandling(RequestDelegate handle)
        {
            this.handle = handle;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {

                await handle(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (exception is ApiException re)
            {
                context.Response.StatusCode = (int)re.Code;
                if (!string.IsNullOrWhiteSpace(exception.Message))
                {
                    context.Response.ContentType = "application/json";
                    var result = JsonConvert.SerializeObject(new
                    {
                        errors = exception.Message
                    });
                    await context.Response.WriteAsync(result);
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                if (!string.IsNullOrWhiteSpace(exception.Message))
                {
                    context.Response.ContentType = "application/json";
                    var result = JsonConvert.SerializeObject(new
                    {
                        errors = exception.Message
                    });
                    await context.Response.WriteAsync(result);
                }
            }

        }
    }
}
