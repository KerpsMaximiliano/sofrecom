using System.Collections.Generic;
using Sofco.Core.Config;
using Sofco.Model.DTO;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface ISolfacService
    {
        Response<Solfac> Add(Solfac solfac);
        IList<Solfac> Search(SolfacParams parameter, string userMail);
        IList<Hito> GetHitosByProject(string projectId);
        IList<Solfac> GetByProject(string projectId);
        Response<Solfac> GetById(int id);
        Response ChangeStatus(Solfac solfac, SolfacStatusParams parameters, EmailConfig emailConfig);
        Response ChangeStatus(int solfacId, SolfacStatusParams parameters, EmailConfig emailConfig);
        Response Delete(int id);
        ICollection<SolfacHistory> GetHistories(int id);
        Response Update(Solfac domain, string modelComments);
        Response<SolfacAttachment> SaveFile(int solfacId, byte[] fileAsArrayBytes, string fileFileName);
        ICollection<SolfacAttachment> GetFiles(int solfacId);
        Response<SolfacAttachment> GetFileById(int fileId);
        Response DeleteFile(int id);
    }
}
