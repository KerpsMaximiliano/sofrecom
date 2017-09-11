using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sofco.WebApi.Config
{
    public class EmailConfig
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpDomain { get; set; }
        public string EmailFrom { get; set; }
        public string DisplyNameFrom { get; set; }
    }
}
