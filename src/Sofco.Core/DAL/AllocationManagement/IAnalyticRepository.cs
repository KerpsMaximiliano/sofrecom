﻿using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IAnalyticRepository : IBaseRepository<Analytic>
    {
        bool Exist(int id);

        IList<Allocation> GetTimelineResources(int id, DateTime startDate, DateTime endDate);
        IList<Employee> GetResources(int id);

        Analytic GetLastAnalytic(int costCenterId);

        bool ExistTitle(string analyticTitle);

        IList<Analytic> GetAnalyticsByEmployee(int employeeId);

        void Close(Analytic analytic);

        ICollection<Analytic> GetAllOpenReadOnly();

        Analytic GetByService(string serviceId);

        ICollection<Analytic> GetByClient(string clientId);

        Analytic GetById(int allocationAnalyticId);

        ICollection<Analytic> GetAnalyticsByManagers(int id);

        List<Analytic> GetByManagerId(int managerId);
    }
}
