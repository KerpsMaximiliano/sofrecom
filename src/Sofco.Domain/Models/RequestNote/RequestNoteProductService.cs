using System;
using System.Collections.Generic;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.RequestNote
{
    public class RequestNoteProductService : BaseEntity
    {
        public int RequestNoteId { get; set; }
        public RequestNote RequestNote { get; set; }
        public string ProductService { get; set; }
        public int Quantity { get; set; }
    }
}
