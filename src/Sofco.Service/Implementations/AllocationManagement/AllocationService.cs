using System;
using System.Collections.Generic;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.DTO;
using Sofco.Model.Models.TimeManagement;
using Sofco.Model.Utils;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Model.Enums;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class AllocationService : IAllocationService
    {
        private readonly IAllocationRepository allocationRepository;
        private readonly IAnalyticRepository analyticRepository;
        private readonly IEmployeeRepository employeeRepository;

        public AllocationService(IAllocationRepository allocationRepo, IAnalyticRepository analyticRepo, IEmployeeRepository employeeRepo)
        {
            allocationRepository = allocationRepo;
            analyticRepository = analyticRepo;
            employeeRepository = employeeRepo;
        }

        public Response<Allocation> Add(AllocationAsignmentParams parameters)
        {
            var response = new Response<Allocation>();

            var analytic = AnalyticValidationHelper.Find(response, analyticRepository, parameters.AnalyticId);
            AllocationValidationHelper.ValidateDates(response, parameters.DateSince, parameters.DateTo);

            if (response.HasErrors()) return response;

            EmployeeValidationHelper.Exist(response, employeeRepository, parameters.EmployeeId);
            AllocationValidationHelper.ValidateAnalyticDates(analytic, response, parameters);
            AllocationValidationHelper.ValidatePercentage(response, parameters.Percentage);

            if (response.HasErrors()) return response; 

            var allocationsBetweenDays = allocationRepository.GetBetweenDaysByEmployeeId(parameters.EmployeeId, parameters.DateSince.Value, parameters.DateTo.Value);

            if(allocationsBetweenDays.Count > 0)
            {
                AllocationValidationHelper.ValidatePercentageRange(response, allocationsBetweenDays, parameters);

                if (response.HasErrors()) return response;

                SaveAllocation(parameters, response);
            }
            else
            {
                if (response.HasErrors()) return response;

                SaveAllocation(parameters, response);
            }

            return response;
        }

        public ICollection<Allocation> GetAllocations(int employeeId, DateTime startDate, DateTime endDate)
        {
            return allocationRepository.GetAllocationsForAnalyticDates(employeeId, startDate, endDate);
        }

        private void SaveAllocation(AllocationAsignmentParams parameters, Response response)
        {
            var allocation = new Allocation();

            try
            {
                allocation.AnalyticId = parameters.AnalyticId;
                allocation.StartDate = parameters.DateSince.Value;
                allocation.EndDate = parameters.DateTo.Value;
                allocation.Percentage = parameters.Percentage.Value;
                allocation.EmployeeId = parameters.EmployeeId;

                allocationRepository.Insert(allocation);
                allocationRepository.Save();

                response.Messages.Add(new Message(Resources.es.AllocationManagement.Allocation.Added, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }
        }
    }
}
