using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Services.Billing;
using Sofco.Core.StatusHandlers;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Utils;
using Sofco.Core.Logger;
using Sofco.Core.Models.Billing;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Domain.Helpers;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Relationships;
using System.Globalization;
using Sofco.Service.Crm.Interfaces;

namespace Sofco.Service.Implementations.Billing
{
    public class SolfacService : ISolfacService
    {
        private readonly ISolfacStatusFactory solfacStatusFactory;
        private readonly IUnitOfWork unitOfWork;
        private readonly CrmConfig crmConfig;
        private readonly ICrmInvoicingMilestoneService crmInvoiceService;
        private readonly ILogMailer<SolfacService> logger;
        private readonly IUserData userData;
         
        public SolfacService(ISolfacStatusFactory solfacStatusFactory,
            IUnitOfWork unitOfWork,
            IUserData userData,
            IOptions<CrmConfig> crmOptions,
            ICrmInvoicingMilestoneService crmInvoiceService, ILogMailer<SolfacService> logger)
        {
            this.solfacStatusFactory = solfacStatusFactory;
            crmConfig = crmOptions.Value;
            this.unitOfWork = unitOfWork;
            this.crmInvoiceService = crmInvoiceService;
            this.logger = logger;
            this.userData = userData;
        }

        public Response<Solfac> CreateSolfac(Solfac solfac, IList<int> invoicesId, IList<int> certificatesId)
        {
            var response = Validate(solfac);
             
            if (response.HasErrors()) return response;

            try
            {
                CreateHitoOnCrm(solfac, response);

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
                LoadInvoices(solfac, invoicesId);

                // Add Certificates
                LoadCertificates(solfac, certificatesId);

                if (invoicesId.Any() || certificatesId.Any())
                {
                    // Save Invoices or Certificates related
                    unitOfWork.Save();
                }

                response.Data = solfac;
                response.AddSuccess(Resources.Billing.Solfac.SolfacCreated);

                if (SolfacHelper.IsCreditNote(solfac) || SolfacHelper.IsDebitNote(solfac))
                    return response;

                //var crmResult = crmInvoiceService.UpdateHitos(solfac.Hitos);

                //response.AddMessages(crmResult.Messages);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void LoadInvoices(Solfac solfac, IList<int> invoicesId)
        {
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
        }

        private void LoadCertificates(Solfac solfac, IList<int> certificatesId)
        {
            foreach (var certificateId in certificatesId)
            {
                var solfacCertificate = new SolfacCertificate
                {
                    CertificateId = certificateId,
                    SolfacId = solfac.Id
                };

                unitOfWork.CertificateRepository.RelateToSolfac(solfacCertificate);
            }
        }

        public IList<Solfac> Search(SolfacParams parameter)
        {
            var user = userData.GetCurrentUser();
            var isDirector = unitOfWork.UserRepository.HasDirectorGroup(user.Email);
            var isDaf = unitOfWork.UserRepository.HasDafGroup(user.Email);
            var isCdg = unitOfWork.UserRepository.HasCdgGroup(user.Email);
            var isComercial = unitOfWork.UserRepository.HasCdgGroup(user.Email);

            if (isDirector || isDaf || isCdg || isComercial)
            {
                return unitOfWork.SolfacRepository.SearchByParams(parameter);
            }

            return unitOfWork.SolfacRepository.SearchByParamsAndUser(parameter, user);
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
                response.AddError(Resources.Billing.Solfac.NotFound);
                return response;
            }

            if (string.IsNullOrWhiteSpace(solfac.OpportunityNumber))
            {
                var project = unitOfWork.ProjectRepository.GetByIdCrm(solfac.ProjectId);

                if (project != null)
                {
                    solfac.OpportunityNumber = project.OpportunityNumber;
                }
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
                response.AddSuccess(solfacStatusHandler.GetSuccessMessage());

                // Update Hitos
                solfacStatusHandler.UpdateHitos(unitOfWork.SolfacRepository.GetHitosIdsBySolfacId(solfac.Id), solfac);
            }
            catch(Exception ex)
            {
                response = new Response();
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(ex);
                return response;
            }

            try
            {
                // Send Mail
                solfacStatusHandler.SendMail(solfac, emailConfig);
            }
            catch
            {
                response.AddWarning(Resources.Common.ErrorSendMail);
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

                var hitos = unitOfWork.SolfacRepository.GetHitosBySolfacId(solfac.Id);

                //crmInvoiceService.UpdateHitoStatus(hitos.ToList(), HitoStatus.Pending);

                unitOfWork.SolfacRepository.Delete(solfac);
                unitOfWork.Save();

                response.AddSuccess(Resources.Billing.Solfac.Deleted);
            }
            catch (Exception ex)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(ex);
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
                //var crmResult = crmInvoiceService.UpdateHitos(solfac.Hitos);

                //response.AddMessages(crmResult.Messages);

                response.AddSuccess(Resources.Billing.Solfac.SolfacUpdated);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<SolfacAttachment> SaveFile(int solfacId, byte[] fileAsArrayBytes, string fileFileName)
        {
            var response = new Response<SolfacAttachment>();

            var solfac = unitOfWork.SolfacRepository.GetSingle(x => x.Id == solfacId);

            if (solfac == null)
            {
                response.AddError(Resources.Billing.Solfac.NotFound);
                return response;
            }

            var attachment = new SolfacAttachment
            {
                SolfacId = solfacId,
                Name = fileFileName,
                CreationDate = DateTime.Now,
                File = fileAsArrayBytes
            };

            try
            {
                unitOfWork.SolfacRepository.SaveAttachment(attachment);
                unitOfWork.Save();

                response.Data = attachment;
                response.AddSuccess(Resources.Billing.Solfac.FileAdded);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

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
                response.AddError(Resources.Common.FileNotFound);
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
                response.AddError(Resources.Common.FileNotFound);
                return response;
            }

            try
            {
                unitOfWork.SolfacRepository.DeleteFile(file);
                unitOfWork.Save();
                response.AddSuccess(Resources.Billing.Solfac.FileDeleted);
            }
            catch(Exception ex)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(ex);
            }

            return response;
        }
         
        public Response<Solfac> Validate(Solfac solfac)
        {
            var response = new Response<Solfac>();

            SolfacValidationHelper.ValidateProvincePercentage(solfac, response);
            SolfacValidationHelper.ValidateDetails(solfac.Hitos, response);
            SolfacValidationHelper.ValidateHitos(solfac.Hitos, response);
            SolfacValidationHelper.ValidatePercentage(solfac, response);
            SolfacValidationHelper.ValidateTimeLimit(solfac, response);
            SolfacValidationHelper.ValidateContractNumber(solfac, response, unitOfWork);
            SolfacValidationHelper.ValidateImputationNumber(solfac, response);
            SolfacValidationHelper.ValidateBusinessName(solfac, response);

            if (SolfacHelper.IsCreditNote(solfac))
            {
                SolfacValidationHelper.ValidateCreditNote(solfac, unitOfWork.SolfacRepository, response);
            }

            if (SolfacHelper.IsDebitNote(solfac) || SolfacHelper.IsCreditNote(solfac))
            {
                var hito = solfac.Hitos.First();
                HitoValidatorHelper.ValidateOpportunity(new HitoParameters { OpportunityId = hito.OpportunityId }, response);
            }

            return response;
        }

        public Response DeleteDetail(int id)
        {
            var response = new Response();

            var detail = unitOfWork.SolfacRepository.GetDetail(id);

            if (detail == null)
            {
                response.AddError(Resources.Billing.Solfac.DetailNotFound);
                return response;
            }

            try
            {
                unitOfWork.SolfacRepository.DeleteDetail(detail);
                unitOfWork.Save();
                response.AddSuccess(Resources.Billing.Solfac.DetailDeleted);
            }
            catch(Exception ex)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(ex);
            }

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
                response.AddSuccess(Resources.Billing.Solfac.BillUpdated);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.ErrorSave);
            }

