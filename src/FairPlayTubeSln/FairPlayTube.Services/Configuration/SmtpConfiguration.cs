namespace FairPlayTube.Services.Configuration
{
    public class SmtpConfiguration
    {
        public string SenderDisplayName { get; set; }
        public string SenderEmail { get; set; }
        public string SenderUsername { get; set; }
        public string SenderPassword { get; set; }
        public int Port { get; set; }
        public string Server { get; set; }
        public bool UseSSL { get; set; }
    }

}
