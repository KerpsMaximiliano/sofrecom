﻿using System;
using Sofco.Domain.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sofco.Core.Models;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Models.Billing;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IAnalyticService
    {
        ICollection<Analytic> GetAll();

        Response<Analytic> GetById(int id);

        Response<IList<Allocation>> GetTimelineResources(int id, DateTime dateSince, int months);

        AnalyticOptions GetOptions();

        Task<Response<Analytic>> Add(Analytic analytic);

        Response<string> GetNewTitle(int costCenterId);

        Response<Analytic> Update(Analytic domain);

        Response Close(int analyticId, AnalyticStatus status);

        ICollection<Analytic> GetAllActives();

        ICollection<AnalyticOptionForOcModel> GetByClient(string clientId);

        IList<Option> GetResources(int id);

        Response<List<Option>> GetByCurrentUser();

        Response<List<AnalyticSearchViewModel>> Get(AnalyticSearchParameters searchParameters);

        Response<byte[]> CreateReport(List<int> analytics);

        Response UpdateDaf(Analytic analytic);

        Response<IList<SelectListModel>> GetOpportunities(int id);

        Response<List<Option>> GetByCurrentManager();
    }
}
