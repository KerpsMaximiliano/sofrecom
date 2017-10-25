using System.Collections.Generic;
using Sofco.Domain.Rh.Rhpro;

namespace Sofco.Repository.Rh.Repositories.Interfaces
{
    public interface IRhproEmployeeRepository
    {
        IList<RhproEmployee> GetAll();

        IList<RhproEmployeeLicense> GetLicenses();

        IList<RhproLicenseType> GetLicenseTypes();
    }
}
