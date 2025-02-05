﻿using System;
using Sofco.Core.Services.AllocationManagement;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Views;
using Sofco.Core.FileManager;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Managers.AllocationManagement;
using Sofco.Core.Models;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Models.Rrhh;
using Sofco.Framework.MailData;
using Sofco.Domain.Utils;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Relationships;
using Sofco.Framework.Helpers;
using Sofco.Resources.Mails;
using Sofco.Core.Models.AdvancementAndRefund.Common;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Domain.Models.Reports;

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
        private readonly IEmployeeData employeeData;
        private readonly IUserData userData;
        private readonly IEmployeeEndNotificationManager employeeEndNotificationManager;
        private readonly ICurrentAccountService currentAccountService;
        private readonly IEmployeeFileManager employeeFileManager;
        private readonly IEmployeeViewRepository employeeViewRepository;

        public EmployeeService(IUnitOfWork unitOfWork, 
            ILogMailer<EmployeeService> logger, 
            IMailSender mailSender, 
            IOptions<EmailConfig> emailOptions, 
            IMailBuilder mailBuilder, 
            IMapper mapper, 
            IEmployeeData employeeData, 
            IUserData userData, 
            IEmployeeEndNotificationManager employeeEndNotificationManager,
            IEmployeeFileManager employeeFileManager,
            IEmployeeViewRepository employeeViewRepository,
            ICurrentAccountService currentAccountService)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mailSender = mailSender;
            this.mailBuilder = mailBuilder;
            this.mapper = mapper;
            this.employeeData = employeeData;
            this.userData = userData;
            this.employeeEndNotificationManager = employeeEndNotificationManager;
            emailConfig = emailOptions.Value;
            this.currentAccountService = currentAccountService;
            this.employeeFileManager = employeeFileManager;
            this.employeeViewRepository = employeeViewRepository;
        }

        public ICollection<Employee> GetAll()
        {
            return unitOfWork.EmployeeRepository.GetAll();
        }

        public IList<Employee> GetEveryone()
        {
            return unitOfWork.EmployeeRepository.GetEveryone();
        }
        public ICollection<Employee> GetAllForWorkTimeReport()
        {
            return unitOfWork.EmployeeRepository.GetAllForWorkTimeReport();
        }

        public Response<IList<ReportUpdownItemModel>> GetUpdownReport(ReportUpdownParameters parameters)
        {
            var response = new Response<IList<ReportUpdownItemModel>>();

            if(!parameters.StartDate.HasValue) response.AddError(Resources.AllocationManagement.Allocation.DateSinceRequired);
            if(!parameters.EndDate.HasValue) response.AddError(Resources.AllocationManagement.Allocation.DateToRequired);

            if (parameters.StartDate.HasValue && parameters.EndDate.HasValue)
            {
                if(parameters.EndDate.Value.Date < parameters.StartDate.Value.Date) response.AddError(Resources.AllocationManagement.Allocation.DateToLessThanDateSince);
            }

            if (response.HasErrors()) return response;

            var list = unitOfWork.EmployeeRepository.GetUpdownReport(parameters);

            response.Data = list.Select(x => new ReportUpdownItemModel(x, parameters)).ToList();

            if (!response.Data.Any())
            {
                response.AddWarning(Resources.Common.SearchEmpty);
            }

            return response;
        }

        public Response<EmployeeModel> GetById(int id)
        {
            var response = new Response<EmployeeModel>();

            var result = EmployeeValidationHelper.Find(response, unitOfWork.EmployeeRepository, id);

            response.Data = Translate(result);

            return response;
        }

        public Response<string> GetSectorName(int id)
        {
            var response = new Response<string>();

            var employee = EmployeeValidationHelper.Find(new Response<Core.Models.Admin.UserModel>(), unitOfWork.EmployeeRepository, id);

            var user = unitOfWork.UserRepository.GetSingle(x => x.Email.Equals(employee.Email));

            if(user.Sectors != null)
            {
                //Devolver nombre de sector
                response.Data = "Tiene sector vinculado";
            } else
            {
                response.Data = "No tiene sector vinculado";
            }
            return response;
        }

        private EmployeeModel Translate(Employee employee)
        {
            return mapper.Map<Employee, EmployeeModel>(employee);
        }

        public Response<ICollection<Employee>> Search(EmployeeSearchParams parameters)
        {
            var response = new Response<ICollection<Employee>>(){ Data = new List<Employee>() };

            var setting = unitOfWork.CloseDateRepository.GetBeforeCurrentAndNext();

            var period = setting.GetPeriodExcludeDays();

            var employees = unitOfWork.EmployeeRepository.Search(parameters, period.Item1, period.Item2);

            foreach (var employee in employees)
            {
                if (parameters.ManagerId.HasValue && parameters.ManagerId.Value > 0)
                {
                    if (unitOfWork.AllocationRepository.ExistCurrentAllocationByEmployeeAndManagerId(employee.Id, parameters.ManagerId.Value, parameters.AnalyticId, period.Item1, period.Item2))
                    {
                        response.Data.Add(employee);
                    }
                }
                else
                {
                    response.Data.Add(employee);
                }
            }

            if (!response.Data.Any())
            {
                response.AddWarning(Resources.AllocationManagement.Employee.EmployeesNotFound);
            }

            return response;
        }

        public Response SendUnsubscribeNotification(EmployeeEndNotificationModel model)
        {
            var currentUser = userData.GetCurrentUser();

            model.ApplicantUserId = currentUser.Id;
            model.UserName = currentUser.UserName;

            var employeeName = model.EmployeeName;

            var response = new Response();

            if (string.IsNullOrWhiteSpace(employeeName))
            {
                response.AddError(Resources.AllocationManagement.Employee.NameRequired);
                return response;
            }
                    
            var manager = unitOfWork.UserRepository.GetSingle(x => x.UserName.Equals(model.UserName));

            var mailRrhh = unitOfWork.GroupRepository.GetEmail(emailConfig.RrhhCode);

            model.Recipients.Add(mailRrhh);

            try
            {
                var email = mailBuilder.GetEmail(new EmployeeEndNotificationData
                {
                    Recipients = model.Recipients.ToList(),
                    Message = string.Format(MailMessageResource.EmployeeEndNotification,
                        employeeName,
                        manager.Name,
                        model.EndDate.ToString("dd/MM/yyyy"))
                });

                mailSender.Send(email);

                employeeEndNotificationManager.Save(model);

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
                foreach (var categoryId in parameters.CategoriesToAdd)
                {
                    if (!unitOfWork.CategoryRepository.ExistEmployeeCategory(employeeId, categoryId))
                    {
                        var employeeCategory = new EmployeeCategory(employeeId, categoryId);
                        unitOfWork.CategoryRepository.AddEmployeeCategory(employeeCategory);
                    }
                }

                foreach (var categoryId in parameters.CategoriesToRemove)
                {
                    if (unitOfWork.CategoryRepository.ExistEmployeeCategory(employeeId, categoryId))
                    {
                        var employeeCategory = new EmployeeCategory(employeeId, categoryId);
                        unitOfWork.CategoryRepository.RemoveEmployeeCategory(employeeCategory);
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

        public Response Update(int id, EmployeeBusinessHoursParams model)
        {
            var response = new Response();

            var stored = EmployeeValidationHelper.Find(response, unitOfWork.EmployeeRepository, id);
            EmployeeValidationHelper.ValidateBusinessHours(response, model);
            EmployeeValidationHelper.ValidateBillingPercentage(response, model);
            EmployeeValidationHelper.ValidateOffice(response, model);
            EmployeeValidationHelper.ValidateManager(response, model, unitOfWork);

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                response.AddError(Resources.AllocationManagement.Employee.MailEmpty);
            }

            if (response.HasErrors()) return response;

            try
            {
                var employee = new Employee();
                employee.Id = id;
                employee.BusinessHours = model.BusinessHours.GetValueOrDefault();
                employee.BusinessHoursDescription = model.BusinessHoursDescription;
                employee.OfficeAddress = model.Office;
                employee.Email = model.Email;
           
                employee.ManagerId = model.ManagerId;
                employee.BillingPercentage = model.BillingPercentage.GetValueOrDefault();
                employee.HasCreditCard = model.HasCreditCard;

                employee.HolidaysPendingByLaw = CalculateHolidaysPending(model.HolidaysPending.GetValueOrDefault());
                employee.HolidaysPending = model.HolidaysPending.GetValueOrDefault();

                employee.ExtraHolidaysQuantityByLaw = CalculateHolidaysPending(model.ExtraHolidays.GetValueOrDefault());
                employee.ExtraHolidaysQuantity = model.ExtraHolidays.GetValueOrDefault();

                employee.HasExtraHolidays = employee.ExtraHolidaysQuantity != 0;

                unitOfWork.EmployeeRepository.UpdateBusinessHours(employee);
                unitOfWork.Save();

                response.AddSuccess(Resources.AllocationManagement.Employee.BusinessHoursUpdated);
                SaveProfileHistory(stored, employee);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void SaveProfileHistory(Employee stored, Employee employee)
        {
            var fields = GetFieldToCompare();

            var modifiedFields = ElementComparerHelper.CompareModification(employee, stored, fields.ToArray());

            if (modifiedFields.Any())
            {
                unitOfWork.EmployeeProfileHistoryRepository.Insert(CreateProfileHistory(stored, employee, modifiedFields));
                unitOfWork.Save();
            }
        }

        private EmployeeProfileHistory CreateProfileHistory(Employee current, Employee updated, string[] modifiedFields)
        {
            var currentData = JsonSerializeHelper.Serialize(current);
            var updateData = JsonSerializeHelper.Serialize(updated);
            var modifiedFieldsData = JsonSerializeHelper.Serialize(modifiedFields);

            return new EmployeeProfileHistory
            {
                Created = DateTime.UtcNow,
                EmployeeNumber = current.EmployeeNumber,
                EmployeeData = updateData,
                EmployeePreviousData = currentData,
                ModifiedFields = modifiedFieldsData
            };
        }

        private List<string> GetFieldToCompare()
        {
            return new List<string>
            {
                nameof(Employee.BusinessHours),
                nameof(Employee.BusinessHoursDescription),
                nameof(Employee.OfficeAddress),
                nameof(Employee.HolidaysPendingByLaw),
                nameof(Employee.ManagerId),
                nameof(Employee.BillingPercentage),
                nameof(Employee.HolidaysPending)
            };
        }

        private int CalculateHolidaysPending(int days)
        {
            var daysPending = (days / 5) * 2;

            return daysPending + days;
        }

        public Response<IList<EmployeeCategoryOption>> GetCurrentCategories()
        {
            var currentEmployee = employeeData.GetCurrentEmployee();

            return GetCategories(currentEmployee.Id);
        }

        public IList<UnemployeeListItemModel> GetUnemployees(UnemployeeSearchParameters parameters)
        {
            var employees = unitOfWork.EmployeeRepository.SearchUnemployees(parameters);
            var list = new List<UnemployeeListItemModel>();

            foreach (var employee in employees)
            {
                if (parameters.ManagerId.HasValue && parameters.ManagerId.Value > 0)
                {
                    if (unitOfWork.AllocationRepository.ExistAllocationByEmployeeAndManagerId(employee.Id, parameters.ManagerId.Value, parameters.AnalyticId))
                    {
                        list.Add(new UnemployeeListItemModel(employee));
                    }
                }
                else
                {
                    list.Add(new UnemployeeListItemModel(employee));
                }
            }

            return list;
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

        public Response AddExternal(AddExternalModel model)
        {
            var response = new Response();

            var user = EmployeeValidationHelper.User(response, unitOfWork, model);
            EmployeeValidationHelper.Manager(response, unitOfWork, model);
            EmployeeValidationHelper.Phone(response, unitOfWork, model);
            EmployeeValidationHelper.Hours(response, unitOfWork, model);

            if (response.HasErrors()) return response;

            try
            {
                var employee = new Employee
                {
                    Email = user.Email,
                    Name = user.Name,
                    PhoneCountryCode = model.CountryCode,
                    PhoneAreaCode = model.AreaCode,
                    PhoneNumber = model.Phone,
                    ManagerId = model.ManagerId,
                    StartDate = DateTime.UtcNow,
                    IsExternal = true,
                    BusinessHours = model.Hours
                };

                unitOfWork.EmployeeRepository.Insert(employee);
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

        public Response<List<Option>> GetEmployeeOptionByCurrentManager()
        {
            var analytics = unitOfWork.AnalyticRepository.GetAnalyticLiteByManagerId(userData.GetCurrentUser().Id);

            var employees = unitOfWork.EmployeeRepository.GetByAnalyticIds(analytics.Select(x => x.Id).ToList()); 

            return new Response<List<Option>> { Data = Translate(employees.ToList()) };
        }

        public Response<IList<EmployeeAdvancementDetail>> GetAdvancements(int id)
        {
            var response = new Response<IList<EmployeeAdvancementDetail>>();
            response.Data = new List<EmployeeAdvancementDetail>();

            var employee = unitOfWork.EmployeeRepository.Get(id);

            if (employee != null)
            {
                var user = unitOfWork.UserRepository.GetByEmail(employee.Email);

                if (user != null)
                {
                    response.Data = unitOfWork.AdvancementRepository.GetByApplicant(user.Id).Select(x => new EmployeeAdvancementDetail(x)).ToList();
                }
            }

            return response;
        }

        public Response<IList<EmployeeRefundDetail>> GetRefunds(int id)
        {
            var response = new Response<IList<EmployeeRefundDetail>>();
            response.Data = new List<EmployeeRefundDetail>();

            var employee = unitOfWork.EmployeeRepository.Get(id);

            if (employee != null)
            {
                var user = unitOfWork.UserRepository.GetByEmail(employee.Email);

                if (user != null)
                {
                    response.Data = unitOfWork.RefundRepository.GetByApplicant(user.Id)
                        .Select(x => mapper.Map<Refund, EmployeeRefundDetail>(x)).ToList();
                }
            }

            return response;
        }

        public Response<IList<EmployeeCurrentAccount>> GetCurrentAccount(int id)
        {
            var response = new Response<IList<EmployeeCurrentAccount>>();
            var allCurrentAccount = currentAccountService.Get();

            if (allCurrentAccount.Data.Count > 0)
            {
                var employee = unitOfWork.EmployeeRepository.Get(id);

                if (employee != null)
                {
                    var user = unitOfWork.UserRepository.GetByEmail(employee.Email);

                    if (user != null)
                    {
                        response.Data = allCurrentAccount.Data.Where(cc => cc.UserId == user.Id)
                            .Select(x => new EmployeeCurrentAccount
                            {
                                Currency = x.Currency,
                                AdvancementTotal = x.AdvancementTotal,
                                CompanyRefund = x.CompanyRefund,
                                RefundTotal = x.RefundTotal,
                                UserRefund = x.UserRefund
                            }).ToList();
                    }
                }
            }

            return response;
        }

        public Response<byte[]> GetReport()
        {
            var list = employeeViewRepository.Get();

            var response = new Response<byte[]>();

            if (!list.Any())
            {
                response.AddWarning(Resources.Common.SearchEmpty);
                return response;
            }

            response.Data = employeeFileManager.CreateReport(list).GetAsByteArray();

            return response;
        }

        public Response<byte[]> GetShortReport()
        {
            var list = employeeViewRepository.Get();

            var response = new Response<byte[]>();

            if (!list.Any())
            {
                response.AddWarning(Resources.Common.SearchEmpty);
                return response;
            }

            var items = new List<EmployeeView>();

            list = list.OrderBy(x => x.Name).ThenByDescending(x => x.Percentage).ToList();

            var employeeNumber = string.Empty;

            foreach (var employeeView in list)
            {
                if (employeeView.EmployeeNumber != employeeNumber)
                {
                    var employeeItems = list.Where(x => x.EmployeeNumber == employeeView.EmployeeNumber);
                    var first = employeeItems.FirstOrDefault();
                    items.Add(first);

                    employeeNumber = employeeView.EmployeeNumber;
                }    
            }

            response.Data = employeeFileManager.CreateReport(items).GetAsByteArray();

            return response;
        }

        public Response UpdateAssingComment(UpdateAssingCommentModel model)
        {
            var response = new Response();

            var employee = unitOfWork.EmployeeRepository.Get(model.EmployeeId);

            if (employee == null)
            {
                response.AddError(Resources.AllocationManagement.Employee.NotFound);
                return response;
            }

            try
            {
                employee.AssignComments = model.Comment;
                unitOfWork.EmployeeRepository.UpdateAssignComments(employee);
                unitOfWork.Save();

                response.AddSuccess(Resources.Common.SaveSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<EmployeeModel> GetByMail(string email)
        {
            var response = new Response<EmployeeModel>();

            var result = unitOfWork.EmployeeRepository.GetByEmail(email);

            response.Data = Translate(result);

            return response;
        }

        private EmployeeProfileModel GetEmployeeModel(Employee employee)
        {
            var model = TranslateToProfile(employee);

            var employeeAllocations = unitOfWork.AllocationRepository.GetByEmployee(employee.Id);

            DateTime date;

            if (employee.EndDate.HasValue)
            {
                date = new DateTime(employee.EndDate.Value.Year, employee.EndDate.Value.Month, 1);
            }
            else
            {
                date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            var analitycs = employeeAllocations.Where(x => x.Percentage > 0 && x.StartDate.Date == date).Select(x => x.Analytic).Distinct();

            foreach (var analityc in analitycs)
            {
                var allocationThisMonth = analityc.Allocations.FirstOrDefault(x => x.StartDate == date.Date);
                if (allocationThisMonth != null)
                {
                    var item = new EmployeeAllocationModel
                    {
                        Title = analityc.Title,
                        Name = analityc.Name,
                        AllocationPercentage = allocationThisMonth.Percentage,
                        StartDate = unitOfWork.AllocationRepository.GetStartDate(analityc.Id, employee.Id),
                        Client = analityc.AccountName,
                        Service = analityc.ServiceName,
                        ReleaseDate = allocationThisMonth?.ReleaseDate,
                    };

                    model.Allocations.Add(item);
                }
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

        private List<Option> Translate(List<Employee> employees)
        {
            return mapper.Map<List<Employee>, List<Option>>(employees);
        }
    }
}
