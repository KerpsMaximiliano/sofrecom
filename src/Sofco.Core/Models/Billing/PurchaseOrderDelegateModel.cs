using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sofco.Model.Enums;

namespace Sofco.Core.Models.Billing
{
    public class PurchaseOrderDelegateModel
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
