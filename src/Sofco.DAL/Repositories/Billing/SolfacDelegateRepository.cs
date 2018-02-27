using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.Billing;

namespace Sofco.DAL.Repositories.Billing
{
    public class SolfacDelegateRepository : BaseRepository<SolfacDelegate>, ISolfacDelegateRepository
    {
        private DbSet<SolfacDelegate> SolfacDelegateSet { get; }

        public SolfacDelegateRepository(SofcoContext context) : base(context)
        {
            SolfacDelegateSet = context.Set<SolfacDelegate>();
        }

        public new List<SolfacDelegate> GetAll()
        {
            return base.GetAll().ToList();
        }

        public SolfacDelegate Save(SolfacDelegate solfacDelegate)
        {
            var storedItem = GetByServiceIdAndUserId(solfacDelegate.ServiceId, solfacDelegate.UserId);

            if (storedItem != null)
            {
                Update(storedItem);
            }
            else
            {
                Insert(solfacDelegate);
            }

            context.SaveChanges();

            return solfacDelegate;
        }

        private SolfacDelegate GetByServiceIdAndUserId(Guid serviceId, int userId)
        {
            return SolfacDelegateSet
                .SingleOrDefault(s => s.ServiceId == serviceId && s.UserId == userId);
        }
    }
}
