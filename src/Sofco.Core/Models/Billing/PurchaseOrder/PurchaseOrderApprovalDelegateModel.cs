using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sofco.Domain.Enums;

namespace Sofco.Core.Models.Billing.PurchaseOrder
{
    public class PurchaseOrderApprovalDelegateModel
    {
        public int Id { get; set; }

        public int SourceId { get; set; }

        public string SourceName { get; set; }

        public int ResponsableId { get; set; }

        public string ResponsableName { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string CreatedUser { get; set; }

        public DateTime? Created { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public UserDelegateType Type { get; set; }
    }
}
