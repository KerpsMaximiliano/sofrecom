﻿namespace Sofco.Model.Models.Billing
{
    public class Hito
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public short Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public string ExternalId { get; set; }

        public int SolfacId { get; set; }
        public Solfac Solfac { get; set; }
    }
}
