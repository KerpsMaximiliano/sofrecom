using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class HealthInsuranceRepository : BaseRepository<HealthInsurance>, IHealthInsuranceRepository
    {
        public HealthInsuranceRepository(SofcoContext context) : base(context)
        {
        }

        public void Save(List<HealthInsurance> healthInsurances)
        {
            var storedItems = GetByCode(healthInsurances.Select(s => s.Code).ToArray());

            var storedCodes = storedItems.Select(s => s.Code).ToList();

            foreach (var item in healthInsurances)
            {
                if (storedCodes.Contains(item.Code))
                {
                    var updateItem = storedItems
                        .First(s => s.Code == item.Code);

                    Update(updateItem, item);
                }
                else
                {
                    Insert(item);
                }
            }

            context.SaveChanges();
        }

        public HealthInsurance GetByCode(int code)
        {
            return context.HealthInsurances.FirstOrDefault(s => s.Code == code);
        }

        private List<HealthInsurance> GetByCode(int[] codes)
        {
            return context.HealthInsurances
                .Where(s => codes.Contains(s.Code))
                .ToList();
        }

        private void Update(HealthInsurance stored, HealthInsurance data)
        {
            stored.Name = data.Name;

            Update(stored);
        }
    }
}
