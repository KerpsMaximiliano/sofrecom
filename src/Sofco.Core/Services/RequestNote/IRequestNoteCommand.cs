using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.Recruitment;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.RequestNote
{
    public interface IRequestNoteCommand<T> where T : RequestNoteSubmitDTO
    {
        Response CanExecute(T dto);
        Response Validate(T dto);
        Response Execute(T dto);
        Response Notify(T dto);
        
        
    }
}
