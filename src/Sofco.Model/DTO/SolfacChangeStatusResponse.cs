using System.Collections.Generic;
using Sofco.Model.Enums;

namespace Sofco.Model.DTO
{
    public class SolfacChangeStatusResponse
    {
        public ICollection<string> Hitos { get; set; }
        public HitoStatus HitoStatus { get; set; }
    }
}
