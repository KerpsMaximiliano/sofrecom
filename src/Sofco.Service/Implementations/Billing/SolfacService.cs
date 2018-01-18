using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.CrmServices;
using Sofco.Core.DAL;
using Sofco.Core.Services.Billing;
using Sofco.Core.StatusHandlers;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Model.Helpers;

namespace Sofco.Service.Implementations.Billing
{
    public class SolfacService : ISolfacService
    {
        private readonly ISolfacStatusFactory solfacStatusFactory;
        private readonly IUnitOfWork unitOfWork;
        private readonly CrmConfig crmConfig;
        private readonly IMailSender mailSender;
        private readonly ICrmInvoiceService crmInvoiceService;
        private readonly ILogMailer<SolfacService> logger;

        public SolfacService(ISolfacStatusFactory solfacStatusFactory,
            IUnitOfWork unitOfWork,
            IOptions<CrmConfig> crmOptions,
            IMailSender mailSender,
            ICrmInvoiceService crmInvoiceService, ILogMailer<SolfacService> logger)
        {
            this.solfacStatusFactory = solfacStatusFactory;
            crmConfig = crmOptions.Value;
            this.unitOfWork = unitOfWork;
            this.mailSender = mailSender;
            this.crmInvoiceService = crmInvoiceService;
            this.logger = logger;
        }

        public Response<Solfac> CreateSolfac(Solfac solfac, IList<int> invoicesId)
        {
            var response = Validate(solfac);

            if (response.HasErrors()) return response;

            try
            {
                CreateHitoOnCrm(solfac);

                solfac.UpdatedDate = DateTime.Now;
                solfac.ModifiedByUserId = solfac.UserApplicantId;

                // Add History
                solfac.Histories.Add(GetHistory(SolfacStatus.None, solfac.Status, solfac.UserApplicantId,
                    string.Empty));

                // Insert Solfac
                unitOfWork.SolfacRepository.Insert(solfac);

                // Save
                unitOfWork.Save();

                // Update Invoice Status to Related
                foreach (var invoiceId in invoicesId)
                {
                    var invoiceToModif = new Invoice
                    {
                        Id = invoiceId,
                        SolfacId = solfac.Id,
                        InvoiceStatus = InvoiceStatus.Related
                    };
                    unitOfWork.InvoiceRepository.UpdateSolfacId(invoiceToModif);
                    unitOfWork.InvoiceRepository.UpdateStatus(invoiceToModif);
                }

                if (invoicesId.Any())
                {
                    // Save Invoices related
                    unitOfWork.Save();
                }

                response.Data = solfac;
                response.Messages.Add(new Message(Resources.Billing.Solfac.SolfacCreated, MessageType.Success));

                if (SolfacHelper.IsCreditNote(solfac) || SolfacHelper.IsDebitNote(solfac))
                    return response;

                var crmResult = crmInvoiceService.UpdateHitos(solfac.Hitos);

                response.AddMessages(crmResult.Messages);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public IList<Solfac> Search(SolfacParams parameter, string userMail, EmailConfig emailConfig)
        {
            var isDirector = unitOfWork.UserRepository.HasDirectorGroup(userMail);
            var isDaf = unitOfWork.UserRepository.HasDafGroup(userMail, emailConfig.DafCode);
            var isCdg = unitOfWork.UserRepository.HasCdgGroup(userMail, emailConfig.CdgCode);

            if (isDirector || isDaf || isCdg)
            {
                return unitOfWork.SolfacRepository.SearchByParams(parameter);
            }
            else
            {
                return unitOfWork.SolfacRepository.SearchByParamsAndUser(parameter, userMail);
            }
        }

        public IList<Hito> GetHitosByProject(string projectId)
        {
            return unitOfWork.SolfacRepository.GetHitosByProject(projectId);
        }

        public IList<Solfac> GetByProject(string projectId)
        {
            return unitOfWork.SolfacRepository.GetByProject(projectId);
        }

        public Response<Solfac> GetById(int id)
        {
            var response = new Response<Solfac>();

            var solfac = unitOfWork.SolfacRepository.GetById(id);

            if (solfac == null)
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.NotFound, MessageType.Error));
                return response;
            }

