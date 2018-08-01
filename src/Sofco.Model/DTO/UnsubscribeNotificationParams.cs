using System;
using System.Collections.Generic;

namespace Sofco.Domain.DTO
{
    public class UnsubscribeNotificationParams
    {
        public IList<string> Receipents { get; set; }

        public DateTime EndDate { get; set; }

        public string UserName { get; set; }
    }
}
