using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sofco.Common.Domains;
using Sofco.Core.Config;
using Sofco.Core.CrmServices;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Core.StatusHandlers;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;
using Sofco.Core.DAL.Admin;
using Sofco.Core.Mail;
using Sofco.Framework.ValidationHelpers.Billing;

namespace Sofco.Service.Implementations.Billing
{
    public class SolfacService : ISolfacService
    {
        private readonly ISolfacRepository solfacRepository;
        private readonly IInvoiceRepository invoiceRepository;
        private readonly ISolfacStatusFactory solfacStatusFactory;
        private readonly IUserRepository userRepository;
        private readonly CrmConfig crmConfig;
        private readonly IMailSender mailSender;
        private readonly ICrmInvoiceService crmInvoiceService;

        public SolfacService(ISolfacRepository solfacRepository,
            IInvoiceRepository invoiceRepository,
            ISolfacStatusFactory solfacStatusFactory,
            IUserRepository userRepository,
            IOptions<CrmConfig> crmOptions,
            IMailSender mailSender, 
            ICrmInvoiceService crmInvoiceService)
        {
            this.solfacRepository = solfacRepository;
            this.invoiceRepository = invoiceRepository;
            this.solfacStatusFactory = solfacStatusFactory;
            this.crmConfig = crmOptions.Value;
            this.userRepository = userRepository;
            this.mailSender = mailSender;
            this.crmInvoiceService = crmInvoiceService;
        }

        public Response<Solfac> Add(Solfac solfac, IList<int> invoicesId)
        {
            var response = Validate(solfac);

            if (response.HasErrors()) return response;

            try
            {
                solfac.UpdatedDate = DateTime.Now;
                solfac.ModifiedByUserId = solfac.UserApplicantId;

                // Add History
                solfac.Histories.Add(GetHistory(SolfacStatus.None, solfac.Status, solfac.UserApplicantId,
                    string.Empty));

                // Insert Solfac
                solfacRepository.Insert(solfac);

                // Save
                solfacRepository.Save();

                // Update Invoice Status to Related
                foreach (var invoiceId in invoicesId)
                {
                    var invoiceToModif = new Invoice
                    {
                        Id = invoiceId,
                        SolfacId = solfac.Id,
                        InvoiceStatus = InvoiceStatus.Related
                    };
                    invoiceRepository.UpdateSolfacId(invoiceToModif);
                    invoiceRepository.UpdateStatus(invoiceToModif);
                }

                if (invoicesId.Any())
                {
                    // Save Invoices related
                    solfacRepository.Save();
                }

                response.Data = solfac;
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.SolfacCreated, MessageType.Success));

                UpdateHitos(solfac.Hitos, response);
            }
            catch (Exception e)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public IList<Solfac> Search(SolfacParams parameter, string userMail)
        {
            var isDirector = userRepository.HasDirectorGroup(userMail);

            if (isDirector)
            {
                return solfacRepository.SearchByParams(parameter);
            }
            else
            {
                return solfacRepository.SearchByParamsAndUser(parameter, userMail);
            }
        }

        public IList<Hito> GetHitosByProject(string projectId)
        {
            return solfacRepository.GetHitosByProject(projectId);
        }

        public IList<Solfac> GetByProject(string projectId)
        {
            return solfacRepository.GetByProject(projectId);
        }

