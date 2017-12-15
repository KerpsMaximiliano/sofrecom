using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class LicenseTypeRepository : BaseRepository<LicenseType>, ILicenseTypeRepository
    {
        public LicenseTypeRepository(SofcoContext context) : base(context)
        {
        }

        public void Save(List<LicenseType> licenseTypes)
        {
            var storedItems = GetAll();

            var storedNumbers = storedItems.Select(s => s.LicenseTypeNumber).ToArray();

            foreach (var item in licenseTypes)
            {
                if(storedNumbers.Contains(item.LicenseTypeNumber))
                {
                    var updateItem = storedItems
                        .First(s => s.LicenseTypeNumber == item.LicenseTypeNumber);

                    Update(updateItem, item);
                } else
                {
                    Insert(item);
                }
            }

            context.SaveChanges();
        }

        private void Update(LicenseType storedData, LicenseType data)
        {
            storedData.Description = data.Description;

            Update(storedData);
        }
    }
}
