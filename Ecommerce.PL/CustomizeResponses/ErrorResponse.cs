namespace Ecommerce.PL.CustomizeResponses
{
    public class ErrorResponse: Response
    {
        public IEnumerable<string> Errors { get; set; }

        public ErrorResponse() : base(400)
        {
            Errors = new List<string>();
        }

    }
}
