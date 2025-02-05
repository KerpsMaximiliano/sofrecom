﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    public class SwissPrepaidFileManager : BasePrepaidFileManager, IPrepaidFileManager
    {
        private readonly IUnitOfWork unitOfWork;

        private const int DocumentColumn = 23;
        private const int CuilColumn = 9;
        private const int BeneficiariesColumn = 7;
        private const int CostColumn = 11;
        private const int PlanColumn = 3;
        private const int PeriodColumn = 8;

        public SwissPrepaidFileManager(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            FileName = "swiss.xlsx";
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
                var dni = sheet.GetValue(index, DocumentColumn)?.ToString();

                if (string.IsNullOrWhiteSpace(dni))
                {
                    end = true;
                    continue;
                }

                var itemToAdd = new PrepaidImportedData();
                var dniToInt = Convert.ToInt32(dni);

                itemToAdd.Date = new DateTime(yearId, monthId, 1);
                itemToAdd.PrepaidId = prepaid.Id;

                itemToAdd.Dni = dni;
                itemToAdd.Cuil = sheet.GetValue(index, CuilColumn)?.ToString();
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

        private DateTime GetPeriod(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return DateTime.MinValue;

            try
            {
                #if DEBUG
                    if (DateTime.TryParse(data,
                        System.Globalization.CultureInfo.GetCultureInfo("es-AR"),
                        System.Globalization.DateTimeStyles.None, out DateTime dateValue))
                    {
                        return new DateTime(dateValue.Year, dateValue.Month, 1);
                    }
                    else
                        return DateTime.MinValue;
                #else
                        if (DateTime.TryParse(data, out DateTime dateValue))
                        {
                            return new DateTime(dateValue.Year, dateValue.Month, 1);
                        }
                        else
                            return DateTime.MinValue;
                #endif
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