            //try
            //{
            //    crmInvoiceService.UpdateHitoInvoice(unitOfWork.SolfacRepository.GetHitosBySolfacId(solfac.Id), parameters);
            //}
            //catch (Exception ex)
            //{
            //    logger.LogError(ex);
            //    response.AddError(Resources.Billing.Solfac.ErrorSaveOnHitos);
            //}

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
                response.AddSuccess(Resources.Billing.Solfac.CashUpdated);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.ErrorSave);
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

                    response.AddSuccess(Resources.Billing.Solfac.InvoiceDeleted);
                }
                else
                {
                    response.AddError(Resources.Billing.Invoice.NotFound);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<ICollection<InvoiceFileOptions>> GetInvoices(int id)
        {
            var response = new Response<ICollection<InvoiceFileOptions>>();

            SolfacValidationHelper.ValidateIfExist(id, unitOfWork.SolfacRepository, response);

            if (response.HasErrors()) return response;

            response.Data = unitOfWork.InvoiceRepository.GetBySolfac(id).Select(x => new InvoiceFileOptions(x)).ToList();

            return response;
        }

        public Response<List<Invoice>> AddInvoices(int id, IList<int> invoices)
        {
            var response = new Response<List<Invoice>> { Data = new List<Invoice>() };

            SolfacValidationHelper.ValidateIfExist(id, unitOfWork.SolfacRepository, response);

            if (response.HasErrors()) return response;

            try
            {
                foreach (var invoiceToAdd in invoices)
                {
                    var invoice = unitOfWork.InvoiceRepository.GetById(invoiceToAdd);

                    if (invoice != null)
                    {
                        invoice.InvoiceStatus = InvoiceStatus.Related;
                        invoice.SolfacId = id;
                        unitOfWork.InvoiceRepository.UpdateStatus(invoice);
                        unitOfWork.InvoiceRepository.UpdateSolfacId(invoice);

                        response.Data.Add(new Invoice { Id = invoice.Id,
                            InvoiceNumber = invoice.InvoiceNumber,
                            PDfFileData = new File
                            {
                                FileName = invoice.PDfFileData?.FileName
                            }});
                    }
                    else
                    {
                        response.AddWarning(Resources.Billing.Invoice.NotFound);
                    }
                }

                unitOfWork.Save();

                response.AddSuccess(Resources.Billing.Solfac.InvoicesAdded);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<Solfac> Post(Solfac solfac, IList<int> invoicesId, IList<int> certificatesId)
        {
            var result = CreateSolfac(solfac, invoicesId, certificatesId);

            return result;
        }

        public Response DeleteSolfacCertificate(int id, int certificateId)
        {
            var response = new Response();

            SolfacValidationHelper.ValidateIfExist(id, unitOfWork.SolfacRepository, response);
            CertificateValidationHandler.Exist(response, certificateId, unitOfWork);

            if (response.HasErrors()) return response;

            try
            {
                var solfacCertificate = new SolfacCertificate {SolfacId = id, CertificateId = certificateId};
                unitOfWork.SolfacCertificateRepository.Delete(solfacCertificate);
                unitOfWork.Save();

                response.AddSuccess(Resources.Billing.Certificate.SolfacCertificateDeleted);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<IList<Certificate>> AddCertificates(int id, IList<int> certificates)
        {
            var response = new Response<IList<Certificate>> { Data = new List<Certificate>() };

            SolfacValidationHelper.ValidateIfExist(id, unitOfWork.SolfacRepository, response);

            if (response.HasErrors()) return response;

            try
            {
                foreach (var certificateId in certificates)
                {
                    var certificate = unitOfWork.CertificateRepository.GetById(certificateId);

                    if (certificate != null)
                    {
                        if (!unitOfWork.SolfacCertificateRepository.Exist(id, certificateId))
                        {
                            var solfacCertificate = new SolfacCertificate { SolfacId = id, CertificateId = certificateId };
                            unitOfWork.SolfacCertificateRepository.Insert(solfacCertificate);

                            response.Data.Add(certificate);
                        }
                    }
                    else
                    {
                        response.AddWarning(Resources.Billing.Certificate.NotFound);
                    }
                }

                unitOfWork.Save();

                response.AddSuccess(Resources.Billing.Certificate.SolfacCertificateRelated);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void CreateHitoOnCrm(Solfac solfac, Response response)
        {
            if (!SolfacHelper.IsCreditNote(solfac) && !SolfacHelper.IsDebitNote(solfac))
                return;

            var hito = solfac.Hitos.First();

            hito.SolfacId = 0;

            hito.Total = hito.Details.Sum(s => s.Total);

            var hitoParams = new HitoParameters();

            hitoParams.Ammount = SolfacHelper.IsCreditNote(solfac) ? (-1)*hito.Total : hito.Total;
            hitoParams.Name = hito.Description = GetPrefixTitle(solfac) + hito.Description;
            hitoParams.StatusCode = Convert.ToInt32(HitoStatus.Pending).ToString();
            hitoParams.StartDate = DateTime.UtcNow;
            hitoParams.Month = hito.Month;
            hitoParams.ProjectId = hito.ExternalProjectId;
            hitoParams.OpportunityId = hito.OpportunityId;
            hitoParams.MoneyId = hito.CurrencyId;

            var hitoId = crmInvoiceService.Create(hitoParams, response);

            if (!response.HasErrors())
            {
                hito.ExternalHitoId = hitoId;
            }
            else
            {
                response.AddError(Resources.Billing.Solfac.ErrorSaveOnHitos);
            }
        }

        private string GetPrefixTitle(Solfac solfac)
        {
            if (SolfacHelper.IsCreditNote(solfac))
                return Resources.Billing.Invoice.CrmPrefixCreditNoteTitle;

            if (SolfacHelper.IsDebitNote(solfac))
                return Resources.Billing.Invoice.CrmPrefixDebitNoteTitle;

            return string.Empty;
        }
    }
}
