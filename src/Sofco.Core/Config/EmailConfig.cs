namespace Sofco.Core.Config
{
    public class EmailConfig
    {
        public string SmtpServer { get; set; }

        public int SmtpPort { get; set; }

        public string SmtpDomain { get; set; }

        public string EmailFrom { get; set; }

        public string DisplyNameFrom { get; set; }

        public string SiteUrl { get; set; }

        public string DafCode { get; set; }

        public string CdgCode { get; set; }

        public string PmoCode { get; set; }

        public string MailDevFolder { get; set; }
    }
}