            response.Data = solfac;
            return response;
        }

        public Response ChangeStatus(int solfacId, SolfacStatusParams parameters, EmailConfig emailConfig)
        {
            var response = new Response<SolfacChangeStatusResponse>();

            var solfac = SolfacValidationHelper.ValidateIfExistAndGetWithUser(solfacId, unitOfWork.SolfacRepository, response);

            if (response.HasErrors()) return response;

            return ChangeStatus(solfac, parameters, emailConfig);
        }

        public Response ChangeStatus(Solfac solfac, SolfacStatusParams parameters, EmailConfig emailConfig)
        {
            var response = new Response();

            var solfacStatusHandler = solfacStatusFactory.GetInstance(parameters.Status);

            try
            {
                // Validate status
                var statusErrors = solfacStatusHandler.Validate(solfac, parameters);

                if (statusErrors.Messages.Any()) response.AddMessages(statusErrors.Messages);

                if (response.HasErrors()) return response;

                // Update Status
                solfacStatusHandler.SaveStatus(solfac, parameters);

                // Add history
                var history = GetHistory(solfac.Id, solfac.Status, parameters.Status, parameters.UserId, parameters.Comment);
                unitOfWork.SolfacRepository.AddHistory(history);

                // Save
                unitOfWork.Save();
                response.Messages.Add(new Message(solfacStatusHandler.GetSuccessMessage(), MessageType.Success));

                // Update Hitos
                solfacStatusHandler.UpdateHitos(unitOfWork.SolfacRepository.GetHitosIdsBySolfacId(solfac.Id), solfac, crmConfig.Url);
            }
            catch
            {
                response = new Response();
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
                return response;
            }

            try
            {
                // Send Mail
                solfacStatusHandler.SendMail(mailSender, solfac, emailConfig);
            }
            catch
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSendMail, MessageType.Warning));
            }

            return response;
        }

        public Response Delete(int id)
        {
            var response = new Response();

            var solfac = SolfacValidationHelper.ValidateIfExist(id, unitOfWork.SolfacRepository, response);

            if (response.HasErrors()) return response;

            try
            {
                var invoices = unitOfWork.InvoiceRepository.GetBySolfac(id);

                foreach (var invoice in invoices)
                {
                    invoice.InvoiceStatus = InvoiceStatus.Approved;
                    invoice.SolfacId = null;
                    unitOfWork.InvoiceRepository.UpdateStatus(invoice);
                    unitOfWork.InvoiceRepository.UpdateSolfacId(invoice);
                }

                unitOfWork.SolfacRepository.Delete(solfac);
                unitOfWork.Save();

                response.Messages.Add(new Message(Resources.Billing.Solfac.Deleted, MessageType.Success));
            }
            catch (Exception ex)
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public ICollection<SolfacHistory> GetHistories(int id)
        {
            return unitOfWork.SolfacRepository.GetHistories(id);
        }

        public Response Update(Solfac solfac, string comments)
        {
            var response = Validate(solfac);

            if (response.HasErrors()) return response;

            try
            {
                solfac.UpdatedDate = DateTime.Now;

                // Add History
                solfac.Histories.Add(GetHistory(solfac.Status, solfac.Status, solfac.UserApplicantId, comments));

                // Insert Solfac
                unitOfWork.SolfacRepository.Update(solfac);

                // Save changes
                unitOfWork.Save();

                // Update hitos in CRM
                var crmResult = crmInvoiceService.UpdateHitos(solfac.Hitos);

                response.AddMessages(crmResult.Messages);

                response.Messages.Add(new Message(Resources.Billing.Solfac.SolfacUpdated, MessageType.Success));
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<SolfacAttachment> SaveFile(int solfacId, byte[] fileAsArrayBytes, string fileFileName)
        {
            var response = new Response<SolfacAttachment>();

            var solfac = unitOfWork.SolfacRepository.GetSingle(x => x.Id == solfacId);

            if (solfac == null)
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.NotFound, MessageType.Error));
                return response;
            }

            var attachment = new SolfacAttachment
            {
                SolfacId = solfacId,
                Name = fileFileName,
                CreationDate = DateTime.Now,
                File = fileAsArrayBytes
            };

            unitOfWork.SolfacRepository.SaveAttachment(attachment);
            unitOfWork.Save();

            response.Data = attachment;
            response.Messages.Add(new Message(Resources.Billing.Solfac.FileAdded, MessageType.Success));

            return response;
        }

        public ICollection<SolfacAttachment> GetFiles(int solfacId)
        {
            return unitOfWork.SolfacRepository.GetFiles(solfacId);
        }

        public Response<SolfacAttachment> GetFileById(int fileId)
        {
            var response = new Response<SolfacAttachment>();

            var file = unitOfWork.SolfacRepository.GetFileById(fileId);

            if (file == null)
            {
                response.Messages.Add(new Message(Resources.Common.FileNotFound, MessageType.Error));
                return response;
            }

            response.Data = file;
            return response;
        }

        public Response DeleteFile(int id)
        {
            var response = new Response();

            var file = unitOfWork.SolfacRepository.GetFileById(id);

            if (file == null)
            {
                response.Messages.Add(new Message(Resources.Common.FileNotFound, MessageType.Error));
                return response;
            }

            try
            {
                unitOfWork.SolfacRepository.DeleteFile(file);
                unitOfWork.Save();
                response.Messages.Add(new Message(Resources.Billing.Solfac.FileDeleted, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Solfac> Validate(Solfac solfac)
        {
            var response = new Response<Solfac>();

            SolfacValidationHelper.ValidateProvincePercentage(solfac, response);
            SolfacValidationHelper.ValidateHitos(solfac.Hitos, response);
            SolfacValidationHelper.ValidatePercentage(solfac, response);
            SolfacValidationHelper.ValidateTimeLimit(solfac, response);
            SolfacValidationHelper.ValidateContractNumber(solfac, response);
            SolfacValidationHelper.ValidateImputationNumber(solfac, response);
            SolfacValidationHelper.ValidateBusinessName(solfac, response);

            if (SolfacHelper.IsCreditNote(solfac))
            {
                SolfacValidationHelper.ValidateCreditNote(solfac, unitOfWork.SolfacRepository, response);
            }

            if (SolfacHelper.IsDebitNote(solfac) || SolfacHelper.IsCreditNote(solfac))
            {
                var hito = solfac.Hitos.First();
                HitoValidatorHelper.ValidateOpportunity(new HitoSplittedParams { OpportunityId = hito.OpportunityId }, response);
            }

            return response;
        }

        public Response DeleteDetail(int id)
        {
            var response = new Response();

            var detail = unitOfWork.SolfacRepository.GetDetail(id);

            if (detail == null)
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.DetailNotFound, MessageType.Error));
                return response;
            }

            try
            {
                unitOfWork.SolfacRepository.DeleteDetail(detail);
                unitOfWork.Save();
                response.Messages.Add(new Message(Resources.Billing.Solfac.DetailDeleted, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public async Task<Response> SplitHito(HitoSplittedParams hito)
        {
            var response = ValidateHitoSplitted(hito);

            if (response.HasErrors()) return response;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmConfig.Url);

                await UpdateFirstHito(response, hito, client);
                await CreateNewHito(response, hito, client);

                if (!response.HasErrors())
                {
                    response.Messages.Add(new Message(Resources.Billing.Project.HitoSplitted, MessageType.Success));
                }
            }

            return response;
        }

        private async Task CreateNewHito(Response response, HitoSplittedParams hito, HttpClient client)
        {
            var data =
                $"Ammount={hito.Ammount}&StatusCode=1&StartDate={hito.StartDate:O}&Name={hito.Name}&MoneyId={hito.MoneyId}" +
                $"&Month={hito.Month}&ProjectId={hito.ProjectId}&OpportunityId={hito.OpportunityId}&ManagerId={hito.ManagerId}";

            var urlPath = "/api/InvoiceMilestone";

            try
            {
                var stringContent = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

                var httpResponse = await client.PostAsync(urlPath, stringContent);

                httpResponse.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                logger.LogError(urlPath +"; data: "+ data,ex);
                response.Messages.Add(new Message(Resources.Billing.Solfac.ErrorSaveOnHitos, MessageType.Error));
            }
        }

        private async Task UpdateFirstHito(Response response, HitoSplittedParams hito, HttpClient client)
        {
            var closeStatusCode = crmConfig.CloseStatusCode;

            if (hito.AmmountFirstHito == 0 || hito.StatusCode == closeStatusCode) return;

            if (hito.AmmountFirstHito - hito.Ammount <= 0)
                hito.AmmountFirstHito = 0;
            else
                hito.AmmountFirstHito -= hito.Ammount.GetValueOrDefault();

            var data = $"Ammount={hito.AmmountFirstHito}";

            if (hito.AmmountFirstHito == 0) data += "&StatusCode="+ closeStatusCode;

            var urlPath = $"/api/InvoiceMilestone/{hito.ExternalHitoId}";

            try
            {
                var stringContent = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

                var httpResponse = await client.PutAsync(urlPath, stringContent);

                httpResponse.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                logger.LogError(urlPath + "; data: " + data, ex);
                response.Messages.Add(new Message(Resources.Billing.Solfac.ErrorSaveOnHitos, MessageType.Error));
            }
        }

        private Response ValidateHitoSplitted(HitoSplittedParams hito)
        {
            var response = new Response();

            HitoValidatorHelper.ValidateName(hito, response);
            HitoValidatorHelper.ValidateMonth(hito, response);
            HitoValidatorHelper.ValidateAmmounts(hito, response);
            HitoValidatorHelper.ValidateOpportunity(hito, response);

            return response;
        }

        private SolfacHistory GetHistory(int solfacId, SolfacStatus statusFrom, SolfacStatus statusTo, int userId,
            string comment)
        {
            var history = GetHistory(statusFrom, statusTo, userId, comment);
            history.SolfacId = solfacId;
            return history;
        }

        private SolfacHistory GetHistory(SolfacStatus statusFrom, SolfacStatus statusTo, int userId, string comment)
        {
            var history = new SolfacHistory
            {
                SolfacStatusFrom = statusFrom,
                SolfacStatusTo = statusTo,
                UserId = userId,
                Comment = comment,
                CreatedDate = DateTime.Now
            };

            return history;
        }

        public Response UpdateBill(int id, SolfacStatusParams parameters)
        {
            var response = new Response();

            var solfac = SolfacValidationHelper.ValidateIfExistAndGetWithUser(id, unitOfWork.SolfacRepository, response);

            if (response.HasErrors()) return response;

            SolfacValidationHelper.ValidateInvoiceCode(parameters, unitOfWork.SolfacRepository, response, solfac.InvoiceCode);
            SolfacValidationHelper.ValidateInvoiceDate(parameters, response, solfac);

            if (response.HasErrors()) return response;

            try
            {
                var solfacToModif = new Solfac
                {
                    Id = solfac.Id,
                    InvoiceCode = parameters.InvoiceCode,
                    InvoiceDate = parameters.InvoiceDate
                };
                unitOfWork.SolfacRepository.UpdateInvoice(solfacToModif);

                var history = GetHistory(solfac.Id, solfac.Status, solfac.Status, parameters.UserId, string.Empty);
                unitOfWork.SolfacRepository.AddHistory(history);

                unitOfWork.Save();
                response.Messages.Add(new Message(Resources.Billing.Solfac.BillUpdated, MessageType.Success));
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            try
            {
                crmInvoiceService.UpdateHitoInvoice(unitOfWork.SolfacRepository.GetHitosBySolfacId(solfac.Id), parameters);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Billing.Solfac.ErrorSaveOnHitos);
            }

            return response;
        }

        public Response UpdateCashedDate(int id, SolfacStatusParams parameters)
        {
            var response = new Response();

            var solfac = SolfacValidationHelper.ValidateIfExist(id, unitOfWork.SolfacRepository, response);

            if (response.HasErrors()) return response;

            SolfacValidationHelper.ValidateCasheDate(parameters, response, solfac);

            if (response.HasErrors()) return response;

            try
            {
                var solfacToModif = new Solfac { Id = solfac.Id, CashedDate = parameters.CashedDate };
                unitOfWork.SolfacRepository.UpdateCash(solfacToModif);

                var history = GetHistory(solfac.Id, solfac.Status, solfac.Status, parameters.UserId, string.Empty);
                unitOfWork.SolfacRepository.AddHistory(history);

                unitOfWork.Save();
                response.Messages.Add(new Message(Resources.Billing.Solfac.CashUpdated, MessageType.Success));
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response DeleteInvoice(int id, int invoiceId)
        {
            var response = new Response();

            SolfacValidationHelper.ValidateIfExist(id, unitOfWork.SolfacRepository, response);

            if (response.HasErrors()) return response;

            try
            {
                var invoice = unitOfWork.InvoiceRepository.GetSingle(x => x.Id == invoiceId);

                if (invoice != null)
                {
                    invoice.InvoiceStatus = InvoiceStatus.Approved;
                    invoice.SolfacId = null;
                    unitOfWork.InvoiceRepository.UpdateStatus(invoice);
                    unitOfWork.InvoiceRepository.UpdateSolfacId(invoice);

                    unitOfWork.Save();

                    response.Messages.Add(new Message(Resources.Billing.Solfac.InvoiceDeleted, MessageType.Success));
                }
                else
                {
                    response.Messages.Add(new Message(Resources.Billing.Invoice.NotFound, MessageType.Error));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<ICollection<Invoice>> GetInvoices(int id)
        {
            var response = new Response<ICollection<Invoice>>();

            SolfacValidationHelper.ValidateIfExist(id, unitOfWork.SolfacRepository, response);

            if (response.HasErrors()) return response;

            response.Data = unitOfWork.InvoiceRepository.GetBySolfac(id);

            return response;
        }

        public Response<List<Invoice>> AddInvoices(int id, IList<int> invoices)
        {
            var response = new Response<List<Invoice>> { Data = new List<Invoice>() };

            var solfac = SolfacValidationHelper.ValidateIfExist(id, unitOfWork.SolfacRepository, response);

            if (response.HasErrors()) return response;

            try
            {
                foreach (var invoiceToAdd in invoices)
                {
                    var invoice = unitOfWork.InvoiceRepository.GetSingle(x => x.Id == invoiceToAdd);

                    if (invoice != null)
                    {
                        invoice.InvoiceStatus = InvoiceStatus.Related;
                        invoice.SolfacId = id;
                        unitOfWork.InvoiceRepository.UpdateStatus(invoice);
                        unitOfWork.InvoiceRepository.UpdateSolfacId(invoice);

                        response.Data.Add(new Invoice { Id = invoice.Id, InvoiceNumber = invoice.InvoiceNumber, PdfFileName = invoice.PdfFileName });
                    }
                    else
                    {
                        response.Messages.Add(new Message(Resources.Billing.Invoice.NotFound, MessageType.Warning));
                    }
                }

                unitOfWork.Save();

                response.Messages.Add(new Message(Resources.Billing.Solfac.InvoicesAdded, MessageType.Success));
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Solfac> Post(Solfac solfac, IList<int> invoicesId)
        {
            var result = CreateSolfac(solfac, invoicesId);

            if (result.HasErrors())
                return result;

            if (SolfacHelper.IsCreditNote(solfac) || SolfacHelper.IsDebitNote(solfac))
                return result;

            crmInvoiceService.UpdateHitoStatus(result.Data.Hitos.ToList(), HitoStatus.Pending);

            return result;
        }

        private void CreateHitoOnCrm(Solfac solfac)
        {
            if (!SolfacHelper.IsCreditNote(solfac) && !SolfacHelper.IsDebitNote(solfac))
                return;

            var hito = solfac.Hitos.First();

            hito.SolfacId = 0;

            var hitoResult = crmInvoiceService.CreateHitoBySolfac(solfac);

            hito.ExternalHitoId = hitoResult.Data;
        }
    }
}
