using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class PrepaidHealthRepository : BaseRepository<PrepaidHealth>, IPrepaidHealthRepository
    {
        public PrepaidHealthRepository(SofcoContext context) : base(context)
        {
        }

        public void Save(List<PrepaidHealth> prepaidHealths)
        {
            var storedItems = GetAll().ToList();

            foreach (var item in prepaidHealths)
            {
                var updateItem = GetItemToUpdate(storedItems, item);

                if (updateItem != null)
                {
                    Update(updateItem, item);
                }
                else
                {
                    Insert(item);
                }
            }

            context.SaveChanges();
        }

        private PrepaidHealth GetItemToUpdate(List<PrepaidHealth> storedItems, PrepaidHealth item)
        {
            return storedItems.FirstOrDefault(s =>
                s.HealthInsuranceCode == item.HealthInsuranceCode
                && s.PrepaidHealthCode == item.PrepaidHealthCode);
        }

        private void Update(PrepaidHealth stored, PrepaidHealth data)
        {
            stored.Name = data.Name;
            stored.Amount = data.Amount;

            Update(stored);
        }
    }
}
