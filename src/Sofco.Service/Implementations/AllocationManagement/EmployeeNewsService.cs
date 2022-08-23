using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Framework.MailData;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class EmployeeNewsService : IEmployeeNewsService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<EmployeeNewsService> logger;
        private readonly IMailSender mailSender;
        private readonly EmailConfig emailConfig;
        private readonly IMapper mapper;
        private readonly IMailBuilder mailBuilder;
        private readonly ISessionManager sessionManager;

        public EmployeeNewsService(IUnitOfWork unitOfWork, 
            ILogMailer<EmployeeNewsService> logger, 
            IMailSender mailSender, 
            IOptions<EmailConfig> emailOptions,
            IMapper mapper, IMailBuilder mailBuilder, ISessionManager sessionManager)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mailSender = mailSender;
            emailConfig = emailOptions.Value;
            this.mapper = mapper;
            this.mailBuilder = mailBuilder;
            this.sessionManager = sessionManager;
        }

        public Response<IList<EmployeeNewsModel>> GetEmployeeNews()
        {
            var employeeSyncActions = unitOfWork.EmployeeSyncActionRepository.GetAll();

            var data = Translate(employeeSyncActions.ToList());

            MatchReincorporation(data);

            FillEndReason(employeeSyncActions, data);

            var response = new Response<IList<EmployeeNewsModel>> { Data = data };

            return response;
        }

        private void FillEndReason(IList<EmployeeSyncAction> employeeSyncActions, List<EmployeeNewsModel> data)
        {
            var employeesInGaps = unitOfWork.EmployeeRepository.GetByEmployeeNumbers(employeeSyncActions.Select(x => x.EmployeeNumber));

            foreach (var employeeNewsModel in data.Where(x => x.Status == "Delete"))
            {
                employeeNewsModel.EndReason = employeesInGaps
                    .SingleOrDefault(x => x.EmployeeNumber.Equals(employeeNewsModel.EmployeeNumber))
                    ?.EndReason;
            }
        }

        private void MatchReincorporation(List<EmployeeNewsModel> data)
        {
            var employeeNumbers = data
                .Where(s => s.Status == EmployeeSyncActionStatus.New)
                .Select(s => s.EmployeeNumber).ToArray();

            var storedNewEmployee = unitOfWork.EmployeeRepository.GetByEmployeeNumber(employeeNumbers);

            foreach (var employee in storedNewEmployee)
            {
                var employeeNews = data.First(s => s.EmployeeNumber == employee.EmployeeNumber);

                employeeNews.IsReincorporation = true;
            }
        }

        public Response<EmployeeSyncAction> Add(int newsId)
        {
            var userName = sessionManager.GetUserName();

            var response = new Response<EmployeeSyncAction>();

            EmployeeSyncActionValidationHelper.Exist(newsId, response, unitOfWork);
            EmployeeSyncActionValidationHelper.ValidateNewStatus(response);

            if (response.HasErrors()) return response;

            try
            {
                var employee = JsonConvert.DeserializeObject<Employee>(response.Data.EmployeeData);

                if (string.IsNullOrWhiteSpace(employee.Email))
                {
                    response.AddErrorAndNoTraslate($"Verificar el email del recurso {employee.Name} y volver a 'Actualizar datos' para confirmar la novedad");
                    response.Data = null;
                    return response;
                }

                if (!employee.Email.Contains("@sofrecom.com.ar"))
                {
                    response.AddErrorAndNoTraslate($"Verificar el email del recurso {employee.Name} y volver a 'Actualizar datos' para confirmar la novedad");
                    response.Data = null;
                    return response;
                } 

                employee.Created = DateTime.UtcNow;
                employee.Modified = DateTime.UtcNow;
                employee.CreatedByUser = userName;
                employee.BusinessHours = 8;
                employee.OnTestPeriod = true;
                employee.BusinessHoursDescription = "09:00 hs a 18:00 hs";

                if (employee.StartDate.Month == 10 || employee.StartDate.Month == 11 || employee.StartDate.Month == 12)
                {
                    CalculateDaysAfterSeptember30(employee);
                }
          
                SetEmployeeHistory(employee);

                // Add new/existing employee
                unitOfWork.EmployeeRepository.Save(employee);

                // Delete news
                unitOfWork.EmployeeSyncActionRepository.Delete(response.Data);

                // Save all changes
                unitOfWork.Save();

                response.AddSuccess(Resources.AllocationManagement.Employee.Added);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void CalculateDaysAfterSeptember30(Employee employee)
        {
            var daysWorked = new DateTime(DateTime.UtcNow.Year, 12, 31).Subtract(employee.StartDate.Date).Days + 1;

            var daysAvg = (double)daysWorked / 20;

            if (daysAvg % 1 >= 0.5)
            {
                daysAvg++;
            }

            var days = (int)daysAvg;

            if (days > 6)
            {
                employee.HolidaysPendingByLaw = days;
                days -= 2;
            }
            else if (days == 6)
            {
                employee.HolidaysPendingByLaw = days;
                days--;
            }

            employee.HolidaysByLaw = days;
            employee.HolidaysPending = days;
        }

        private void SetEmployeeHistory(Employee employee)
        {
            var storedEmployee =
                unitOfWork.EmployeeRepository.GetSingle(s => s.EmployeeNumber == employee.EmployeeNumber);

            if (storedEmployee == null) return;

            var employeeHistory = Translate(storedEmployee);

            unitOfWork.EmployeeHistoryRepository.Save(employeeHistory);
        }

        public Response<EmployeeSyncAction> Delete(int newsId, NewsDeleteModel model)
        {
            var userName = sessionManager.GetUserName();

            var response = new Response<EmployeeSyncAction>();

            EmployeeSyncActionValidationHelper.Exist(newsId, response, unitOfWork);
            EmployeeSyncActionValidationHelper.ValidateDeleteStatus(response);
            EmployeeSyncActionValidationHelper.ValidateEndReasonType(response, model);

            var employeeToChange = unitOfWork.EmployeeRepository.GetByEmployeeNumber(response.Data.EmployeeNumber);

            if (employeeToChange == null)
            {
                response.AddError(Resources.AllocationManagement.Employee.NotFound);

                return response;
            }

            if (unitOfWork.LicenseRepository.HasAfterDateByEmployeeCancelledOrRejected(employeeToChange.Id,
                response.Data.EndDate.GetValueOrDefault()))
            {
                response.AddErrorAndNoTraslate("El recurso tiene licencias cargadas con fecha posterior al momento de la baja, verificar para cancelar o rechazar las mismas");
            }

            if (response.HasErrors()) return response;

            try
            {
                var allocations = unitOfWork.AllocationRepository.GetLastAllocationsForEmployee(employeeToChange.Id, response.Data.EndDate ?? DateTime.Now);

                if(allocations.Any()) unitOfWork.AllocationRepository.Delete(allocations);

                employeeToChange.CreatedByUser = userName;
                employeeToChange.Modified = DateTime.UtcNow;
                employeeToChange.EndDate = response.Data.EndDate;
                employeeToChange.EndReason = model.Comments;
                employeeToChange.TypeEndReasonId = model.Type.GetValueOrDefault();
                //employeeToChange.ExcludeForTigerReport = true;

                unitOfWork.EmployeeRepository.UpdateEndDate(employeeToChange);

                var user = unitOfWork.UserRepository.GetByEmail(employeeToChange.Email);

                if (user != null)
                {
                    user.Active = false;
                    user.EndDate = DateTime.UtcNow.Date;
                    unitOfWork.UserRepository.Update(user);
                }

                // Delete news
                unitOfWork.EmployeeSyncActionRepository.Delete(response.Data);

                // Save all changes
                unitOfWork.Save();

                //Elimina las Licencias
                var licenses = unitOfWork.LicenseRepository.GetByEmployeeAndStartDate(employeeToChange.Id, response.Data.EndDate ?? DateTime.Now);
                if (licenses.Any())
                {
                    foreach (var license in licenses)
                    {
                        try
                        {
                            unitOfWork.LicenseRepository.Delete(license);
                            unitOfWork.WorkTimeRepository.RemoveBetweenDays(license.EmployeeId, license.StartDate, license.EndDate);
                        }
                        catch (Exception e)
                        {
                            logger.LogError(e);
                            response.AddWarning(Resources.Rrhh.License.GenerateWorkTimesError);
                        }
                    }
                    unitOfWork.Save();
                }
                            
                response.AddSuccess(Resources.AllocationManagement.Employee.Deleted);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            SendMailForUnsubscribe(response, employeeToChange);

            return response;
        }

        public Response<EmployeeSyncAction> Cancel(int newsId)
        {
            var response = new Response<EmployeeSyncAction>();

            EmployeeSyncActionValidationHelper.Exist(newsId, response, unitOfWork);

            if (response.HasErrors()) return response;

            try
            {
                unitOfWork.EmployeeSyncActionRepository.Delete(response.Data);
                unitOfWork.Save();

                response.Data = null;
                response.AddSuccess(Resources.AllocationManagement.EmployeeSyncAction.Deleted);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void SendMailForUnsubscribe(Response<EmployeeSyncAction> response, Employee employeeToChange)
        {
            try
            {
                var mailPmo = unitOfWork.GroupRepository.GetEmail(emailConfig.PmoCode);

                var analitycs = unitOfWork.AnalyticRepository.GetAnalyticsByEmployee(employeeToChange.Id);

                var mails = new List<string> { mailPmo };

                mails.AddRange(from analityc in analitycs where !string.IsNullOrWhiteSpace(analityc.Manager.Email) select analityc.Manager.Email);

                var data = new EmployeeEndConfirmationData
                {
                    Recipients = mails,
                    EmployeeNumber = employeeToChange.EmployeeNumber,
                    Name = employeeToChange.Name,
                    EndDate = employeeToChange.EndDate.GetValueOrDefault().ToString("dd/MM/yyyy")
                };

                var mail = mailBuilder.GetEmail(data);

                mailSender.Send(mail);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddWarning(Resources.Common.ErrorSendMail);
            }
        }

        private List<EmployeeNewsModel> Translate(List<EmployeeSyncAction> employeeSyncActions)
        {
            return mapper.Map<List<EmployeeSyncAction>, List<EmployeeNewsModel>>(employeeSyncActions);
        }

        private EmployeeHistory Translate(Employee employee)
        {
            return mapper.Map<Employee, EmployeeHistory>(employee);
        }
    }
}
