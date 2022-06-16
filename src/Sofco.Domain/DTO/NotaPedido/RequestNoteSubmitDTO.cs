using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.DTO
{
    public abstract class RequestNoteSubmitDTO
    {
        public int Id { get; set; }
        public string Action { get; set; }
    }
}
