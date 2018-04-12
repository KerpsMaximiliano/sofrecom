using Sofco.Model.Models.AllocationManagement;

namespace Sofco.WebApi.Models.AllocationManagement
{
    public class AnalyticEditViewModel : AnalyticViewModel
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
