using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Rh.Tiger;
using Sofco.Model.Enums;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Repository.Rh.Repositories.Interfaces;
using Sofco.Repository.Rh.Settings;
using Sofco.Service.Implementations.AllocationManagement;

namespace Sofco.Service.Implementations.Jobs
{
    public class EmployeeSyncJobService : IEmployeeSyncJobService
    {
        private const int FromLastMonth = -1;

        private readonly ITigerEmployeeRepository tigerEmployeeRepository;

        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;

        private readonly DateTime startDate;

        private readonly IMailSender mailSender;

        private readonly EmailConfig emailConfig;

        private readonly ILogMailer<EmployeeSyncJobService> logger;

        private const string MailSubject = "NOVEDADES RECURSOS";

        private const string MailBody = "<font size='3'>" +
                                            "<span style='font-size:12pt'>" +
                                                "Estimados, </br></br>" +
                                                "Existen novedades que necesitan su confirmacion, por favor acceder a la lista " +
                                                "a traves del siguiente <a href='{0}' target='_blank'>link</a></br></br>" +
                                                "Saludos" +
                                            "</span>" +
                                        "</font>";

        public EmployeeSyncJobService(ITigerEmployeeRepository tigerEmployeeRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogMailer<EmployeeSyncJobService> logger,
            IMailSender mailSender,
            IOptions<EmailConfig> emailOptions)
        {
            this.tigerEmployeeRepository = tigerEmployeeRepository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            startDate = DateTime.UtcNow.AddMonths(FromLastMonth);
            this.mailSender = mailSender;
            this.emailConfig = emailOptions.Value;
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
                SendMail();
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
                if(news.Any(x => x.EmployeeNumber == employeeNumber && x.Status == EmployeeSyncActionStatus.New)) continue;

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
                SendMail();
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
                if (news.Any(x => x.EmployeeNumber == employeeNumber && x.Status == EmployeeSyncActionStatus.Delete)) continue;

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

        private void SendMail()
        {
            try
            {
                var body = string.Format(MailBody, $"{emailConfig.SiteUrl}allocationManagement/employees/news");

                var mailRrhh = unitOfWork.GroupRepository.GetEmail(emailConfig.RrhhCode);

                mailSender.Send(mailRrhh, MailSubject, body);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex);
            }
        }
    }
}
