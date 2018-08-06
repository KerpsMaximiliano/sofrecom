using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.Models.AllocationManagement
{
    public class AnalyticEditModel : AnalyticModel
    {
        public override Analytic CreateDomain()
        {
            var domain = new Analytic();

            FillData(domain);

            domain.CreationDate = CreationDate;
            domain.Status = Status;

            return domain;
        }
    }
}
