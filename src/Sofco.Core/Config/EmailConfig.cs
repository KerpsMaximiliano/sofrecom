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

        public int DafMail { get; set; }

        public int CdgMail { get; set; }

        public string MailDevFolder { get; set; }
    }
}
