using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Billing;
using Sofco.Core.StatusHandlers;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Utils;
using Sofco.Core.Mail;
using Sofco.Core.Managers;
using Sofco.Core.Models.Billing;
using Sofco.Framework.MailData;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Resources.Mails;
using File = Sofco.Domain.Models.Common.File;
using Sofco.Core.Managers;
using Sofco.Core.Services.Common;

namespace Sofco.Service.Implementations.Billing
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IInvoiceStatusFactory invoiceStatusFactory;
        private readonly IMailSender mailSender;
        private readonly ISessionManager sessionManager;
        private readonly EmailConfig emailConfig;
        private readonly ILogMailer<InvoiceService> logger;
        private readonly IMailBuilder mailBuilder;
        private readonly FileConfig fileConfig;
        private readonly IFileService fileService;
        private readonly IRoleManager roleManager;

        public InvoiceService(IUnitOfWork unitOfWork,
            IInvoiceStatusFactory invoiceStatusFactory,
            IOptions<EmailConfig> emailOptions,
            IMailBuilder mailBuilder,
            IRoleManager roleManager,
            IFileService fileService,
            ILogMailer<InvoiceService> logger,
            IMailSender mailSender, ISessionManager sessionManager, IOptions<FileConfig> fileOptions)
        {
            this.unitOfWork = unitOfWork;
            this.invoiceStatusFactory = invoiceStatusFactory;
            this.mailSender = mailSender;
            this.sessionManager = sessionManager;
            this.emailConfig = emailOptions.Value;
            this.logger = logger;
            this.roleManager = roleManager;
            this.mailBuilder = mailBuilder;
            fileConfig = fileOptions.Value;
            this.fileService = fileService;
        }

        public IList<Invoice> GetByProject(string projectId)
        {
            return unitOfWork.InvoiceRepository.GetByProject(projectId);
        }

        public Response<Invoice> GetById(int id)
        {
            var response = new Response<Invoice>();

            var invoce = unitOfWork.InvoiceRepository.GetById(id);

            if (invoce == null)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            response.Data = invoce;

            return response;
        }

        public Response<Invoice> Add(Invoice invoice, string identityName)
        {
            var response = new Response<Invoice>();

            InvoiceValidationHelper.ValidateCuit(response, invoice);
            InvoiceValidationHelper.ValidateAddress(response, invoice);
            InvoiceValidationHelper.ValidateZipCode(response, invoice);
            InvoiceValidationHelper.ValidateCity(response, invoice);
            InvoiceValidationHelper.ValidateProvince(response, invoice);
            InvoiceValidationHelper.ValidateCountry(response, invoice);

            if (response.HasErrors()) return response;

            try
            {
                var userName = identityName.Split('@')[0];
                var user = unitOfWork.UserRepository.GetSingle(x => x.UserName == userName);

                if (user == null)
                {
                    response.Messages.Add(new Message(Resources.Admin.User.NotFound, MessageType.Error));
                    return response;
                }

                invoice.InvoiceStatus = InvoiceStatus.SendPending;
                invoice.InvoiceNumber = "0000-00000000";
                invoice.UserId = user.Id;

                // Add History
                invoice.Histories.Add(GetHistory(InvoiceStatus.None, invoice.InvoiceStatus, invoice.UserId, string.Empty));

                // Insert Solfac
                unitOfWork.InvoiceRepository.Insert(invoice);

                // Save
                unitOfWork.Save();

                response.Data = invoice;
                response.Messages.Add(new Message(Resources.Billing.Invoice.InvoiceCreated, MessageType.Success));
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response ChangeStatus(int invoiceId, InvoiceStatus status, EmailConfig emailConfig, InvoiceStatusParams parameters)
        {
            var response = new Response();

            var invoice = unitOfWork.InvoiceRepository.GetById(invoiceId);

            if (invoice == null)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            var invoiceStatusHandler = invoiceStatusFactory.GetInstance(status);

            try
            {
                // Validate status
                var statusErrors = invoiceStatusHandler.Validate(invoice, parameters);

                if (statusErrors.HasErrors())
                {
                    response.AddMessages(statusErrors.Messages);
                    return response;
                }

                // Update Status
                invoiceStatusHandler.SaveStatus(invoice, parameters);

                // Add history
                var history = GetHistory(invoiceId, invoice.InvoiceStatus, status, parameters.UserId, parameters.Comment);
                unitOfWork.InvoiceRepository.AddHistory(history);

                // Save
                unitOfWork.Save();
                response.Messages.Add(new Message(invoiceStatusHandler.GetSuccessMessage(), MessageType.Success));
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            try
            {
                // Send Mail
                invoiceStatusHandler.SendMail(mailSender, invoice, emailConfig);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.Messages.Add(new Message(Resources.Common.ErrorSendMail, MessageType.Error));
            }

            return response;
        }

        public IList<Invoice> GetOptions(string projectId)
        {
            return unitOfWork.InvoiceRepository.GetOptions(projectId);
        }

        public Response Delete(int id)
        {
            var response = new Response();

            var invoice = unitOfWork.InvoiceRepository.GetById(id);

            if (invoice == null)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            if (invoice.InvoiceStatus != InvoiceStatus.SendPending && invoice.InvoiceStatus != InvoiceStatus.Rejected)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.CannotDelete, MessageType.Error));
                return response;
            }

            try
            {
                unitOfWork.InvoiceRepository.Delete(invoice);
                unitOfWork.Save();

                response.Messages.Add(new Message(Resources.Billing.Invoice.Deleted, MessageType.Success));
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public ICollection<Invoice> Search(InvoiceParams parameters)
        {
            var userMail = sessionManager.GetUserEmail();
            var hasAllAccess = roleManager.HasFullAccess();

            if (hasAllAccess)
            {
                return unitOfWork.InvoiceRepository.SearchByParams(parameters);
            }

            return unitOfWork.InvoiceRepository.SearchByParamsAndUser(parameters, userMail);
        }

        public Response<Invoice> Clone(int id)
        {
            var response = new Response<Invoice>();

            var invoice = unitOfWork.InvoiceRepository.GetSingle(x => x.Id == id);

            if (invoice == null)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            var invoiceToClone = invoice.Clone();

            try
            {
                unitOfWork.InvoiceRepository.Insert(invoiceToClone);
                unitOfWork.Save();
                response.Data = invoiceToClone;
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public ICollection<InvoiceHistory> GetHistories(int id)
        {
            return unitOfWork.InvoiceRepository.GetHistories(id);
        }

        public Response RequestAnnulment(InvoiceAnnulmentModel model)
        {
            var response = new Response();

            var invoices = unitOfWork.InvoiceRepository.GetByIds(model.Invoices);

            if (!invoices.Any()) return response;

            try
            {
                foreach (var invoice in invoices)
                {
                    invoice.InvoiceStatus = InvoiceStatus.RequestAnnulment;
                    unitOfWork.InvoiceRepository.UpdateStatus(invoice);
                }

                unitOfWork.Save();

                var invoicesToListString = invoices.Select(x => $"<a href='{emailConfig.SiteUrl}billing/invoice/{x.Id}/project/{x.ProjectId}' target='_blank'>{x.ExcelFileData?.FileName}</a>");

                var subject = MailSubjectResource.InvoiceRequestAnnulment;
                var body = string.Format(MailMessageResource.InvoiceRequestAnnulment, string.Join("</br>", invoicesToListString), model.Comments);

                var mailDaf = unitOfWork.GroupRepository.GetEmail(emailConfig.DafCode);

                var data = new InvoiceRequestAnnulmentData
                {
                    Title = subject,
                    Message = body,
                    Recipient = mailDaf
                };

                var email = mailBuilder.GetEmail(data);

                mailSender.Send(email);

                response.AddSuccess(Resources.Billing.Invoice.RequestAnnulmentSent);
            }
            catch (Exception ex)
            {
                response.AddError(Resources.Common.ErrorSendMail);
                logger.LogError(ex);
            }

            return response;
        }

        private InvoiceHistory GetHistory(int invoiceId, InvoiceStatus statusFrom, InvoiceStatus statusTo, int userId, string comment)
        {
            var history = GetHistory(statusFrom, statusTo, userId, comment);
            history.InvoiceId = invoiceId;
            return history;
        }

        private InvoiceHistory GetHistory(InvoiceStatus statusFrom, InvoiceStatus statusTo, int userId, string comment)
        {
            var history = new InvoiceHistory
            {
                StatusFrom = statusFrom,
                StatusTo = statusTo,
                UserId = userId,
                Comment = comment,
                CreatedDate = DateTime.Now
            };

            return history;
        }

        public async Task<Response<File>> AttachFile(int invoiceId, Response<File> response, IFormFile file, string userName)
        {
            var invoice = unitOfWork.InvoiceRepository.GetSingle(x => x.Id == invoiceId);

            if (response.HasErrors()) return response;

            var fileToAdd = new File();
            var lastDotIndex = file.FileName.LastIndexOf('.');

            fileToAdd.FileName = file.FileName;
            fileToAdd.FileType = file.FileName.Substring(lastDotIndex);
            fileToAdd.InternalFileName = Guid.NewGuid();
            fileToAdd.CreationDate = DateTime.UtcNow;
            fileToAdd.CreatedUser = userName;

            var path = string.Empty;
            var successMsg = string.Empty;

            if (fileToAdd.FileType.Equals(".pdf"))
            {
                invoice.PDfFileData = fileToAdd;
                path = fileConfig.InvoicesPdfPath;
                successMsg = Resources.Billing.Invoice.PdfUpload;
            }
            else if (fileToAdd.FileType.Equals(".xlsx") || fileToAdd.FileType.Equals(".xls"))
            {
                invoice.ExcelFileData = fileToAdd;
                path = fileConfig.InvoicesExcelPath;
                successMsg = Resources.Billing.Invoice.ExcelUpload;
            }

            if (string.IsNullOrWhiteSpace(path)) return response;

            try
            {
                var fileName = $"{fileToAdd.InternalFileName.ToString()}{fileToAdd.FileType}";

                using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                unitOfWork.FileRepository.Insert(fileToAdd);
                unitOfWork.InvoiceRepository.Update(invoice);
                unitOfWork.Save();

                response.Data = fileToAdd;
                response.AddSuccess(successMsg);
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.SaveFileError);
                logger.LogError(e);
            }

            return response;
        }

        public Response<Stream> GetZip(IList<int> ids)
        {
            var response = new Response<Stream>();

            if (ids.Count > 0)
            {
                var zipStream = new MemoryStream();

                try
                {
                    using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var id in ids)
                        {
                            var responseFile = fileService.GetFile(id, fileConfig.InvoicesPdfPath);

                            if (!responseFile.HasErrors())
                            {
                                CopyStream(zip, responseFile);
                            }
                            else
                            {
                                responseFile = fileService.GetFile(id, fileConfig.InvoicesExcelPath);

                                if (!responseFile.HasErrors())
                                {
                                    CopyStream(zip, responseFile);
                                }
                            }
                        }
                    }

                    zipStream.Position = 0;

                    response.Data = zipStream;
                }
                catch (Exception e)
                {
                    logger.LogError(e);
                    response.AddError(Resources.Common.ExportFileError);
                }
            }

            return response;
        }

        private void CopyStream(ZipArchive zip, Response<Tuple<byte[], string>> responseFile)
        {
            var entry = zip.CreateEntry(responseFile.Data.Item2);

            using (var entryStream = entry.Open())
            {
                var memoryStream = new MemoryStream(responseFile.Data.Item1);
                memoryStream.CopyTo(entryStream);
            }
        }
    }
}
