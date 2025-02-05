﻿using System.Linq;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Relationships;

namespace Sofco.DAL.Repositories.Billing
{
    public class SolfacCertificateRepository : BaseRepository<SolfacCertificate>, ISolfacCertificateRepository
    {
        public SolfacCertificateRepository(SofcoContext context) : base(context)
        {
        }

        public bool HasCertificates(int solfacId)
        {
            return context.SolfacCertificates.Any(x => x.SolfacId == solfacId);
        }

        public bool Exist(int id, int certificateId)
        {
            return context.SolfacCertificates.Any(x => x.SolfacId == id && x.CertificateId == certificateId);
        }
    }
}
