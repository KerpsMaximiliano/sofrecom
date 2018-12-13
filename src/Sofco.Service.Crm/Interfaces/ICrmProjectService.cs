using System.Collections.Generic;
using Sofco.Domain.Crm;

namespace Sofco.Service.Crm.Interfaces
{
    public interface ICrmProjectService
    {
        List<CrmProject> GetAll();
    }
}