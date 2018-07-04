using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sofco.Common.Domains;
using Sofco.Model.Enums;

namespace Sofco.Model.Models.Common
{
    public class UserDelegate : BaseEntity, IEntityDate
    {
        public Guid? ServiceId { get; set; }

        public int? SourceId { get; set; }

        public int UserId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public UserDelegateType Type { get; set; }

        public string CreatedUser { get; set; }

        public DateTime? Created { get; set; }

        [JsonIgnore]
        public DateTime? Modified { get; set; }
    }
}
