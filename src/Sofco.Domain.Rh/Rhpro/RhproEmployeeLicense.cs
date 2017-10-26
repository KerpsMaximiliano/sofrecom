using System;

namespace Sofco.Domain.Rh.Rhpro
{
    public class RhproEmployeeLicense
    {
        public int Empleado { get; set; }

        public DateTime Elfechadesde { get; set; }

        public DateTime Elfechahasta { get; set; }

        public int Tdnro { get; set; }
    }
}
