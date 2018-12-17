using System.Collections.Generic;
using Sofco.Domain.Enums;

namespace Sofco.Domain.DTO
{
    public class SolfacChangeStatusResponse
    {
        public ICollection<string> Hitos { get; set; }
        public HitoStatus HitoStatus { get; set; }
    }
}
