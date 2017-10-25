namespace Diploma.EmailSender.Models
{
    public class EmailSenderOptions
    {
        public string From { get; set; }

        public string Password { get; set; }

        public string To { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }
    }
}
