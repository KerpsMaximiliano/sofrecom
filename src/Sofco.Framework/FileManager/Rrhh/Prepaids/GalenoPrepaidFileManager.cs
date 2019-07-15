using System;
using System.Collections.Generic;
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
    public class GalenoPrepaidFileManager : BasePrepaidFileManager, IPrepaidFileManager
    {
        private readonly IUnitOfWork unitOfWork;

        private const int DocumentColumn = 18;
        private const int CostColumn = 22;
        private const int BeneficiariesColumn = 20;
        private const int PlanColumn1 = 10;
        private const int PlanColumn2 = 14;
        private const int TypeColumn = 17;
        private const int PeriodColumn = 6;
        private const int CredentialColumn = 7;

        public GalenoPrepaidFileManager(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            FileName = "galeno.xls";
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
                        if (index < 4)
                        {
                            index++;
                            continue;
                        }

                        var cuil = reader.GetString(DocumentColumn);

                        if (string.IsNullOrWhiteSpace(cuil))
                        {
                            index++;
                            continue;
                        }
                        else
                        {
                            cuil = cuil.Trim();
                        }

                        var type = reader.GetString(TypeColumn).Trim();

                        var credential = reader.GetDouble(CredentialColumn).ToString();

                        if (credential != credentialAux)
                        {
                            credentialAux = credential;

                            if (index > 4)
                            {
                                CheckDifferencies(itemToAdd);
                                listToAdd.Add(itemToAdd);
                            }

                            itemToAdd = new PrepaidImportedData();
                            itemToAdd.Date = new DateTime(yearId, monthId, 1);
                            itemToAdd.PrepaidId = prepaid.Id;

                            itemToAdd.Cuil = cuil;
                            itemToAdd.Dni = GetDni(cuil);
                            itemToAdd.Period = GetPeriod(reader.GetString(PeriodColumn));

                            SetTitularData(reader, type, itemToAdd);

                            var employee = unitOfWork.EmployeeRepository.GetByDocumentNumber(Convert.ToInt32(itemToAdd.Dni));

                            if (employee == null)
                            {
                                itemToAdd.Status = PrepaidImportedDataStatus.Error;
                                itemToAdd.Comments = Resources.Rrhh.Prepaid.EmployeeNotFound;
                            }
                            else
                            {
                                if (!decimal.TryParse(CryptographyHelper.Decrypt(employee.PrepaidAmount), out var prepaidAmount)) prepaidAmount = 0;

                                itemToAdd.EmployeeId = employee.Id;
                                itemToAdd.EmployeeName = employee.Name;
                                itemToAdd.TigerBeneficiaries = employee.BeneficiariesCount+1;
                                itemToAdd.EmployeeNumber = employee.EmployeeNumber;
                                itemToAdd.TigerCost = prepaidAmount;
                                itemToAdd.TigerPlan = employee.PrepaidPlan;
                            }
                        }
                        else
                        {
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

        private string GetDni(string cuil)
        {
            if (cuil.Length != 13) return "1";

            var split = cuil.Split('-');

            return split[1];
        }

        private void SetTitularData(IExcelDataReader reader, string type, PrepaidImportedData itemToAdd)
        {
            if (type == "Plan Medico")
            {
                itemToAdd.PrepaidCost += Convert.ToDecimal(reader.GetDouble(CostColumn));
                itemToAdd.PrepaidPlan = reader.GetString(PlanColumn1).Trim() + " " + reader.GetString(PlanColumn2).Trim();
                itemToAdd.PrepaidBeneficiaries = Convert.ToInt32(reader.GetDouble(BeneficiariesColumn));
            }

            if (type == "Descuento Plan Médico")
            {
                itemToAdd.PrepaidCost += Convert.ToDecimal(reader.GetDouble(CostColumn));
            }
        }
    }
}
