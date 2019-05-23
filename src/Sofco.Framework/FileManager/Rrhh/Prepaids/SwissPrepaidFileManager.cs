using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Sofco.Core.DAL;
using Sofco.Core.FileManager;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Utils;

namespace Sofco.Framework.FileManager.Rrhh.Prepaids
{
    public class SwissPrepaidFileManager : IPrepaidFileManager
    {
        private readonly IUnitOfWork unitOfWork;

        private const int DocumentColumn = 23;
        private const int CuilColumn = 9;
        private const int BeneficiariesColumn = 7;
        private const int CosColumn = 11;
        private const int PlanColumn = 3;
        private const int PeriodColumn = 8;
        private const int PrepaidColumn = 20;

        public SwissPrepaidFileManager(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Response Process(int yearId, int monthId, IFormFile file)
        {
           

            var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);

            var excel = new ExcelPackage(memoryStream);

            var sheet = excel.Workbook.Worksheets.First();

            var response = ProcessData(sheet, yearId, monthId);

            return response;
        }

        private Response ProcessData(ExcelWorksheet sheet, int yearId, int monthId)
        {
            var response = new Response();

            var listToAdd = new List<PrepaidImportedData>();

            var end = false;
            var index = 2;

            while (!end)
            {
                var dni = sheet.GetValue(index, DocumentColumn)?.ToString();

                if (string.IsNullOrWhiteSpace(dni))
                {
                    end = true;
                    continue;
                }

                var itemToAdd = new PrepaidImportedData();
                var dniToInt = Convert.ToInt32(dni);

                itemToAdd.Date = new DateTime(yearId, monthId, 1);

                itemToAdd.Dni = dni;
                itemToAdd.Cuil = sheet.GetValue(index, CuilColumn)?.ToString();
                itemToAdd.PrepaidBeneficiaries = GetPrepaidBeneficiaries(sheet.GetValue(index, BeneficiariesColumn)?.ToString());
                itemToAdd.PrepaidCost = GetPrepaidCost(sheet.GetValue(index, CosColumn)?.ToString());
                itemToAdd.PrepaidPlan = sheet.GetValue(index, PlanColumn)?.ToString();
                itemToAdd.Prepaid = sheet.GetValue(index, PrepaidColumn)?.ToString();
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
                    itemToAdd.TigerBeneficiaries = employee.BeneficiariesCount;
                    itemToAdd.EmployeeNumber = employee.EmployeeNumber;
                    itemToAdd.TigerCost = employee.PrepaidAmount;
                    itemToAdd.TigerPlan = employee.PrepaidPlan;

                    CheckDifferencies(itemToAdd);
                }

                listToAdd.Add(itemToAdd);
                index++;
            }

            if (listToAdd.Count > 0)
            {
                unitOfWork.PrepaidImportedDataRepository.Insert(listToAdd);
                unitOfWork.Save();
            }
            else
            {
                response.AddWarning(Resources.Rrhh.Prepaid.FileEmpty);
            }

            return response;
        }

        private void CheckDifferencies(PrepaidImportedData itemToAdd)
        {
            if (itemToAdd.TigerBeneficiaries != itemToAdd.PrepaidBeneficiaries)
            {
                itemToAdd.Status = PrepaidImportedDataStatus.Error;
                itemToAdd.Comments = string.Concat(itemToAdd.Comments, Resources.Rrhh.Prepaid.DifferenceInBeneficiaires);
            }

            if (itemToAdd.TigerCost != itemToAdd.PrepaidCost)
            {
                itemToAdd.Status = PrepaidImportedDataStatus.Error;
                itemToAdd.Comments = string.Concat(itemToAdd.Comments, Resources.Rrhh.Prepaid.DifferenceInCosts);
            }

            if (itemToAdd.TigerPlan != itemToAdd.PrepaidPlan)
            {
                itemToAdd.Status = PrepaidImportedDataStatus.Error;
                itemToAdd.Comments = string.Concat(itemToAdd.Comments, Resources.Rrhh.Prepaid.DifferenceInPlan);
            }

            if (itemToAdd.Date.Date != itemToAdd.Period.Date)
            {
                itemToAdd.Status = PrepaidImportedDataStatus.Error;
                itemToAdd.Comments = string.Concat(itemToAdd.Comments, Resources.Rrhh.Prepaid.DifferenceInPeriod);
            }
        }

        private DateTime GetPeriod(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return DateTime.MinValue;

            var dataSplit = data.Split(' ');

            if(dataSplit.Length != 4) return DateTime.MinValue;

            var dateSplit = dataSplit[0].Split('/');

            if (dateSplit.Length != 3) return DateTime.MinValue;

            try
            {
                return new DateTime(Convert.ToInt32(dateSplit[2]), Convert.ToInt32(dateSplit[1]), 1);
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
