﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Common;
using Sofco.Core.Managers;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;

namespace Sofco.Framework.Managers
{
    public class ApproverEmployeeManager : IApproverEmployeeManager
    {
        private readonly IUserApproverEmployeeRepository repository;

        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly IAnalyticData analyticData;

        private readonly ISessionManager sessionManager;

        private Dictionary<UserApproverType, Func<UserApproverQuery, List<UserApproverEmployee>>> CurrentDirectorDicts;

        public ApproverEmployeeManager(ISessionManager sessionManager, IUserApproverEmployeeRepository repository, IUnitOfWork unitOfWork, IUserData userData, IAnalyticData analyticData)
        {
            this.sessionManager = sessionManager;
            this.repository = repository;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.analyticData = analyticData;
            SetCurrentDirectorDicts();
        }

        public List<UserApproverEmployee> GetByCurrentServices(UserApproverQuery query, UserApproverType type)
        {
            var result = new List<UserApproverEmployee>();

            if (IsDirector())
            {
                return CurrentDirectorDicts.ContainsKey(type) ? CurrentDirectorDicts[type](query) : result;
            }

            var userName = sessionManager.GetUserName();

            var isManager = unitOfWork.UserRepository.HasManagerGroup(userName);

            if (!isManager) return new List<UserApproverEmployee>();

            List<UserApproverEmployee> analytics;

            if (query.AnalyticId > 0)
            {
                analytics = repository.Get(query, type);
            }
            else
            {
                var currentUser = userData.GetCurrentUser();

                var currentAnalyticIds = unitOfWork.AnalyticRepository.GetAnalyticsByManagerId(currentUser.Id).Select(x => x.Id)
                    .ToList();

                analytics = repository.GetByAnalytics(currentAnalyticIds, query.ApprovalId, type);
            }

            result = ResolveApprovalUser(analytics);

            return ResolveManagers(result);
        }

        private bool IsDirector()
        {
            var currentUser = userData.GetCurrentUser();

            var isDirector = unitOfWork.UserRepository.HasDirectorGroup(currentUser.Email) 
                || unitOfWork.SectorRepository.HasSector(currentUser.Id);

            return isDirector;
        }

        private void SetCurrentDirectorDicts()
        {
            CurrentDirectorDicts =
                new Dictionary<UserApproverType, Func<UserApproverQuery, List<UserApproverEmployee>>>
                {
                    {UserApproverType.License, GetByCurrentSectorsLicense},
                    {UserApproverType.WorkTime, GetByCurrentSectorsWorktime}
                };
        }

        private List<UserApproverEmployee> GetByCurrentSectorsLicense(UserApproverQuery query)
        {
            var currentUser = userData.GetCurrentUser();

            var sectorIds = unitOfWork.SectorRepository.GetByUserId(currentUser.Id).Select(s => s.Id).ToList();

            var result = ResolveApprovalUser(repository.GetManagersBySectors(sectorIds, UserApproverType.License));

            return ResolveManagers(result);
        }

        private List<UserApproverEmployee> GetByCurrentSectorsWorktime(UserApproverQuery query)
        {
            var result = ResolveApprovalUser(repository.Get(query, UserApproverType.WorkTime));

            return ResolveManagers(result);
        }

        private List<UserApproverEmployee> ResolveManagers(List<UserApproverEmployee> data)
        {
            var managerIds = data.Where(s => s.ManagerId.HasValue).Select(s => s.ManagerId.Value).Distinct().ToList();

            var managers = managerIds.Select(managerId => userData.GetUserLiteById(managerId)).ToList();

            foreach (var item in data)
            {
                var manager = managers.FirstOrDefault(s => s.Id == item.ManagerId);
                if(manager == null) continue;
                item.Manager = manager.Name;
            }

            return data;
        }

        private List<UserApproverEmployee> ResolveApprovalUser(List<UserApproverEmployee> data)
        {
            var userIds = data.Where(s => s.UserApprover != null).Select(s => s.UserApprover.ApproverUserId).Distinct().ToList();

            var userNames = userIds.Select(x => userData.GetUserLiteById(x)).ToList();

            foreach (var item in data.Where(s => s.UserApprover != null))
            {
                var approvalUser = userNames.FirstOrDefault(s => s.Id == item.UserApprover.ApproverUserId);
                if (approvalUser == null) continue;
                item.ApprovalName = approvalUser.Name;
            }

            data = ResolveAnalytics(data);

            return data;
        }

        private List<UserApproverEmployee> ResolveAnalytics(List<UserApproverEmployee> data)
        {
            var analyticsIds = data.Where(s => s.AnalyticId != null).Select(s => s.AnalyticId.Value).Distinct().ToList();

            var analytics = analyticsIds.Select(x => analyticData.GetLiteById(x)).ToList();

            foreach (var item in data.Where(s => s.AnalyticId != null))
            {
                var analytic = analytics.FirstOrDefault(s => s.Id == item.AnalyticId);
                if (analytic == null) continue;
                item.Analytic = $"{analytic.Title} - {analytic.Name}";
            }

            return data;
        }
    }
}
