using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Billing;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;
using File = Sofco.Domain.Models.Common.File;

namespace Sofco.Service.Implementations.Billing
{
    public class CertificateService : ICertificateService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<CertificateService> logger;
        private readonly FileConfig fileConfig;

        public CertificateService(IUnitOfWork unitOfWork, ILogMailer<CertificateService> logger, IOptions<FileConfig> fileOptions)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.fileConfig = fileOptions.Value;
        }

        public Response<Certificate> Add(Certificate domain)
        {
            var response = new Response<Certificate>();

            Validate(domain, response);

            if (response.HasErrors()) return response;

            try
            {
                unitOfWork.CertificateRepository.Insert(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.Billing.Certificate.SaveSuccess);

                response.Data = domain;
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
            }

            return response;
        }

        public async Task<Response<File>> AttachFile(int certificateId, Response<File> response, IFormFile file, string userName)
        {
            var certificate = CertificateValidationHandler.Find(certificateId, response, unitOfWork);

            if (response.HasErrors()) return response;

            var fileToAdd = new File();
            var lastDotIndex = file.FileName.LastIndexOf('.');

            fileToAdd.FileName = file.FileName;
            fileToAdd.FileType = file.FileName.Substring(lastDotIndex);
            fileToAdd.InternalFileName = Guid.NewGuid();
            fileToAdd.CreationDate = DateTime.UtcNow;
            fileToAdd.CreatedUser = userName;

            certificate.File = fileToAdd;

            try
            {
                var fileName = $"{fileToAdd.InternalFileName.ToString()}{fileToAdd.FileType}";

                using (var fileStream = new FileStream(Path.Combine(fileConfig.CertificatesPath, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                unitOfWork.FileRepository.Insert(fileToAdd);
                unitOfWork.CertificateRepository.Update(certificate);
                unitOfWork.Save();

                response.Data = fileToAdd;
                response.AddSuccess(Resources.Billing.Certificate.FileAdded);
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.SaveFileError);
                logger.LogError(e);
            }

            return response;
        }

        public Response<Certificate> GetById(int id)
        {
            var response = new Response<Certificate>();

            response.Data = CertificateValidationHandler.Find(id, response, unitOfWork);

            return response;
        }

        public ICollection<Certificate> GetByClient(string client)
        {
            return unitOfWork.CertificateRepository.GetByClients(client);
        }

        public ICollection<SolfacCertificate> GetBySolfac(int id)
        {
            return unitOfWork.CertificateRepository.GetBySolfacs(id);
        }

        public Response Update(Certificate domain)
        {
            var response = new Response<Certificate>();

            CertificateValidationHandler.Exist(response, domain.Id, unitOfWork);

            if (response.HasErrors()) return response;

            Validate(domain, response);

            if (response.HasErrors()) return response;

            try
            {
                unitOfWork.CertificateRepository.Update(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.Billing.Certificate.Updated);

                response.Data = domain;
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
            }

            return response;
        }

        public ICollection<Certificate> Search(SearchCertificateParams parameters)
        {
            return unitOfWork.CertificateRepository.Search(parameters);
        }

        public Response DeleteFile(int id)
        {
            var response = new Response();

            var certificate = CertificateValidationHandler.Find(id, response, unitOfWork);

            if (certificate.File == null)
            {
                response.AddError(Resources.Common.FileNotFound);
                return response;
            }

            try
            {
                var file = certificate.File;
                certificate.File = null;
                certificate.FileId = null;

                unitOfWork.CertificateRepository.Update(certificate);
                unitOfWork.FileRepository.Delete(file);

                var fileName = $"{file.InternalFileName.ToString()}{file.FileType}";
                var path = Path.Combine(fileConfig.CertificatesPath, fileName);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                unitOfWork.Save();
                response.AddSuccess(Resources.Common.FileDeleted);
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.GeneralError);
                logger.LogError(e);
            }

            return response;
        }

        private void Validate(Certificate domain, Response<Certificate> response)
        {
            CertificateValidationHandler.ValidateName(response, domain);
            CertificateValidationHandler.ValidateClient(response, domain);
            CertificateValidationHandler.ValidateYear(response, domain);
        }
    }
}
