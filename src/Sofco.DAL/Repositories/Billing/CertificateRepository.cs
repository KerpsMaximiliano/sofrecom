using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.DTO;
using Sofco.Model.Models.Billing;

namespace Sofco.DAL.Repositories.Billing
{
    public class CertificateRepository : BaseRepository<Certificate>, ICertificateRepository
    {
        public CertificateRepository(SofcoContext context) : base(context)
        {
        }

        public Certificate GetById(int certificateId)
        {
            return context.Certificates.Include(x => x.File).SingleOrDefault(x => x.Id == certificateId);
        }

        public bool Exist(int id)
        {
            return context.Certificates.Any(x => x.Id == id);
        }

        public ICollection<Certificate> Search(SearchCertificateParams parameters)
        {
            IQueryable<Certificate> query = context.Certificates
                .Include(x => x.File);

            if (parameters != null)
            {
                if (!string.IsNullOrWhiteSpace(parameters.ClientId) && !parameters.ClientId.Equals("0"))
                    query = query.Where(x => x.ClientExternalId.Equals(parameters.ClientId));

                if (parameters.Year > 0)
                    query = query.Where(x => x.Year == parameters.Year);
            }

            return query.ToList();
        }
    }
}
