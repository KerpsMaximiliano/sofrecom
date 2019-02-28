using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Relationships;

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
                    query = query.Where(x => x.AccountId.Equals(parameters.ClientId));

                if (parameters.Year > 0)
                    query = query.Where(x => x.Year == parameters.Year);
            }

            return query.ToList();
        }

        public ICollection<Certificate> GetByClients(string client)
        {
            return context.Certificates.Where(x => x.AccountId.Equals(client)).ToList();
        }

        public void RelateToSolfac(SolfacCertificate solfacCertificate)
        {
            context.SolfacCertificates.Add(solfacCertificate);
        }

        public ICollection<SolfacCertificate> GetBySolfacs(int id)
        {
            return context.SolfacCertificates
                .Include(x => x.Certificate)
                .ThenInclude(x => x.File)
                .Where(x => x.SolfacId == id)
                .ToList();
        }
    }
}
