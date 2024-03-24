
namespace Ecommerce.PL.CustomizeResponses
{
    public class Response
    {
        public string? message { get; set; } // we make the meassage nullable 

        public int statusCode { get; set; }

        // Note: that the message and statusCode should be written in the same way as the original response class
        // mean if i write Message or StatusCode it will not work

        public Response(int _statusCode, string? _message = null)
        {
            statusCode = _statusCode ;
            message = _message ?? DefaultMessage(_statusCode);
        }

        private string? DefaultMessage(int statusCode)
        {
            return statusCode switch
            {
                200 => "Success",
                201 => "Created", 
                204 => "No Content",
                400 => "Bad Request",
                401 => "Unauthorized",
                404 => "Data Not Found",
                500 => "Server Error",
                _ => "Error"
            };
        }
    }
}
