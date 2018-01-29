using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sofco.Model.DTO
{
    public class UnsubscribeNotificationParams
    {
        public IList<string> Receipents { get; set; }

        public DateTime EndDate { get; set; }

        public string UserName { get; set; }
    }
}
