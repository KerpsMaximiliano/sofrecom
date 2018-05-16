using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Domain.Nolaborables;

namespace Sofco.Framework.NolaborablesServices.Interfaces
{
    public interface INolaborablesService
    {
        Result<List<Feriado>> Get(int year);
    }
}