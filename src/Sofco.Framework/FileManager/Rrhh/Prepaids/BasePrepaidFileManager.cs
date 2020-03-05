using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;

namespace Sofco.Framework.FileManager.Rrhh.Prepaids
{
    public class BasePrepaidFileManager
    {
        protected string FileName;

        protected readonly int costTigerAccountNumber = 930;
        protected readonly int costoNetoAccountNumber = 962;
        protected readonly int netoProviderAccountNumber = 960;

        public void CheckDifferencies(PrepaidImportedData itemToAdd)
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

            //if (!string.IsNullOrWhiteSpace(itemToAdd.TigerPlan) && !string.IsNullOrWhiteSpace(itemToAdd.PrepaidPlan))
            //{
            //    if (itemToAdd.TigerPlan.Trim().ToLowerInvariant() != itemToAdd.PrepaidPlan.Trim().ToLowerInvariant())
            //    {
            //        itemToAdd.Status = PrepaidImportedDataStatus.Error;
            //        itemToAdd.Comments = string.Concat(itemToAdd.Comments, Resources.Rrhh.Prepaid.DifferenceInPlan);
            //    }
            //}

            if (itemToAdd.Date.Date != itemToAdd.Period.Date)
            {
                itemToAdd.Status = PrepaidImportedDataStatus.Error;
                itemToAdd.Comments = string.Concat(itemToAdd.Comments, Resources.Rrhh.Prepaid.DifferenceInPeriod);
            }
        }

        public Response<PrepaidDashboard> CheckFileName(IFormFile file)
        {
            var response = new Response<PrepaidDashboard>();

            if (file.FileName.Trim().ToLowerInvariant() != FileName)
            {
                response.AddError(Resources.Rrhh.Prepaid.FileWrong);
            }

            return response;
        }

        protected void CreateResponse(Response<PrepaidDashboard> response, IList<PrepaidImportedData> list)
        {
            list = list.ToList();

            response.Data = new PrepaidDashboard
            {
                CountSuccess = list.Count(x => x.Status == PrepaidImportedDataStatus.Success),
                CountError = list.Count(x => x.Status == PrepaidImportedDataStatus.Error),
            };
        }

        protected void SetTigerCost(int yearId, int monthId, Employee employee, PrepaidImportedData itemToAdd)
        {
            if (employee.SocialCharges != null)
            {
                var socialCharge = employee.SocialCharges.Where(x => x.Year == yearId && x.Month == monthId).ToList();

                if (socialCharge.Any())
                {
                    decimal total = 0;

                    foreach (var charge in socialCharge)
                    {
                        var item = charge.Items.FirstOrDefault(x => x.AccountNumber == costTigerAccountNumber);
                        var costoNetoItem = charge.Items.FirstOrDefault(x => x.AccountNumber == costoNetoAccountNumber);
                        var netoProviderItem = charge.Items.FirstOrDefault(x => x.AccountNumber == netoProviderAccountNumber);

                        if (item != null)
                        {
                            var valueString = CryptographyHelper.Decrypt(item.Value);

                            if (!string.IsNullOrWhiteSpace(valueString))
                            {
                                if (!decimal.TryParse(valueString, out var prepaidAmount)) prepaidAmount = 0;

                                itemToAdd.TigerCost += prepaidAmount;
                            }
                        }

                        if (costoNetoItem != null)
                        {
                            var valueString = CryptographyHelper.Decrypt(costoNetoItem.Value);

                            if (!string.IsNullOrWhiteSpace(valueString))
                            {
                                if (!decimal.TryParse(valueString, out var prepaidAmount)) prepaidAmount = 0;

                                itemToAdd.CostDifference = prepaidAmount;
                            }
                        }
                        else
                        {
                            itemToAdd.CostDifference = itemToAdd.PrepaidCost - itemToAdd.TigerCost;
                        }

                        if (netoProviderItem != null)
                        {
                            var valueString = CryptographyHelper.Decrypt(netoProviderItem.Value);

                            if (!string.IsNullOrWhiteSpace(valueString))
                            {
                                if (!decimal.TryParse(valueString, out var prepaidAmount)) prepaidAmount = 0;

                                itemToAdd.NetoProvider = prepaidAmount;
                            }
                        }
                    }
                }
            }
        }
    }
}
