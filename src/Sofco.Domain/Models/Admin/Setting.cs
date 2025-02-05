﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sofco.Common.Domains;
using Sofco.Domain.Enums;

namespace Sofco.Domain.Models.Admin
{
    public class Setting : IEntityDate
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SettingValueType Type { get; set; }

        public SettingCategory Category { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }
    }
}
