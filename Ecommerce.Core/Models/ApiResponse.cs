using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Models
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public object data { get; set; }
        public string message { get; set; }

        public ApiResponse(HttpStatusCode statusCode, object data, string message)
        {
            this.StatusCode = statusCode;
            this.data = data;
            this.message = message;
        }
    }
}
