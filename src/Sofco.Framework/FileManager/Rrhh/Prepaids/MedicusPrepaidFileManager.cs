using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Sofco.Core.DAL;
using Sofco.Core.FileManager;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;

namespace Sofco.Framework.FileManager.Rrhh.Prepaids
{
    public class MedicusPrepaidFileManager : BasePrepaidFileManager, IPrepaidFileManager
    {
        private readonly IUnitOfWork unitOfWork;

        private const int DocumentColumn = 16;
        private const int CostColumn = 17;
        private const int PlanColumn = 18;
        private const int PeriodColumn = 2;
        private const int CredentialColumn = 7;
        private const int TypeColumn = 10;

        public MedicusPrepaidFileManager(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            FileName = "medicus.xls";
        }

        public Response<PrepaidDashboard> Process(int yearId, int monthId, IFormFile file, Prepaid prepaid)
        {
            var response = CheckFileName(file);

            if (response.HasErrors()) return response;

            var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);

            response = ProcessData(memoryStream, yearId, monthId, prepaid);

            return response;
        }

        private Response<PrepaidDashboard> ProcessData(MemoryStream memoryStream, int yearId, int monthId, Prepaid prepaid)
        {
            var response = new Response<PrepaidDashboard>();

            var listToAdd = new List<PrepaidImportedData>();

            PrepaidImportedData itemToAdd = new PrepaidImportedData();
            var credentialAux = string.Empty;
            var index = 1;

            using (var reader = ExcelReaderFactory.CreateReader(memoryStream))
            {
                do
                {
                    while (reader.Read())
                    {
                        if (index == 1)
                        {
                            index++;
                            continue;
                        }

                        var dni = reader.GetDouble(DocumentColumn);

                        var credential = reader.GetDouble(CredentialColumn).ToString();
                        var type = reader.GetString(TypeColumn).Trim();

                        if (credential != credentialAux)
                        {
                            credentialAux = credential;

                            if (index > 2)
                            {
                                CheckDifferencies(itemToAdd);
                                listToAdd.Add(itemToAdd);
                            }

                            itemToAdd = new PrepaidImportedData();
                            itemToAdd.Date = new DateTime(yearId, monthId, 1);
                            itemToAdd.PrepaidId = prepaid.Id;

                            itemToAdd.Dni = dni.ToString();
                            itemToAdd.PrepaidBeneficiaries = 1;

                            SetTitularData(reader, type, itemToAdd);

                            var dniToInt = Convert.ToInt32(dni);

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
                            }
                        }
                        else
                        {
                            itemToAdd.PrepaidBeneficiaries++;
                            SetTitularData(reader, type, itemToAdd);
                        }

                        index++;
                    }

                } while (reader.NextResult());

                // Insert last item
                if (!string.IsNullOrWhiteSpace(itemToAdd.Dni))
                {
                    CheckDifferencies(itemToAdd);
                    listToAdd.Add(itemToAdd);
                }
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

        private void SetTitularData(IExcelDataReader reader, string type, PrepaidImportedData itemToAdd)
        {
            if (type == "TITULAR")
            {
                itemToAdd.PrepaidCost = Convert.ToDecimal(reader.GetDouble(CostColumn));
                itemToAdd.PrepaidPlan = reader.GetString( PlanColumn).Trim();
                itemToAdd.Period = GetPeriod(reader.GetDouble( PeriodColumn).ToString(CultureInfo.InvariantCulture));
            }
        }

        private DateTime GetPeriod(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return DateTime.MinValue;

            if (data.Length != 7 && data.Length != 8) return DateTime.MinValue;

            try
            {
                if (data.Length == 7)
                {
                    var month = Convert.ToInt32(data.Substring(1, 2));
                    var year = Convert.ToInt32(data.Substring(3, 4));

                    return new DateTime(year, month, 1);
                }

                if (data.Length == 8)
                {
                    var month = Convert.ToInt32(data.Substring(0, 2));
                    var year = Convert.ToInt32(data.Substring(2, 4));

                    return new DateTime(year, month, 1);
                }
            }
            catch (Exception e)
            {
                return DateTime.MinValue;
            }

            return DateTime.MinValue;
        }
    }
}
