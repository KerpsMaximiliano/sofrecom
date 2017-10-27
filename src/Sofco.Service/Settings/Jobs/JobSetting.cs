namespace Sofco.Service.Settings.Jobs
{
    public class JobSetting
    {
        public string LocalTimeZoneName { get; set; }

        public SolfacJobSetting SolfacJob { get; set; }

        public EmployeeEndJobSetting EmployeeEndJob { get; set; }
    }
}