        public Response<Solfac> GetById(int id)
        {
            var response = new Response<Solfac>();

            var solfac = solfacRepository.GetById(id);

            if (solfac == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.NotFound, MessageType.Error));
                return response;
            }

            response.Data = solfac;
            return response;
        }

        public Response ChangeStatus(int solfacId, SolfacStatusParams parameters, EmailConfig emailConfig)
        {
            var response = new Response<SolfacChangeStatusResponse>();

            var solfac = SolfacValidationHelper.ValidateIfExistAndGetWithUser(solfacId, solfacRepository, response);

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

                if (statusErrors.HasErrors())
                {
                    response.AddMessages(statusErrors.Messages);
                    return response;
                }

                // Update Status
                solfacStatusHandler.SaveStatus(solfac, parameters, solfacRepository);

                // Add history
                var history = GetHistory(solfac.Id, solfac.Status, parameters.Status, parameters.UserId,
                    parameters.Comment);
                solfacRepository.AddHistory(history);

                // Save
                solfacRepository.Save();
                response.Messages.Add(new Message(solfacStatusHandler.GetSuccessMessage(), MessageType.Success));

                // Update Hitos
                solfacStatusHandler.UpdateHitos(solfacRepository.GetHitosIdsBySolfacId(solfac.Id), solfac,
                    crmConfig.Url);
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            try
            {
                // Send Mail
                HandleSendMail(emailConfig, solfacStatusHandler, solfac);
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSendMail, MessageType.Warning));
            }

            return response;
        }

        public Response Delete(int id)
        {
            var response = new Response();

            var solfac = SolfacValidationHelper.ValidateIfExist(id, solfacRepository, response);

            if (response.HasErrors()) return response;

            try
            {
                var invoices = invoiceRepository.GetBySolfac(id);

                foreach (var invoice in invoices)
                {
                    invoice.InvoiceStatus = InvoiceStatus.Approved;
                    invoice.SolfacId = null;
                    invoiceRepository.UpdateStatus(invoice);
                    invoiceRepository.UpdateSolfacId(invoice);
                }

                solfacRepository.Delete(solfac);
                solfacRepository.Save();

                response.Messages.Add(new Message(Resources.es.Billing.Solfac.Deleted, MessageType.Success));
            }
            catch(Exception ex)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public ICollection<SolfacHistory> GetHistories(int id)
        {
            return solfacRepository.GetHistories(id);
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
                solfacRepository.Update(solfac);

                // Save changes
                solfacRepository.Save();

                // Update hitos in CRM
                UpdateHitos(solfac.Hitos, response);

                response.Messages.Add(new Message(Resources.es.Billing.Solfac.SolfacUpdated, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<SolfacAttachment> SaveFile(int solfacId, byte[] fileAsArrayBytes, string fileFileName)
        {
            var response = new Response<SolfacAttachment>();

            var solfac = solfacRepository.GetSingle(x => x.Id == solfacId);

            if (solfac == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.NotFound, MessageType.Error));
                return response;
            }

            var attachment = new SolfacAttachment
            {
                SolfacId = solfacId,
                Name = fileFileName,
                CreationDate = DateTime.Now,
                File = fileAsArrayBytes
            };

            solfacRepository.SaveAttachment(attachment);
            solfacRepository.Save();

            response.Data = attachment;
            response.Messages.Add(new Message(Resources.es.Billing.Solfac.FileAdded, MessageType.Success));

            return response;
        }

        public ICollection<SolfacAttachment> GetFiles(int solfacId)
        {
            return solfacRepository.GetFiles(solfacId);
        }

        public Response<SolfacAttachment> GetFileById(int fileId)
        {
            var response = new Response<SolfacAttachment>();

            var file = solfacRepository.GetFileById(fileId);

            if (file == null)
            {
                response.Messages.Add(new Message(Resources.es.Common.FileNotFound, MessageType.Error));
                return response;
            }

            response.Data = file;
            return response;
        }

        public Response DeleteFile(int id)
        {
            var response = new Response();

            var file = solfacRepository.GetFileById(id);

            if (file == null)
            {
                response.Messages.Add(new Message(Resources.es.Common.FileNotFound, MessageType.Error));
                return response;
            }

            try
            {
                solfacRepository.DeleteFile(file);
                solfacRepository.Save();
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.FileDeleted, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
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

            return response;
        }

        public Response DeleteDetail(int id)
        {
            var response = new Response();

            var detail = solfacRepository.GetDetail(id);

            if (detail == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.DetailNotFound, MessageType.Error));
                return response;
            }

            try
            {
                solfacRepository.DeleteDetail(detail);
                solfacRepository.Save();
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.DetailDeleted, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public async Task<Response> SplitHito(IList<HitoSplittedParams> hitos)
        {
            var response = ValidateHitoSplitted(hitos);

            if (response.HasErrors()) return response;

            var first = hitos.FirstOrDefault();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmConfig.Url);

                await UpdateFirstHito(response, first, client);
                await CreateNewHitos(response, hitos, client);

                if (!response.HasErrors())
                {
                    response.Messages.Add(new Message(Resources.es.Billing.Project.HitoSplitted, MessageType.Success));
                }
            }

            return response;
        }

        private async Task CreateNewHitos(Response response, IList<HitoSplittedParams> hitos, HttpClient client)
        {
            for (int i = 1; i < hitos.Count; i++)
            {
                var hito = hitos[i];

                try
                {
                    var data =
                        $"Ammount={hito.Ammount}&StatusCode={hito.StatusCode}&StartDate={hito.StartDate:O}&Name={hito.Name}&MoneyId={hito.MoneyId}" +
                        $"&Month={hito.Month}&ProjectId={hito.ProjectId}&OpportunityId={hito.OpportunityId}&ManagerId={hito.ManagerId}";

                    var stringContent = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var httpResponse = await client.PostAsync($"/api/InvoiceMilestone", stringContent);

                    httpResponse.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    response.Messages.Add(new Message(Resources.es.Billing.Solfac.ErrorSaveOnHitos, MessageType.Error));
                }
            }
        }

        private async Task UpdateFirstHito(Response response, HitoSplittedParams first, HttpClient client)
        {
            try
            {
                var data = $"Ammount={first.Ammount}&Name={first.Name}";

                var stringContent = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                var httpResponse =
                    await client.PutAsync($"/api/InvoiceMilestone/{first.ExternalHitoId}", stringContent);

                httpResponse.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.ErrorSaveOnHitos, MessageType.Error));
            }
        }

        private Response ValidateHitoSplitted(IList<HitoSplittedParams> hitos)
        {
            var response = new Response();

            HitoValidatorHelper.ValidateHitosQuantity(hitos, response);
            HitoValidatorHelper.ValidateAmmounts(hitos, response);
            HitoValidatorHelper.ValidateOpportunity(hitos, response);

            return response;
        }

        private void HandleSendMail(EmailConfig emailConfig, ISolfacStatusHandler solfacStatusHandler, Solfac solfac)
        {
            var subject = solfacStatusHandler.GetSubjectMail(solfac);
            var body = solfacStatusHandler.GetBodyMail(solfac, emailConfig.SiteUrl);
            var recipients = solfacStatusHandler.GetRecipients(solfac, emailConfig);

            mailSender.Send(recipients, subject, body);
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

        private async void UpdateHitos(ICollection<Hito> hitos, Response response)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmConfig.Url);

                foreach (var item in hitos)
                {
                    try
                    {
                        var sum = item.Details.Sum(x => x.Total);

                        if (sum != item.Total)
                        {
                            var total = $"Ammount={sum}";

                            var stringContent = new StringContent(total, Encoding.UTF8,
                                "application/x-www-form-urlencoded");
                            var httpResponse = await client.PutAsync($"/api/InvoiceMilestone/{item.ExternalHitoId}",
                                stringContent);

                            httpResponse.EnsureSuccessStatusCode();
                        }
                    }
                    catch (Exception)
                    {
                        response.Messages.Add(new Message(Resources.es.Billing.Solfac.ErrorSaveOnHitos,
                            MessageType.Warning));
                    }
                }
            }
        }

        public Response UpdateBill(int id, SolfacStatusParams parameters)
        {
            var response = new Response();

            var solfac = SolfacValidationHelper.ValidateIfExistAndGetWithUser(id, solfacRepository, response);

            if (response.HasErrors()) return response;

            SolfacValidationHelper.ValidateInvoiceCode(parameters, solfacRepository, response, solfac.InvoiceCode);
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
                solfacRepository.UpdateInvoice(solfacToModif);

                var history = GetHistory(solfac.Id, solfac.Status, solfac.Status, parameters.UserId, string.Empty);
                solfacRepository.AddHistory(history);

                solfacRepository.Save();
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.BillUpdated, MessageType.Success));
            }
            catch (Exception ex)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response UpdateCashedDate(int id, SolfacStatusParams parameters)
        {
            var response = new Response();

            var solfac = SolfacValidationHelper.ValidateIfExist(id, solfacRepository, response);

            if (response.HasErrors()) return response;

            SolfacValidationHelper.ValidateCasheDate(parameters, response, solfac);

            if (response.HasErrors()) return response;

            try
            {
                var solfacToModif = new Solfac {Id = solfac.Id, CashedDate = parameters.CashedDate};
                solfacRepository.UpdateCash(solfacToModif);

                var history = GetHistory(solfac.Id, solfac.Status, solfac.Status, parameters.UserId, string.Empty);
                solfacRepository.AddHistory(history);

                solfacRepository.Save();
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.CashUpdated, MessageType.Success));
            }
            catch (Exception ex)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response DeleteInvoice(int id, int invoiceId)
        {
            var response = new Response();

            var solfac = SolfacValidationHelper.ValidateIfExist(id, solfacRepository, response);

            if (response.HasErrors()) return response;

            try
            {
                var invoice = invoiceRepository.GetSingle(x => x.Id == invoiceId);

                if (invoice != null)
                {
                    invoice.InvoiceStatus = InvoiceStatus.Approved;
                    invoice.SolfacId = null;
                    invoiceRepository.UpdateStatus(invoice);
                    invoiceRepository.UpdateSolfacId(invoice);

                    invoiceRepository.Save();

                    response.Messages.Add(new Message(Resources.es.Billing.Solfac.InvoiceDeleted, MessageType.Success));
                }
                else
                {
                    response.Messages.Add(new Message(Resources.es.Billing.Invoice.NotFound, MessageType.Error));
                }
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<ICollection<Invoice>> GetInvoices(int id)
        {
            var response = new Response<ICollection<Invoice>>();

            var solfac = SolfacValidationHelper.ValidateIfExist(id, solfacRepository, response);

            if (response.HasErrors()) return response;

            response.Data = invoiceRepository.GetBySolfac(id);

            return response;
        }

        public Response AddInvoices(int id, IList<int> invoices)
        {
            var response = new Response();

            var solfac = SolfacValidationHelper.ValidateIfExist(id, solfacRepository, response);

            if (response.HasErrors()) return response;

            try
            {
                foreach (var invoiceToAdd in invoices)
                {
                    var invoice = invoiceRepository.GetSingle(x => x.Id == invoiceToAdd);

                    if (invoice != null)
                    {
                        invoice.InvoiceStatus = InvoiceStatus.Related;
                        invoice.SolfacId = id;
                        invoiceRepository.UpdateStatus(invoice);
                        invoiceRepository.UpdateSolfacId(invoice);
                    }
                    else
                    {
                        response.Messages.Add(new Message(Resources.es.Billing.Invoice.NotFound, MessageType.Warning));
                    }
                }

                invoiceRepository.Save();

                response.Messages.Add(new Message(Resources.es.Billing.Solfac.InvoicesAdded, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Solfac> Post(Solfac solfac, IList<int> invoicesId)
        {
            var validationResult = SolfacValidationHelper.ValidatePost(solfac, solfacRepository);

            if (validationResult.HasErrors())
                return validationResult;

            CreateDataOnCrm(solfac);

            var result = Add(solfac, invoicesId);

            if (result.HasErrors())
                return result;

            var solfacChangeStatusResponse = new SolfacChangeStatusResponse
            {
                HitoStatus = HitoStatus.Pending,
                Hitos = result.Data.Hitos.Select(x => x.ExternalHitoId).ToList()
            };

            ChangeHitoStatus(solfacChangeStatusResponse);

            return result;
        }

        private async void ChangeHitoStatus(SolfacChangeStatusResponse data)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmConfig.Url);

                foreach (var item in data.Hitos)
                {
                    try
                    {
                        var stringContent = new StringContent($"StatusCode={(int) data.HitoStatus}", Encoding.UTF8,
                            "application/x-www-form-urlencoded");

                        var response = await client.PutAsync($"/api/InvoiceMilestone/{item}", stringContent);

                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private void CreateDataOnCrm(Solfac solfac)
        {
            if (!IsCreditNote(solfac))
                return;

            var hito = solfac.Hitos.First();

            hito.SolfacId = 0;

            var hitoResult = crmInvoiceService.CreateHitoBySolfac(solfac);

            hito.ExternalHitoId = hitoResult.Data;
        }

        private static bool IsCreditNote(Solfac solfac)
        {
            return new[] { SolfacDocumentType.CreditNoteA, SolfacDocumentType.CreditNoteB }.Contains(solfac.DocumentTypeId);
        }
    }
}
