using System;
using Sofco.Core.Services.AllocationManagement;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Model.Utils;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<EmployeeService> logger;

        public EmployeeService(IUnitOfWork unitOfWork, ILogMailer<EmployeeService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
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

        public ICollection<EmployeeSyncAction> GetNews()
        {
            return unitOfWork.EmployeeSyncActionRepository.GetAll();
        }

        public Response<EmployeeSyncAction> DeleteNews(int id)
        {
            var response = new Response<EmployeeSyncAction>();

            EmployeeSyncActionValidationHelper.Exist(id, response, unitOfWork);

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
                this.logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<EmployeeSyncAction> Add(int newsId, string userName)
        {
            var response = new Response<EmployeeSyncAction>();

            EmployeeSyncActionValidationHelper.Exist(newsId, response, unitOfWork);
            EmployeeSyncActionValidationHelper.ValidateNewStatus(response);

            if (response.HasErrors()) return response;

            try
            {
                var employee = JsonConvert.DeserializeObject<Employee>(response.Data.EmployeeData);
                employee.Created = DateTime.UtcNow;
                employee.Modified = DateTime.UtcNow;
                employee.CreatedByUser = userName;

                // Add new employee
                unitOfWork.EmployeeRepository.Insert(employee);

                // Delete news
                unitOfWork.EmployeeSyncActionRepository.Delete(response.Data);

                // Save all changes
                unitOfWork.Save();

                response.AddSuccess(Resources.AllocationManagement.Employee.Added);
            }
            catch (Exception e)
            {
                this.logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }
    }
}
