using Sofco.Model.Models.TimeManagement;

namespace Sofco.WebApi.Models.AllocationManagement
{
    public class EmployeeOptionModel
    {
        public EmployeeOptionModel(Employee domain)
        {
            Id = domain.Id;
            Name = $"{domain.EmployeeNumber} - {domain.Name}";
            Senority = domain.Seniority;
            Profile = domain.Profile;
            BillingPercentage = domain.BillingPercentage;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Senority { get; set; }
        public string Profile { get; set; }
        public decimal BillingPercentage { get; set; }
    }
}
