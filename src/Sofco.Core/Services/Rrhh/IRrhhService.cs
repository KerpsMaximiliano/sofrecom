using System;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Rrhh
{
    public interface IRrhhService
    {
        Response<byte[]> GenerateTigerTxt(bool allUsers);
        Response UpdateSocialCharges(int year, int month);
        Response UpdateManagers();
        Response<SalaryReportResponse> GetSalaryReport(DateTime? startDate, DateTime? endDate);
    }
}
