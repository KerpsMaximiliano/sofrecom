using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Sofco.Core.DAL;
using Sofco.Core.FileManager;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;

namespace Sofco.Framework.FileManager.Rrhh.Prepaids
{
    public class MedifePrepaidFileManager : BasePrepaidFileManager, IPrepaidFileManager
    {
        private readonly IUnitOfWork unitOfWork;

        private const int CuilColumn = 6;
        private const int BeneficiariesColumn = 8;
        private const int CostColumn = 2;
        private const int PlanColumn = 9;
        private const int PeriodColumn = 7;

        public MedifePrepaidFileManager(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            FileName = "medife.xlsx";
        }

        public Response<PrepaidDashboard> Process(int yearId, int monthId, IFormFile file, Prepaid prepaid)
        {
            var response = CheckFileName(file);

            if (response.HasErrors()) return response;

            var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);

            var excel = new ExcelPackage(memoryStream);

            var sheet = excel.Workbook.Worksheets.First();

            response = ProcessData(sheet, yearId, monthId, prepaid);

            return response;
        }

        private Response<PrepaidDashboard> ProcessData(ExcelWorksheet sheet, int yearId, int monthId, Prepaid prepaid)
        {
            var response = new Response<PrepaidDashboard>();

            var listToAdd = new List<PrepaidImportedData>();

            var end = false;
            var index = 2;

            while (!end)
            {
                var cuil = sheet.GetValue(index, CuilColumn)?.ToString();

                if (string.IsNullOrWhiteSpace(cuil))
                {
                    end = true;
                    continue;
                }

                var dni = GetDocument(cuil);

                var itemToAdd = new PrepaidImportedData();
                var dniToInt = Convert.ToInt32(dni);

                itemToAdd.Date = new DateTime(yearId, monthId, 1);
                itemToAdd.PrepaidId = prepaid.Id;

                itemToAdd.Dni = dni;
                itemToAdd.Cuil = cuil;
                itemToAdd.PrepaidBeneficiaries = GetPrepaidBeneficiaries(sheet.GetValue(index, BeneficiariesColumn)?.ToString());
                itemToAdd.PrepaidCost = GetPrepaidCost(sheet.GetValue(index, CostColumn)?.ToString());
                itemToAdd.PrepaidPlan = sheet.GetValue(index, PlanColumn)?.ToString();
                itemToAdd.Period = GetPeriod(sheet.GetValue(index, PeriodColumn)?.ToString());

                var employee = unitOfWork.EmployeeRepository.GetByDocumentNumber(dniToInt);

                if (employee == null)
                {
                    itemToAdd.Status = PrepaidImportedDataStatus.Error;
                    itemToAdd.Comments = Resources.Rrhh.Prepaid.EmployeeNotFound;
                }
                else
                {
                    itemToAdd.EmployeeId = employee.Id;
                    itemToAdd.EmployeeName = employee.Name;
                    itemToAdd.TigerBeneficiaries = employee.BeneficiariesCount+1;
                    itemToAdd.EmployeeNumber = employee.EmployeeNumber;
                    itemToAdd.TigerPlan = employee.PrepaidPlan;

                    SetTigerCost(yearId, monthId, employee, itemToAdd);

                    CheckDifferencies(itemToAdd);
                }

                listToAdd.Add(itemToAdd);
                index++;
            }

            if (listToAdd.Count > 0)
            {
                unitOfWork.PrepaidImportedDataRepository.Insert(listToAdd);
                unitOfWork.Save();

                CreateResponse(response, listToAdd);
                response.Data.Prepaid = prepaid.Text;
            }
            else
            {
                response.AddWarning(Resources.Rrhh.Prepaid.FileEmpty);
            }

            return response;
        }

        private string GetDocument(string data)
        {
            if (data.Length != 11) return "1";

            return data.Substring(2, 8);
        }

        private DateTime GetPeriod(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return DateTime.MinValue;

            var dateSplit = data.Split('/');

            if (dateSplit.Length != 2) return DateTime.MinValue;

            try
            {
                return new DateTime(Convert.ToInt32(dateSplit[1]), Convert.ToInt32(dateSplit[0]), 1);
            }
            catch (Exception e)
            {
                return DateTime.MinValue;
            }
        }

        private decimal GetPrepaidCost(string data)
        {
            decimal value = 0;

            decimal.TryParse(data, out value);

            return value;
        }

        private int GetPrepaidBeneficiaries(string data)
        {
            var value = 0;

            int.TryParse(data, out value);

            return value;
        }
    }
}
