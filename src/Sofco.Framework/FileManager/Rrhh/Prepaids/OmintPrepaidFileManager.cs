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
    public class OmintPrepaidFileManager : BasePrepaidFileManager, IPrepaidFileManager
    {
        private readonly IUnitOfWork unitOfWork;

        private const int DocumentColumn = 14;
        private const int DocumentTypeColumn = 13;
        private const int CuilColumn = 14;
        private const int CostColumn = 18;
        private const int PlanColumn = 15;
        private const int PeriodColumn = 25;
        private const int TypeColumn = 26;
        private const int CredentialColumn = 2;
        private const int BeneficiariesColumn = 1;

        public OmintPrepaidFileManager(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            FileName = "omint.csv";
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

            var end = false;
            var index = 1;

            PrepaidImportedData itemToAdd = new PrepaidImportedData();
            var credentialAux = string.Empty;

            using (var reader = ExcelReaderFactory.CreateCsvReader(memoryStream))
            {
                do
                {
                    while (reader.Read())
                    {
                        if (index < 2)
                        {
                            index++;
                            continue;
                        }

                        var typeDoc = reader.GetString(DocumentTypeColumn);
                        var nroDoc = GetDni(reader.GetString(DocumentColumn), typeDoc);

                        var credential = reader.GetString(CredentialColumn);
                        var type = reader.GetString(TypeColumn);
                    
                        if (credential != credentialAux)
                        {
                            credentialAux = credential;

                            if (index > 2)
                            {
                                itemToAdd.PrepaidBeneficiaries++;
                                CheckDifferencies(itemToAdd);
                                listToAdd.Add(itemToAdd);
                            }

                            itemToAdd = new PrepaidImportedData();
                            itemToAdd.Date = new DateTime(yearId, monthId, 1);
                            itemToAdd.PrepaidId = prepaid.Id;

                            itemToAdd.Dni = nroDoc;
                            itemToAdd.Cuil = reader.GetString(CuilColumn);
                            itemToAdd.PrepaidBeneficiaries = Convert.ToInt32(reader.GetString( BeneficiariesColumn));
                            itemToAdd.Period = GetPeriod(reader.GetString(PeriodColumn), type);
                            itemToAdd.PrepaidCost = GetPrepaidCost(reader, type);
                            itemToAdd.PrepaidPlan = GetPlan(reader.GetString(PlanColumn), itemToAdd.PrepaidPlan);

                            var dniToInt = Convert.ToInt32(nroDoc);

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
                            itemToAdd.PrepaidBeneficiaries = Convert.ToInt32(reader.GetString(BeneficiariesColumn));
                            itemToAdd.PrepaidCost += GetPrepaidCost(reader, type);
                            itemToAdd.PrepaidPlan = GetPlan(reader.GetString( PlanColumn), itemToAdd.PrepaidPlan);
                            itemToAdd.Period = GetPeriod(reader.GetString(PeriodColumn), type);
                        }

                        index++;
                    }

                } while (reader.NextResult());
            }

            // Insert last item
            if (!string.IsNullOrWhiteSpace(itemToAdd.Dni))
            {
                itemToAdd.PrepaidBeneficiaries++;
                CheckDifferencies(itemToAdd);
                listToAdd.Add(itemToAdd);
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

        private string GetPlan(string data, string prepaidPlan)
        {
            if (string.IsNullOrWhiteSpace(prepaidPlan))
            {
                return data;
            }

            return prepaidPlan;
        }

        private DateTime GetPeriod(string data, string type)
        {
            if(string.IsNullOrWhiteSpace(type) || type == "APORTES") return DateTime.MinValue;

            if (string.IsNullOrWhiteSpace(data)) return DateTime.MinValue;

            var dateSplit = data.Split('/');

            if (dateSplit.Length != 2) return DateTime.MinValue;

            try
            {
                return new DateTime(Convert.ToInt32(dateSplit[0]), Convert.ToInt32(dateSplit[1]), 1);
            }
            catch (Exception e)
            {
                return DateTime.MinValue;
            }
        }

        private decimal GetPrepaidCost(IExcelDataReader data, string type)
        {
            if (string.IsNullOrWhiteSpace(type) || type == "APORTES") return 0;

            var value = data.GetString(CostColumn);

            return Convert.ToDecimal(value);
        }

        private string GetDni(string nroDoc, string typeDoc)
        {
            if (typeDoc == "CUIL")
            {
                var split = nroDoc.Split('-');

                if (split.Length == 3)
                {
                    return split[1];
                }
            }

            if (typeDoc == "DNI")
            {
                return nroDoc;
            }

            return string.Empty;
        }
    }
}
