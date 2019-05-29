using Microsoft.AspNetCore.Http;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.Services.Rrhh;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.Mail;
using Sofco.Core.Services.ManagementReport;
using Sofco.Domain.Enums;
using Sofco.Framework.Helpers;
using Sofco.Framework.MailData;

namespace Sofco.Service.Implementations.Rrhh
{
    public class PrepaidService : IPrepaidService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<PrepaidService> logger;
        private readonly IPrepaidFactory prepaidFactory;
        private readonly IMailSender mailSender;
        private readonly EmailConfig emailConfig;
        private readonly IManagementReportCalculateCostsService managementReportCalculateCostsService;

        public PrepaidService(IUnitOfWork unitOfWork, 
            ILogMailer<PrepaidService> logger, 
            IPrepaidFactory prepaidFactory,             
            IOptions<EmailConfig> emailOptions,
            IManagementReportCalculateCostsService managementReportCalculateCostsService,
            IMailSender mailSender)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.prepaidFactory = prepaidFactory;
            this.mailSender = mailSender;
            this.emailConfig = emailOptions.Value;
            this.managementReportCalculateCostsService = managementReportCalculateCostsService;
        }

        public Response<PrepaidDashboard> Import(int prepaidId, int yearId, int monthId, IFormFile file)
        {
            var response = new Response<PrepaidDashboard>();

            var prepaid = unitOfWork.UtilsRepository.GetPrepaid(prepaidId);

            if (prepaid == null)
                response.AddError(Resources.Rrhh.Prepaid.NotFound);
             
            if (monthId < 1 || monthId > 12)
                response.AddError(Resources.Rrhh.Prepaid.MonthError);

            var today = DateTime.UtcNow;

            if (yearId < today.AddYears(-1).Year || yearId > today.Year)
                response.AddError(Resources.Rrhh.Prepaid.YearError);

            if (response.HasErrors()) return response;

            if (unitOfWork.PrepaidImportedDataRepository.DateIsClosed(prepaidId, yearId, monthId))
            {
                response.AddError(Resources.Rrhh.Prepaid.DateClosed);
                return response;
            }

            try
            {
                var fileManager = prepaidFactory.GetInstance(prepaid?.Code);

                if (fileManager == null)
                {
                    response.AddError(Resources.Rrhh.Prepaid.NotImplemented);
                    return response;
                }

                unitOfWork.PrepaidImportedDataRepository.DeleteByDateAndPrepaid(prepaid.Id, new DateTime(yearId, monthId, 1));

                response = fileManager.Process(yearId, monthId, file, prepaid);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.SaveFileError);
            }

            return response;
        }

        public Response<IList<PrepaidDashboard>> GetDashboard(int yearId, int monthId)
        {
            var response = new Response<IList<PrepaidDashboard>>();

            response.Data = unitOfWork.PrepaidImportedDataRepository.GetDashboard(yearId, monthId);

            return response;
        }

        public Response<IList<PrepaidImportedData>> Get(int yearId, int monthId)
        {
            var response = new Response<IList<PrepaidImportedData>>();

            response.Data = unitOfWork.PrepaidImportedDataRepository.GetByDate(yearId, monthId);

            return response;
        }

        public Response Update(PrepaidImportedDataUpdateModel model)
        {
            var response = new Response();

            if (model.Ids == null || !model.Ids.Any())
            {
                response.AddError(Resources.Rrhh.Prepaid.NoItemsSelected);
            }

            if (!model.Status.HasValue)
            {
                response.AddError(Resources.Rrhh.Prepaid.StatusEmpty);
            }

            try
            {
                var data = unitOfWork.PrepaidImportedDataRepository.GetByIds(model.Ids);

                foreach (var prepaidImportedData in data)
                {
                    prepaidImportedData.Status = model.Status.GetValueOrDefault();
                    unitOfWork.PrepaidImportedDataRepository.UpdateStatus(prepaidImportedData);
                }

                if (data.Any())
                {
                    unitOfWork.Save();
                    response.AddSuccess(Resources.Rrhh.Prepaid.UpdateSuccess);
                }
                else
                    response.AddWarning(Resources.Rrhh.Prepaid.NoDataUpdate);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Close(int yearId, int monthId)
        {
            var response = new Response();

            var list = unitOfWork.PrepaidImportedDataRepository.GetByDate(yearId, monthId);

            if (list.Any(x => x.Status == PrepaidImportedDataStatus.Error))
            {
                response.AddError(Resources.Rrhh.Prepaid.CannotClose);
                return response;
            }

            try
            {
                foreach (var prepaidImportedData in list)
                {
                    prepaidImportedData.Closed = true;
                    unitOfWork.PrepaidImportedDataRepository.Close(prepaidImportedData);
                }

                unitOfWork.Save();
                response.AddSuccess(Resources.Rrhh.Prepaid.CloseSuccess);

                SendMailToDaf(response, yearId, monthId);

                managementReportCalculateCostsService.UpdateManagementReports(response, yearId, monthId);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response InformToRrhhPrepaidsImported(int yearId, int monthId)
        {
            var response = new Response();

            try
            {
                var rrhhGroup = unitOfWork.GroupRepository.GetByCode(emailConfig.RrhhCode);

                var data = new MailDefaultData()
                {
                    Title = Resources.Mails.MailSubjectResource.PrepaidImported,
                    Message = string.Format(Resources.Mails.MailMessageResource.PrepaidImported, DatesHelper.GetDateDescription(new DateTime(yearId, monthId, 1))),
                    Recipients = new List<string> { rrhhGroup.Email }
                };

                mailSender.Send(data);
                response.AddSuccess(Resources.Common.MailSent);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSendMail);
            }

            return response;
        }

        private void SendMailToDaf(Response response, int yearId, int monthId)
        {
            try
            {
                var dafGroup = unitOfWork.GroupRepository.GetByCode(emailConfig.DafCode);

                var data = new MailDefaultData()
                {
                    Title = Resources.Mails.MailSubjectResource.PrepaidClosed,
                    Message = string.Format(Resources.Mails.MailMessageResource.PrepaidClosed, DatesHelper.GetDateDescription(new DateTime(yearId, monthId, 1))),
                    Recipients = new List<string> { dafGroup.Email }
                };

                mailSender.Send(data);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSendMail);
            }
        }
    }
}
