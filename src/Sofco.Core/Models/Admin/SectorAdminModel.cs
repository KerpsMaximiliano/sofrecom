﻿using System;
using Sofco.Model;

namespace Sofco.Core.Models.Admin
{
    public class SectorAdminModel : BaseEntity
    {
        public int ResponsableId { get; set; }

        public string Text { get; set; }

        public bool Active { get; set; }
    }
}
