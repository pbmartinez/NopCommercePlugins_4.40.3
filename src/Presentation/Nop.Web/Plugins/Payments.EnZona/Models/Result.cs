using System.Net;

namespace Nop.Plugin.Payments.EnZona.Models
{
    public class Result<T>
    {
        public T Value { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public Result(T value, HttpStatusCode statusCode, bool success = true, string errorMessage = "")
        {
            Value = value;
            StatusCode = statusCode;
            Success = success;
            ErrorMessage = errorMessage;
        }
    }
}
