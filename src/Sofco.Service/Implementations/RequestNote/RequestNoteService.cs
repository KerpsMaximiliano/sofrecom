using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Data.Admin;
using Sofco.Core.Logger;
using Sofco.Core.Models.RequestNote;
using Sofco.Core.Services.Common;
using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Models.RequestNote;
using Sofco.Domain.Models.Workflow;
using Sofco.Domain.RequestNoteStates;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Sofco.Domain.Models.Common.File;

namespace Sofco.Service.Implementations.RequestNote
{
    public class RequestNoteService : IRequestNoteService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<RequestNoteService> logger;
        private readonly IUserData userData;
        private readonly FileConfig fileConfig;
        private readonly IFileService fileService;
        public RequestNoteService(IUnitOfWork unitOfWork, ILogMailer<RequestNoteService> logger, IUserData userData,
            IFileService fileService, IOptions<FileConfig> fileOptions)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.userData = userData;
            fileConfig = fileOptions.Value;
            this.fileService = fileService;
        }

        public Response SaveBorrador(RequestNoteSubmitDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response SavePendienteRevisionAbastecimiento(RequestNoteSubmitDTO dto)
        {
            throw new NotImplementedException();
        }


        public void RechazarRequestNote(int requestNodeId)
        {
            Domain.Models.RequestNote.RequestNote requestNote = this.unitOfWork.RequestNoteRepository.GetById(requestNodeId);

            requestNote.StatusId = (int)RequestNoteStates.Reachazada;

            this.unitOfWork.RequestNoteRepository.UpdateRequestNote(requestNote);
            this.unitOfWork.RequestNoteRepository.Save();
        }

        public void CambiarAPendienteApobacionGerenteAnalitica(int requestNodeId)
        {
            Domain.Models.RequestNote.RequestNote requestNote = this.unitOfWork.RequestNoteRepository.GetById(requestNodeId);
            requestNote.StatusId = (int)RequestNoteStates.PendienteAprobaciónGerentesAnalítica;

            this.unitOfWork.RequestNoteRepository.UpdateRequestNote(requestNote);
            this.unitOfWork.RequestNoteRepository.Save();
        }

        public Response<int> GuardarBorrador(RequestNoteModel requestNoteBorrador)
        {/*
            requestNoteBorrador.StatusId = (int)RequestNoteStates.Borrador;
            requestNoteBorrador.CreationDate = DateTime.Now;

            this.unitOfWork.RequestNoteRepository.InsertRequestNote(requestNoteBorrador);
            this.unitOfWork.RequestNoteRepository.Save();*/
            //TODO: Validar
            var response = new Response<int>();
            if (requestNoteBorrador == null)
            {
                response.AddError(Resources.Recruitment.Applicant.ModelNull);
                return response;
            }

            var user = userData.GetCurrentUser();

            var domain = new Domain.Models.RequestNote.RequestNote()
            {
                Description = requestNoteBorrador.Description,
                RequiresEmployeeClient = requestNoteBorrador.RequiresEmployeeClient,
                ConsideredInBudget = requestNoteBorrador.ConsideredInBudget,
                EvalpropNumber = requestNoteBorrador.EvalpropNumber,
                Comments = requestNoteBorrador.Comments,
                TravelSection = requestNoteBorrador.TravelSection,
                TrainingSection = requestNoteBorrador.TrainingSection,
                //PurchaseOrderAmmount = 
                //PurchaseOrderNumber
                CreationDate = DateTime.UtcNow,
                WorkflowId = requestNoteBorrador.WorkflowId,
                StatusId = (int)RequestNoteStates.Borrador,
                UserApplicantId = requestNoteBorrador.UserApplicantId,
                InWorkflowProcess = true,
                CreationUserId = user.Id,
                ProviderAreaId = requestNoteBorrador.ProviderAreaId
            };
            if (requestNoteBorrador.Providers != null)
                domain.Providers = requestNoteBorrador.Providers.Select(p => new RequestNoteProvider()
                {
                    ProviderId = p.ProviderId,
                    FileId = p.FileId
                }).ToList();
            if (requestNoteBorrador.Attachments != null)
                domain.Attachments = requestNoteBorrador.Attachments.Where(f=> f.FileId.HasValue).Select(p => new RequestNoteFile()
                {
                    Type = 1, //Poner enum
                    FileId = p.FileId.Value
                }).ToList();
            if (requestNoteBorrador.Analytics != null)
                domain.Analytics = requestNoteBorrador.Analytics.Select(p => new RequestNoteAnalytic()
                {
                    AnalyticId = p.AnalyticId,
                    Percentage = p.Asigned,
                    Status = "Ninguno"
                }).ToList();
            if (requestNoteBorrador.ProductsServices != null)
                domain.ProductsServices = requestNoteBorrador.ProductsServices.Select(p => new RequestNoteProductService()
                {
                    ProductService = p.ProductService,
                    Quantity = p.Quantity
                }).ToList();
            if (requestNoteBorrador.Travel != null)
            {
                domain.Travels = new List<RequestNoteTravel>();
                domain.Travels.Add(new RequestNoteTravel()
                {
                    Accommodation = requestNoteBorrador.Travel.Accommodation,
                    Conveyance = requestNoteBorrador.Travel.Transportation,
                    DepartureDate = requestNoteBorrador.Travel.DepartureDate,
                    Destination = requestNoteBorrador.Travel.Destination,
                    ItineraryDetail = requestNoteBorrador.Travel.Details,
                    ReturnDate = requestNoteBorrador.Travel.ReturnDate,
                    Employees = requestNoteBorrador.Travel.Passengers?.Select(p => new RequestNoteTravelEmployee()
                    {
                        EmployeeId = p.EmployeeId
                    }).ToList()
                });
            }
            if (requestNoteBorrador.Training != null)
            {
                domain.Trainings = new List<RequestNoteTraining>();
                domain.Trainings.Add(new RequestNoteTraining()
                {
                    Duration = requestNoteBorrador.Training.Duration,
                    TrainingDate = requestNoteBorrador.Training.Date,
                    Ammount = requestNoteBorrador.Training.Ammount,
                    Place = requestNoteBorrador.Training.Location,
                    Subject = requestNoteBorrador.Training.Subject,
                    Topic = requestNoteBorrador.Training.Name,
                    Employees = requestNoteBorrador.Training.Participants?.Select(p => new RequestNoteTrainingEmployee()
                    {
                        EmployeeId = p.EmployeeId
                    }).ToList()
                });
               
            }
            
            this.unitOfWork.RequestNoteRepository.InsertRequestNote(domain);
            this.unitOfWork.RequestNoteRepository.Save();
            response.Data = domain.Id;

            response.AddSuccess(Resources.Recruitment.Applicant.AddSuccess);
            return response;
        }
        public Response<RequestNoteModel> GetById(int id)
        {
            var response = new Response<RequestNoteModel>();

            var note = this.unitOfWork.RequestNoteRepository.GetById(id);
            if (note == null)
            {
                response.AddError(Resources.AdvancementAndRefund.Refund.NotFound);
                return response;
            }
            response.Data = new RequestNoteModel(note);

            return response;
        }
        public async Task<Response<List<File>>> AttachFiles(Response<List<File>> response, List<IFormFile> files)
        {
            var user = userData.GetCurrentUser();

            if (response.HasErrors()) return response;
            response.Data = new List<File>();
            foreach (var file in files)
            {

                var fileToAdd = new File();
                var lastDotIndex = file.FileName.LastIndexOf('.');

                fileToAdd.FileName = file.FileName;
                fileToAdd.FileType = file.FileName.Substring(lastDotIndex);
                fileToAdd.InternalFileName = Guid.NewGuid();
                fileToAdd.CreationDate = DateTime.UtcNow;
                fileToAdd.CreatedUser = user.UserName;

                var path = fileConfig.RequestNotePath;
                var successMsg = Resources.RequestNote.RequestNote.FileUpload;

                if (string.IsNullOrWhiteSpace(path)) return response;

                try
                {
                    var fileName = $"{fileToAdd.InternalFileName.ToString()}{fileToAdd.FileType}";

                    using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    unitOfWork.FileRepository.Insert(fileToAdd);
                    //unitOfWork.InvoiceRepository.Update(reqN);
                    unitOfWork.Save();

                    response.Data.Add(fileToAdd);
                    response.AddSuccess(successMsg);

                }
                catch (Exception e)
                {
                    response.AddError(Resources.Common.SaveFileError);
                    logger.LogError(e);
                }
            }
            return response;
        }
        public IList<Domain.Models.RequestNote.RequestNote> GetAll()
        {
            return this.unitOfWork.RequestNoteRepository.GetAll();
        }

        public void ChangeStatus(int requestNodeId, RequestNoteStates requestNoteStates)
        {
            Domain.Models.RequestNote.RequestNote requestNote = this.unitOfWork.RequestNoteRepository.GetById(requestNodeId);

            requestNote.StatusId = (int)requestNoteStates;

            this.unitOfWork.RequestNoteRepository.UpdateRequestNote(requestNote);
            this.unitOfWork.RequestNoteRepository.Save();
        }
    }
}
