using System.Threading.Tasks;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IHitoService
    {
        Response Close(string id);
    }
}
