using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Models.Rrhh
{
    public class CloseDate : BaseEntity
    {
        public int Day { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }
    }
}
