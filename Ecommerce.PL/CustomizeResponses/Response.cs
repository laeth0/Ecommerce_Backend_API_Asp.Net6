
using System.Net;

namespace Ecommerce.PL.CustomizeResponses
{
    public class Response
    {
        public string? message { get; set; }

        public HttpStatusCode statusCode { get; set; }

        // Note: that the message and statusCode should be written in the same way as the original response class
        // mean if i write Message or StatusCode it will not work

        public Response(HttpStatusCode _statusCode, string? _message = null)
        {
            statusCode = _statusCode ;
            message = _message ?? DefaultMessage(_statusCode);
        }

        private string DefaultMessage(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.OK => "Success",
                HttpStatusCode.Created => "Created",
                HttpStatusCode.NoContent => "No Content",
                HttpStatusCode.BadRequest => "Bad Request",
                HttpStatusCode.Unauthorized => "Unauthorized",
                HttpStatusCode.NotFound => "Data Not Found",
                HttpStatusCode.InternalServerError => "Server Error",
                _ => "Error"
            };
        }
    }
}
