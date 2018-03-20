using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sofco.Common.Domains;
using Sofco.Model.Enums;

namespace Sofco.Model.Models.Admin
{
    public class Setting : IEntityDate
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public GlobalSettingType Type { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }
    }
}
