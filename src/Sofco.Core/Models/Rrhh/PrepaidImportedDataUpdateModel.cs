using System.Collections.Generic;
using Sofco.Domain.Enums;

namespace Sofco.Core.Models.Rrhh
{
    public class PrepaidImportedDataUpdateModel
    {
        public IList<int> Ids { get; set; }

        public PrepaidImportedDataStatus? Status { get; set; }
    }
}
