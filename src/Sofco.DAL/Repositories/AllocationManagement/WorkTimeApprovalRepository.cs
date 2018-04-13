using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class WorkTimeApprovalRepository : BaseRepository<WorkTimeApproval>, IWorkTimeApprovalRepository
    {
        public WorkTimeApprovalRepository(SofcoContext context) : base(context)
        {
        }

        public new List<WorkTimeApproval> GetAll()
        {
            return base.GetAll().ToList();
        }

        public void Save(List<WorkTimeApproval> workTimeApprovals)
        {
            foreach (var item in workTimeApprovals)
            {
                Save(item);
            }
        }

        public void Delete(int workTimeApprovalId)
        {
            var item = new WorkTimeApproval { Id = workTimeApprovalId };

            context.Entry(item).State = EntityState.Deleted;

            context.SaveChanges();
        }

        public List<WorkTimeApproval> GetByServiceIds(List<Guid> serviceIds)
        {
            var result = context.WorkTimeApprovals
                .Where(s => serviceIds.Contains(s.ServiceId))
                .ToList();

            return result;
        }

        private void Save(WorkTimeApproval item)
        {
            var storedItem = GetUnique(item.ServiceId, item.UserId);

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

        private WorkTimeApproval GetUnique(Guid serviceId, int userId)
        {
            return context.WorkTimeApprovals
                .SingleOrDefault(s => s.ServiceId == serviceId
                                      && s.UserId == userId);
        }

        private void Update(WorkTimeApproval stored, WorkTimeApproval data)
        {
            stored.ApprovalUserId = data.ApprovalUserId;

            Update(stored);
        }
    }
}
