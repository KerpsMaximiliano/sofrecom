using System;
using Sofco.Core.Services.AllocationManagement;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Framework.MailData;
using Sofco.Model.Utils;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Model.DTO;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Resources.Mails;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<EmployeeService> logger;
        private readonly IMailSender mailSender;
        private readonly IMailBuilder mailBuilder;
        private readonly EmailConfig emailConfig;
        private readonly IMapper mapper;

        public EmployeeService(IUnitOfWork unitOfWork, ILogMailer<EmployeeService> logger, IMailSender mailSender, IOptions<EmailConfig> emailOptions, IMailBuilder mailBuilder, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mailSender = mailSender;
            this.mailBuilder = mailBuilder;
            this.mapper = mapper;
            emailConfig = emailOptions.Value;
        }

        public ICollection<Employee> GetAll()
        {
            return unitOfWork.EmployeeRepository.GetAll();
        }

        public Response<EmployeeModel> GetById(int id)
        {
            var response = new Response<EmployeeModel>();

            var result = EmployeeValidationHelper.Find(response, unitOfWork.EmployeeRepository, id);

            response.Data = Translate(result);

            return response;
        }

        private EmployeeModel Translate(Employee employee)
        {
            return mapper.Map<Employee, EmployeeModel>(employee);
        }

        public Response<ICollection<Employee>> Search(EmployeeSearchParams parameters)
        {
            var response = new Response<ICollection<Employee>>
            {
                Data = unitOfWork.EmployeeRepository.Search(parameters)
            };

            if (!response.Data.Any())
            {
                response.AddWarning(Resources.AllocationManagement.Employee.EmployeesNotFound);
            }

            return response;
        }

        public Response SendUnsubscribeNotification(string employeeName, UnsubscribeNotificationParams parameters)
        {
            var response = new Response();

            if (string.IsNullOrWhiteSpace(employeeName))
            {
                response.AddError(Resources.AllocationManagement.Employee.NameRequired);
                return response;
            }

            var manager = unitOfWork.UserRepository.GetSingle(x => x.UserName.Equals(parameters.UserName));

            var mailRrhh = unitOfWork.GroupRepository.GetEmail(emailConfig.RrhhCode);

            parameters.Receipents.Add(mailRrhh);
            
            try
            {
                var email = mailBuilder.GetEmail(new EmployeeEndNotificationData
                {
                    Recipients = string.Join(";", parameters.Receipents),
                    Message = string.Format(MailMessageResource.EmployeeEndNotification, employeeName, manager.Name, parameters.EndDate.ToString("d"))
                });

                mailSender.Send(email);

                response.AddSuccess(Resources.Common.MailSent);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.ErrorSendMail);
                
            }

            return response;
        }

        public Response<EmployeeProfileModel> GetProfile(int id)
        {
            var response = new Response<EmployeeProfileModel>();

            var employee = EmployeeValidationHelper.Find(response, unitOfWork.EmployeeRepository, id);

            if (response.HasErrors()) return response;

            var employeeAllocations = unitOfWork.AllocationRepository.GetByEmployee(id);

            var analitycs = employeeAllocations.Select(x => x.Analytic).Distinct();

            var model = TranslateToProfile(employee);

            foreach (var analityc in analitycs)
            {
                var firstAllocation = analityc.Allocations.FirstOrDefault();

                model.Allocations.Add(new EmployeeAllocationModel
                {
                    Title = analityc.Title,
                    Name = analityc.Name,
                    Client = analityc.ClientExternalName,
                    Service = analityc.Service,
                    StartDate = firstAllocation?.StartDate,
                    ReleaseDate = firstAllocation?.ReleaseDate,
                });
            }

            model.History =
                Translate(unitOfWork.EmployeeHistoryRepository.GetByEmployeeNumber(employee.EmployeeNumber));

            model.HealthInsurance = unitOfWork.HealthInsuranceRepository.GetByCode(employee.HealthInsuranceCode);

            model.PrepaidHealth = unitOfWork.PrepaidHealthRepository.GetByCode(employee.HealthInsuranceCode, employee.PrepaidHealthCode);

            response.Data = model;

            return response;
        }

        private List<EmployeeHistoryModel> Translate(List<EmployeeHistory> employeeHistories)
        {
            return mapper.Map<List<EmployeeHistory>, List<EmployeeHistoryModel>>(employeeHistories);
        }

        private EmployeeProfileModel TranslateToProfile(Employee employee)
        {
            return mapper.Map<Employee, EmployeeProfileModel>(employee);
        }
    }
}
