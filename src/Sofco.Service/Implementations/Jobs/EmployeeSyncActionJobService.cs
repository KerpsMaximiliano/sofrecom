using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Rh.Tiger;
using Sofco.Framework.MailData;
using Sofco.Model.Enums;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Repository.Rh.Repositories.Interfaces;
using Sofco.Repository.Rh.Settings;
using Sofco.Resources.Mails;

namespace Sofco.Service.Implementations.Jobs
{
    public class EmployeeSyncActionJobService : IEmployeeSyncActionJobService
    {
        private const int FromLastMonth = -1;

        private readonly ITigerEmployeeRepository tigerEmployeeRepository;

        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;

        private readonly DateTime startDate;

        private readonly IMailSender mailSender;

        private readonly EmailConfig emailConfig;

        private readonly ILogMailer<EmployeeSyncActionJobService> logger;

        private readonly IMailBuilder mailBuilder;

        public EmployeeSyncActionJobService(ITigerEmployeeRepository tigerEmployeeRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogMailer<EmployeeSyncActionJobService> logger,
            IMailSender mailSender,
            IOptions<EmailConfig> emailOptions, IMailBuilder mailBuilder)
        {
            this.tigerEmployeeRepository = tigerEmployeeRepository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            startDate = DateTime.UtcNow.AddMonths(FromLastMonth);
            this.mailSender = mailSender;
            this.mailBuilder = mailBuilder;
            emailConfig = emailOptions.Value;
            this.logger = logger;
        }

        public void SyncNewEmployees()
        {
            var procesedEmployees = Translate(tigerEmployeeRepository.GetWithStartDate(startDate).ToList());

            procesedEmployees = procesedEmployees.Where(s => s.EndDate == null).ToList();

            var newEmployees = GetNewEmployees(procesedEmployees);

            var syncActions = Translate(newEmployees, EmployeeSyncActionStatus.New);

            if (syncActions.Count > 0)
            {
                SendMail(newEmployees, EmployeeSyncActionStatus.New);
            }

            unitOfWork.EmployeeSyncActionRepository.Save(syncActions);
        }

        private List<Employee> GetNewEmployees(List<Employee> employees)
        {
            var numbers = employees.Select(s => s.EmployeeNumber).ToArray();

            var storedEmployees = unitOfWork.EmployeeRepository.GetByEmployeeNumber(numbers);

            var news = unitOfWork.EmployeeSyncActionRepository.GetAll();

            var newEmployees = new List<Employee>();

            foreach (var employee in employees)
            {
                var employeeNumber = employee.EmployeeNumber;

                // Si ya hay una novedad de ese empleado
                if(news != null && news.Any(x => x.EmployeeNumber == employeeNumber && x.Status == EmployeeSyncActionStatus.New)) continue;

                var isNew = storedEmployees.All(s => s.EmployeeNumber != employeeNumber);

                if (!isNew)
                {
                    var storedEmployee = storedEmployees.FirstOrDefault(s => s.EmployeeNumber == employeeNumber);
                    if (storedEmployee != null 
                        && storedEmployee.StartDate != employee.StartDate)
                    {
                        isNew = true;
                    }
                }

                if (isNew)
                {
                    newEmployees.Add(employee);
                }
            }

            return newEmployees;
        }

        public void SyncEndEmployees()
        {
            var procesedEmployees = Translate(tigerEmployeeRepository.GetWithEndDate(startDate).ToList());

            var endEmployees = GetEndEmployees(procesedEmployees);

            var syncActions = Translate(endEmployees, EmployeeSyncActionStatus.Delete);

            if (syncActions.Count > 0)
            {
                SendMail(endEmployees, EmployeeSyncActionStatus.Delete);
            }

            unitOfWork.EmployeeSyncActionRepository.Save(syncActions);
        }

        private List<Employee> GetEndEmployees(List<Employee> employees)
        {
            var numbers = employees.Select(s => s.EmployeeNumber).ToArray();

            var storedEmployees = unitOfWork.EmployeeRepository.GetByEmployeeNumber(numbers);

            var endEmployees = new List<Employee>();

            var news = unitOfWork.EmployeeSyncActionRepository.GetAll();

            foreach (var employee in employees)
            {
                var employeeNumber = employee.EmployeeNumber;

                // Si ya hay una novedad de ese empleado
                if (news != null && news.Any(x => x.EmployeeNumber == employeeNumber && x.Status == EmployeeSyncActionStatus.Delete)) continue;

                var storedEmployee = storedEmployees.FirstOrDefault(s => s.EmployeeNumber == employeeNumber);
                if (storedEmployee != null 
                    && storedEmployee.EndDate == null
                    && employee.EndDate > RhSetting.TigerDateTimeMinValue)
                {
                    endEmployees.Add(employee);
                }
            }

            return endEmployees;
        }

        private List<Employee> Translate(List<TigerEmployee> tigerEmployees)
        {
            return mapper.Map<List<TigerEmployee>, List<Employee>>(tigerEmployees);
        }

        private List<EmployeeSyncAction> Translate(List<Employee> employees, string status)
        {
            var result = mapper.Map<List<Employee>, List<EmployeeSyncAction>>(employees);

            result.ForEach(s => { s.Status = status; });

            return result;
        }

        private void SendMail(List<Employee> employees, string status)
        {
            try
            {
                var mailRrhh = unitOfWork.GroupRepository.GetEmail(emailConfig.RrhhCode);

                var infoText = GetInfoText(employees, status);

                var message = new StringBuilder();
                message.AppendFormat(MailMessageResource.EmployeeNews, $"{emailConfig.SiteUrl}allocationManagement/employees/news");
                message.AppendLine(infoText);

                var mailData = new EmployeeNewsData
                {
                    Recipients = mailRrhh,
                    Message = message.ToString()
                };

                var email = mailBuilder.GetEmail(mailData);

                mailSender.Send(email);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
            }
        }

        private string GetInfoText(List<Employee> employees, string status)
        {
            var actionTxt = status == EmployeeSyncActionStatus.New ? MailCommonResource.EmployeeActionNew : MailCommonResource.EmployeeActionDelete;

            var list = new StringBuilder();

            foreach (var employee in employees)
            {
                list.AppendFormat("<li>{0}</li>", employee.Name);
            }

            return string.Format("{0}:<br><br><ul>{1}</ul>", actionTxt, list);
        }
    }
}
