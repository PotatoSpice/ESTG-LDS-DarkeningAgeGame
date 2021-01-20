namespace GameWebAPI.Models.Response
{
    public class Error
    {
        public string type { get; set; }
        public string message { get; set; }

        public Error(string message)
        {
            this.type = "error";
            this.message = message;
        }

        public Error(string type, string message)
        {
            this.type = type;
            this.message = message;
        }
    }
}