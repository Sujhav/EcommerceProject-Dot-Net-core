namespace EcommerceProject.Models
{
    public class SMTPConfigModel
    {
        public string SenderAddress { get; set; }
        public string SenderDisplayName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string host { get; set; }
        public int port { get; set; }
        public bool EnableSSl { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public bool IsBodyHtml { get; set; }
    }
}
