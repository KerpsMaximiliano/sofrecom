namespace Sofco.Domain.DTO
{
    public class EmployeeSearchParams
    {
        public string Name { get; set; }
        public string Seniority { get; set; }
        public string Profile { get; set; }
        public string Technology { get; set; }
        public string EmployeeNumber { get; set; }
        public int? Percentage { get; set; }
        public int? AnalyticId { get; set; }
        public int? SuperiorId { get; set; }
        public int? ManagerId { get; set; }
        public bool Unassigned { get; set; }
        public bool ExternalOnly { get; set; }
    }
}
