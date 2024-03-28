using System.Net;

namespace Ecommerce.PL.CustomizeResponses
{
    public class SuccessResponse : Response
    {
        public object data { get; set; }

        public SuccessResponse(string? _message = null, object data = null, HttpStatusCode _statusCode = HttpStatusCode.OK) : base(_statusCode, _message)
        {
            this.data = data;
        }


    }
}
