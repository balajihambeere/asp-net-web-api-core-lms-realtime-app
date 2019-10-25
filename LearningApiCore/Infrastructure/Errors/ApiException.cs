namespace LearningApiCore.Infrastructure.Errors
{
    using System;
    using System.Net;

    public class ApiException : Exception
    {
        public ApiException(HttpStatusCode code, string message = null)
            : base(message)
        {
            Code = code;
        }

        public HttpStatusCode Code { get; }
    }
}
