using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.DTO;
using Sofco.Model.Models.Billing;

namespace Sofco.DAL.Repositories.Billing
{
    public class SolfacRepository : BaseRepository<Solfac>, ISolfacRepository
    {
        public SolfacRepository(SofcoContext context) : base(context)
        {
        }

        public IList<Solfac> GetAllWithDocuments()
        {
            return _context.Solfacs.Include(x => x.DocumentType).ToList();
        }

        public IList<Hito> GetHitosByProject(string projectId)
        {
            return _context.Hitos.Where(x => x.ExternalProjectId.Equals(projectId)).ToList();
        }

        public IList<Solfac> GetByProject(string projectId)
        {
            return _context.Solfacs.Where(x => x.ProjectId == projectId).Include(x => x.DocumentType).ToList();
        }

        public Solfac GetById(int id)
        {
            return _context.Solfacs
                .Include(x => x.DocumentType)
                .Include(x => x.Currency)
                .Include(x => x.UserApplicant)
                .Include(x => x.ImputationNumber)
                .Include(x => x.Hitos)
                .Include(x => x.Invoice)
                .SingleOrDefault(x => x.Id == id);
        }

        public IList<Solfac> SearchByParams(SolfacParams parameters)
        {
            IQueryable<Solfac> query = _context.Solfacs;

            if (!string.IsNullOrWhiteSpace(parameters.CustomerId) && !parameters.CustomerId.Equals("0"))
                query = query.Where(x => x.CustomerId == parameters.CustomerId);

            if (!string.IsNullOrWhiteSpace(parameters.ServiceId) && !parameters.ServiceId.Equals("0"))
                query = query.Where(x => x.ServiceId == parameters.ServiceId);

            if (!string.IsNullOrWhiteSpace(parameters.ProjectId) && !parameters.ProjectId.Equals("0"))
                query = query.Where(x => x.ProjectId == parameters.ProjectId);

            if (!string.IsNullOrWhiteSpace(parameters.Analytic))
                query = query.Where(x => x.Analytic.ToLowerInvariant().Equals(parameters.Analytic.ToLowerInvariant()));

            if (parameters.UserApplicantId > 0)
                query = query.Where(x => x.UserApplicantId == parameters.UserApplicantId);

            return query.Include(x => x.DocumentType).ToList();
        }
    }
}
