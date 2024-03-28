using System.Net;

namespace Ecommerce.PL.CustomizeResponses
{
    public class ErrorResponse: Response
    {

        public List<string> Errors { get; set; }

        public ErrorResponse(HttpStatusCode httpStatusCode) : base(httpStatusCode)
        {
            Errors = new List<string>();
        }

      
    }
}
