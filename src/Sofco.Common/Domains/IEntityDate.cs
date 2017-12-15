using System;

namespace Sofco.Common.Domains
{
    public interface IEntityDate
    {
        DateTime? Created { get; set; }

        DateTime? Modified { get; set; }
    }
}
