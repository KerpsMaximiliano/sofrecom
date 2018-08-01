﻿using System;
using Sofco.Core.Services.AllocationManagement;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Framework.MailData;
using Sofco.Domain.Utils;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Relationships;
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
        private readonly ISessionManager sessionManager;
        private readonly IEmployeeData employeeData;

        public EmployeeService(IUnitOfWork unitOfWork, ILogMailer<EmployeeService> logger, IMailSender mailSender, IOptions<EmailConfig> emailOptions, IMailBuilder mailBuilder, IMapper mapper, ISessionManager sessionManager, IEmployeeData employeeData)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mailSender = mailSender;
            this.mailBuilder = mailBuilder;
            this.mapper = mapper;
            this.sessionManager = sessionManager;
            this.employeeData = employeeData;
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
            parameters.UserName = sessionManager.GetUserName();

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

            response.Data = GetEmployeeModel(employee);

            return response;
        }

        public Response FinalizeExtraHolidays(int id)
        {
            var response = new Response();

            var employee = EmployeeValidationHelper.Find(response, unitOfWork.EmployeeRepository, id);

            if (response.HasErrors()) return response;

            try
            {
                employee.ExtraHolidaysQuantity = 0;
                employee.HasExtraHolidays = false;

                unitOfWork.EmployeeRepository.Update(employee);
                unitOfWork.Save();

                response.AddSuccess(Resources.AllocationManagement.Employee.UpdateSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response AddCategories(EmployeeAddCategoriesParams parameters)
        {
            var response = new Response();

            foreach (var employeeId in parameters.Employees)
            {
                foreach (var categoryId in parameters.Categories)
                {
                    if (!unitOfWork.CategoryRepository.ExistEmployeeCategory(employeeId, categoryId))
                    {
                        var employeeCategory = new EmployeeCategory(employeeId, categoryId);
                        unitOfWork.CategoryRepository.AddEmployeeCategory(employeeCategory);
                    }
                }
            }

            try
            {
                unitOfWork.Save();
                response.AddSuccess(Resources.AllocationManagement.Employee.CategoriesAdded);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public ICollection<Option> GetAnalytics(int id)
        {
            return unitOfWork.AnalyticRepository.GetAnalyticsByManagerId(id).Select(x => new Option { Id = x.Id, Text = $"{x.Title} - {x.Name}" }).ToList();
        }

        public Response<IList<EmployeeCategoryOption>> GetCategories(int employeeId)
        {
            var employeeCategories = unitOfWork.EmployeeRepository.GetEmployeeCategories(employeeId);

            var response = new Response<IList<EmployeeCategoryOption>> { Data = new List<EmployeeCategoryOption>() };

            foreach (var employeeCategory in employeeCategories)
            {
                if (employeeCategory.Category == null) continue;

                foreach (var task in employeeCategory.Category.Tasks)
                {
                    if (!task.Active) continue;

                    var option = new EmployeeCategoryOption
                    {
                        CategoryId = employeeCategory.CategoryId,
                        Category = employeeCategory.Category?.Description,
                        TaskId = task.Id,
                        Task = task.Description
                    };

                    response.Data.Add(option);
                }
            }

            return response;
        }

        public Response UpdateBusinessHours(int id, EmployeeBusinessHoursParams model)
        {
            var response = new Response();

            EmployeeValidationHelper.Exist(response, unitOfWork.EmployeeRepository, id);
            EmployeeValidationHelper.ValidateBusinessHours(response, model);

            if (response.HasErrors()) return response;

            try
            {
                var employee = new Employee();
                employee.Id = id;
                employee.BusinessHours = model.BusinessHours;
                employee.BusinessHoursDescription = model.BusinessHoursDescription;
                employee.OfficeAddress = model.Office;
                employee.HolidaysPendingByLaw = model.HolidaysPending;
                employee.ManagerId = model.ManagerId;
                employee.HolidaysPending = CalculateHolidaysPending(model);

                unitOfWork.EmployeeRepository.UpdateBusinessHours(employee);
                unitOfWork.Save();

                response.AddSuccess(Resources.AllocationManagement.Employee.BusinessHoursUpdated);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }
            
            return response;
        }

        private int CalculateHolidaysPending(EmployeeBusinessHoursParams model)
        {
            var daysPending = (model.HolidaysPending / 7) * 5;
            var resto = model.HolidaysPending % 7;

            if (resto < 6) daysPending += resto;
            if (resto == 6 || resto == 7) daysPending += 5;
            return daysPending;
        }

        public Response<IList<EmployeeCategoryOption>> GetCurrentCategories()
        {
            var currentEmployee = employeeData.GetCurrentEmployee();

            return GetCategories(currentEmployee.Id);
        }

        public IList<UnemployeeListItemModel> GetUnemployees(UnemployeeSearchParameters parameters)
        {
            var employees = unitOfWork.EmployeeRepository.SearchUnemployees(parameters);

            return employees.Select(x => new UnemployeeListItemModel(x)).ToList();
        }

        public Response<EmployeeWorkingPendingHoursModel> GetPendingWorkingHours(int employeeId)
        {
            var pendingHours = unitOfWork.WorkTimeRepository.GetPendingHoursByEmployeeId(employeeId);

            var result = new Response<EmployeeWorkingPendingHoursModel>
            {
                Data = new EmployeeWorkingPendingHoursModel
                {
                    EmployeeId = employeeId,
                    PendingHours = pendingHours
                }
            };

            return result;
        }

        private EmployeeProfileModel GetEmployeeModel(Employee employee)
        {
            var model = TranslateToProfile(employee);

            var employeeAllocations = unitOfWork.AllocationRepository.GetByEmployee(employee.Id);

            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var analitycs = employeeAllocations.Where(x => x.Percentage > 0 && x.StartDate.Date == date).Select(x => x.Analytic).Distinct();

            foreach (var analityc in analitycs)
            {
                var firstAllocation = analityc.Allocations.FirstOrDefault();

                var item = new EmployeeAllocationModel
                {
                    Title = analityc.Title,
                    Name = analityc.Name,
                    Client = analityc.ClientExternalName,
                    Service = analityc.Service,
                    StartDate = firstAllocation?.StartDate,
                    ReleaseDate = firstAllocation?.ReleaseDate,
                };

                var allocationThisMonth = analityc.Allocations.FirstOrDefault(x => x.StartDate == new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                if (allocationThisMonth != null)
                {
                    item.AllocationPercentage = allocationThisMonth.Percentage;
                }

                model.Allocations.Add(item);
            }

            model.ManagerId = employee.ManagerId.GetValueOrDefault();

            if (employee.Manager != null) model.Manager = employee.Manager.Name;

            model.History = Translate(unitOfWork.EmployeeHistoryRepository.GetByEmployeeNumber(employee.EmployeeNumber));

            model.HealthInsurance = unitOfWork.HealthInsuranceRepository.GetByCode(employee.HealthInsuranceCode);

            model.PrepaidHealth = unitOfWork.PrepaidHealthRepository.GetByCode(employee.HealthInsuranceCode, employee.PrepaidHealthCode);

            return model;
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
