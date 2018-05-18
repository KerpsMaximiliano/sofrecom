﻿using System;
using Sofco.Common.Domains;
using Sofco.Model.Enums;

namespace Sofco.Model.Models.WorkTimeManagement
{
    public class Holiday : BaseEntity, IEntityDate
    {
        public DateTime Date { get; set; }

        public string Name { get; set; }

        public HolidayDataSource DataSource { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }
    }
}
