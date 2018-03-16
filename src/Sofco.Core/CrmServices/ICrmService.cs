using System;
using Sofco.Common.Domains;

namespace Sofco.Core.CrmServices
{
    public interface ICrmService
    {
        Result<string> DesactivateService(Guid id);

        Result<string> ActivateService(Guid id);
    }
}