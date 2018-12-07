using Sofco.Core.DAL;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Framework.ValidationHelpers.AllocationManagement
{
    public static class EmployeeValidationHelper
    {
        public static void Exist(Response response, IEmployeeRepository employeeRepository, int employeeId)
        {
            var exist = employeeRepository.Exist(employeeId);

            if (!exist)
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.Employee.NotFound, MessageType.Error));
            }
        }

        public static Employee Find(Response response, IEmployeeRepository employeeRepository, int id)
        {
            var employee = employeeRepository.GetById(id);

            if (employee == null)
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.Employee.NotFound, MessageType.Error));
            }

            return employee;
        }

        public static void ValidateBusinessHours(Response response, EmployeeBusinessHoursParams model)
        {
            if (model.BusinessHours < 1 || model.BusinessHours > 8)
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.Employee.BusinessHoursWrong, MessageType.Error));
            }

            if (string.IsNullOrWhiteSpace(model.BusinessHoursDescription))
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.Employee.BusinessHoursEmpty, MessageType.Error));
            }
        }

        public static User User(Response response, IUnitOfWork unitOfWork, AddExternalModel model)
        {
            var user = unitOfWork.UserRepository.Get(model.UserId);

            if (user == null)
            {
                response.AddError(Resources.Admin.User.NotFound);
                return null;
            }

            var employee = unitOfWork.EmployeeRepository.GetByEmail(user.Email);

            if (employee != null)
            {
                response.AddError(Resources.AllocationManagement.Employee.UserRelated);
            }

            return user;
        }

        public static void Manager(Response response, IUnitOfWork unitOfWork, AddExternalModel model)
        {
            var user = unitOfWork.UserRepository.Get(model.ManagerId);

            if (user == null)
            {
                response.AddError(Resources.Admin.User.ManagerNotFound);
            }
        }

        public static void Phone(Response response, IUnitOfWork unitOfWork, AddExternalModel model)
        {
            if (model.AreaCode == 0 || model.CountryCode == 0 || string.IsNullOrWhiteSpace(model.Phone))
            {
                response.AddError(Resources.AllocationManagement.Employee.PhoneRequired);
            }
        }

        public static void Hours(Response response, IUnitOfWork unitOfWork, AddExternalModel model)
        {
            if (model.Hours == 0)
            {
                response.AddError(Resources.AllocationManagement.Employee.HoursRequired);
            }
        }

        public static void ValidateBillingPercentage(Response response, EmployeeBusinessHoursParams model)
        {
            if (!model.BillingPercentage.HasValue)
            {
                response.AddError(Resources.AllocationManagement.Employee.BillingPercentageRequired);
            }
        }

        public static void ValidateManager(Response response, EmployeeBusinessHoursParams model, IUnitOfWork unitOfWork)
        {
            if (!model.ManagerId.HasValue || model.ManagerId.Value <= 0)
            {
                response.AddError(Resources.AllocationManagement.Employee.ManagerRequired);
            }
            else
            {
                var user = unitOfWork.UserRepository.Get(model.ManagerId.Value);

                if (user == null)
                {
                    response.AddError(Resources.Admin.User.ManagerNotFound);
                }
            }
        }
    }
}
