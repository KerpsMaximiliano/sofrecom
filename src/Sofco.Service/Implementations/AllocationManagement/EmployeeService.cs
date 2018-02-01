using System;
using Sofco.Core.Services.AllocationManagement;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
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

        public EmployeeService(IUnitOfWork unitOfWork, ILogMailer<EmployeeService> logger, IMailSender mailSender, IOptions<EmailConfig> emailOptions, IMailBuilder mailBuilder)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mailSender = mailSender;
            this.mailBuilder = mailBuilder;
            emailConfig = emailOptions.Value;
        }

        public ICollection<Employee> GetAll()
        {
            return unitOfWork.EmployeeRepository.GetAll();
        }

        public Response<Employee> GetById(int id)
        {
            var response = new Response<Employee>();

            response.Data = EmployeeValidationHelper.Find(response, unitOfWork.EmployeeRepository, id);

            return response;
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

        public Response<EmployeeProfileDto> GetProfile(int id)
        {
            var response = new Response<EmployeeProfileDto> { Data = new EmployeeProfileDto() };

            var employee = EmployeeValidationHelper.Find(response, unitOfWork.EmployeeRepository, id);

            if (response.HasErrors()) return response;

            var employeeAllocations = unitOfWork.AllocationRepository.GetByEmployee(id);

            var analitycs = employeeAllocations.Select(x => x.Analytic).Distinct();

            response.Data.Id = employee.Id;
            response.Data.EmployeeNumber = employee.EmployeeNumber;
            response.Data.Manager = "Diego O. Miguel";
            response.Data.Name = employee.Name;
            response.Data.Office = "Reconquista";
            response.Data.Percentage = employee.BillingPercentage;
            response.Data.Profile = employee.Profile;
            response.Data.Seniority = employee.Seniority;
            response.Data.Technology = employee.Technology;

            foreach (var analityc in analitycs)
            {
                var firstAllocation = analityc.Allocations.FirstOrDefault();

                response.Data.Allocations.Add(new EmployeeAllocationDto
                {
                    Title = analityc.Title,
                    Name = analityc.Name,
                    Client = analityc.ClientExternalName,
                    Service = analityc.Service,
                    StartDate = firstAllocation?.StartDate,
                    ReleaseDate = firstAllocation?.ReleaseDate,
                });
            }

            return response;
        }
    }
}
