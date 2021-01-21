namespace GameWebServer.Models.Responses
{
    public class ErrorResponse
    {
        public string errorType { get; set; }
        public string message { get; set; }  

        public ErrorResponse(string p_errorType, string p_message)
        {
            this.errorType = p_errorType;
            this.message = p_message;
        }

    }
}