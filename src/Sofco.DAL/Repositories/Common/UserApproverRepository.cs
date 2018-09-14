using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Common;

namespace Sofco.DAL.Repositories.Common
{
    public class UserApproverRepository : BaseRepository<UserApprover>, IUserApproverRepository
    {
        private DbSet<UserApprover> UserApproverSet { get; }

        public UserApproverRepository(SofcoContext context) : base(context)
        {
            UserApproverSet = context.Set<UserApprover>();
        }

        public void Save(List<UserApprover> userApprovers)
        {
            foreach (var item in userApprovers)
            {
                Save(item);
            }
        }

        public void Delete(int userApproverId)
        {
            var item = new UserApprover { Id = userApproverId };

            context.Entry(item).State = EntityState.Deleted;

            context.SaveChanges();
        }

        public List<UserApprover> GetByAnalyticId(int analyticId, UserApproverType type)
        {
            return UserApproverSet
                .Where(s => s.AnalyticId == analyticId
                    && s.Type == type)
                .ToList();
        }

        public List<Analytic> GetByAnalyticApprover(int currentUserId, UserApproverType type)
        {
            return UserApproverSet
                    .Include(x => x.Analytic)
                    .Where(x => x.ApproverUserId == currentUserId
                        && x.Analytic.Status == AnalyticStatus.Open
                        && x.Type == type)
                .Select(x => x.Analytic)
                .Distinct().ToList();
        }

        public IList<User> GetApproverByUserId(int userId, UserApproverType type)
        {
            return UserApproverSet
                .Include(x => x.ApproverUser)
                .Where(x => x.UserId == userId && x.Type == type)
                .Distinct()
                .Select(x => x.ApproverUser)
                .ToList();
        }

        public List<UserApprover> GetByApproverUserId(int approverUserId, UserApproverType type)
        {
            return UserApproverSet
                .Where(x => x.ApproverUserId == approverUserId && x.Type == type)
                .Distinct()
                .ToList();
        }

        private void Save(UserApprover item)
        {
            var storedItem = GetUnique(item.AnalyticId, item.EmployeeId, item.Type);

            if (storedItem != null)
            {
                Update(storedItem, item);
            }
            else
            {
                Insert(item);
            }

            context.SaveChanges();
        }

        private UserApprover GetUnique(int analyticId, int employeeId, UserApproverType type)
        {
            return UserApproverSet
                .SingleOrDefault(s => s.AnalyticId == analyticId
                                      && s.EmployeeId == employeeId
                                      && s.Type == type);
        }

        private void Update(UserApprover stored, UserApprover data)
        {
            stored.ApproverUserId = data.ApproverUserId;

            Update(stored);
        }
    }
}
