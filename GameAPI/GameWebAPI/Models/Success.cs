namespace GameWebAPI.Models.Response
{
    public class Success
    {
        public string type { get; set; }
        public string message { get; set; }

        public Success(string message)
        {
            this.type = "success";
            this.message = message;
        }

        public Success(string type, string message)
        {
            this.type = type;
            this.message = message;
        }
    }
}