using System.Collections.Generic;
using Sofco.Domain.Utils;

namespace Sofco.Core.Managers
{
    public interface IGenericOptionManager
    {
        void SetParameters(Option domain, Dictionary<string, string> parameters);
    }
}
