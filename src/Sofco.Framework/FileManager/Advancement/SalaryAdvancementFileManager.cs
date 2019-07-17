using System;
using System.Globalization;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using Sofco.Core.DAL;
using Sofco.Core.FileManager;
using Sofco.Core.Logger;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Framework.FileManager.Advancement
{
    public class SalaryAdvancementFileManager : ISalaryAdvancementFileManager
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<SalaryAdvancementFileManager> logger;

        public SalaryAdvancementFileManager(IUnitOfWork unitOfWork, ILogMailer<SalaryAdvancementFileManager> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public void Import(MemoryStream memoryStream)
        {
            var excel = new ExcelPackage(memoryStream);

            var sheet = excel.Workbook.Worksheets.First();

            var today = DateTime.UtcNow.AddYears(-1);

            var end = false;
            var i = 2;

            var employees = unitOfWork.EmployeeRepository.GetIdAndEmployeeNumber(today.Year, today.Month);

            while (!end)
            {
                var employeeNumber = sheet.GetValue(i, 2)?.ToString();

                if (string.IsNullOrWhiteSpace(employeeNumber))
                {
                    end = true;
                    continue;
                }

                var employee = employees.FirstOrDefault(x => x.Item2 == employeeNumber);

                if (employee == null)
                {
                    i++;
                    continue;
                }

                var dateString = sheet.GetValue(i, 1)?.ToString();
                DateTime date = DateTime.ParseExact(dateString, "dd-MMM-yy", CultureInfo.InvariantCulture);
                var value = Convert.ToDecimal(sheet.GetValue(i, 4)?.ToString())*(-1);

                var discount = new SalaryDiscount
                {
                    Date = date,
                    Amount = value,
                    EmployeeId = employee.Item1
                };

                i++;
                unitOfWork.AdvancementRepository.AddSalaryDiscount(discount);
            }

            unitOfWork.Save();
        }
    }
}
