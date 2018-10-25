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

        public string ManagersCode { get; set; }

        public string DirectorsCode { get; set; }

        public string GuestCode { get; set; }

        public string CdgCode { get; set; }

        public string PmoCode { get; set; }

        public string QualityCode { get; set; }

        public string RrhhCode { get; set; }

        public string ComplianceCode { get; set; }

        public string SellerCode { get; set; }

        public string ComercialCode { get; set; }

        public string AuthCode { get; set; }

        public string DafAnalytic { get; set; }

        public string MailDevFolder { get; set; }

        public string SupportMailRecipients { get; set; }

        public string SupportMailLogTitle { get; set; }

        public string SupportMailResendRecipients { get; set; }

        public string SupportMailResendTitle { get; set; }

        public int HumanResourceMangerId { get; set; }

        public int HumanResourceProjectLeaderId { get; set; }

        public string PrefixMailEnvironment { get; set; }

        public string AllowedMails { get; set; }
    }
}
