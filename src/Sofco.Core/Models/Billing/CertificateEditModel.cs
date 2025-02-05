﻿using System;
using Sofco.Domain.Models.Billing;

namespace Sofco.Core.Models.Billing
{
    public class CertificateEditModel
    {
        public CertificateEditModel()
        {
        }

        public CertificateEditModel(Certificate domain)
        {
            Id = domain.Id;
            Name = domain.Name;
            ClientExternalId = domain.AccountId;
            ClientExternalName = domain.AccountName;
            Year = domain.Year;

            if (domain.File != null)
            {
                FileId = domain.FileId.GetValueOrDefault();
                FileName = domain.File.FileName;
                CreationDate = domain.File.CreationDate.ToString("d");
            }
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string ClientExternalId { get; set; }

        public string ClientExternalName { get; set; }

        public int Year { get; set; }

        public int? FileId { get; set; }

        public string FileName { get; set; }

        public string CreationDate { get; set; }

        public Certificate CreateDomain(string userName)
        {
            var domain = new Certificate();

            domain.Id = Id;
            domain.Name = Name;
            domain.AccountId = ClientExternalId;
            domain.AccountName = ClientExternalName;
            domain.Year = Year;

            if (FileId > 0)
                domain.FileId = FileId;

            domain.UpdateDate = DateTime.UtcNow;
            domain.UpdateByUser = userName;

            return domain;
        }
    }
}
