using Sofco.Model.Models.AllocationManagement;

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
            Technology = domain.Technology;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Senority { get; set; }

        public string Profile { get; set; }

        public string Technology { get; set; }
    }
}
