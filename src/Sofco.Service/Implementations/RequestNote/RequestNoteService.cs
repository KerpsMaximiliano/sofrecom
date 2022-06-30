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

            requestNote.StatusId = (int)RequestNoteStates.Rechazada;

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

        public void ChangeStatus(RequestNoteModel requestNote, RequestNoteStates status)
        {
            Domain.Models.RequestNote.RequestNote req = this.unitOfWork.RequestNoteRepository.GetById(requestNote.Id.Value);
            var user = userData.GetCurrentUser();
            int nuevoEstado = (int)status; 
            switch (status)
            {
                case RequestNoteStates.Borrador:
                    break;
                case RequestNoteStates.PendienteRevisiónAbastecimiento:
                    //Se marcan las analíticas del gerente logueado como “Rechazada”.
                    //Se cambia el estado de la requestNote a “Pendiente Revisión Abastecimiento”
                    //sin importar el estado de las demás analíticas.
                    foreach (var analitica in req.Analytics.Where(a => a.Analytic.ManagerId == user.Id))
                    {
                        analitica.Status = "Rechazada";
                    }
                    break;
                case RequestNoteStates.PendienteAprobaciónGerentesAnalítica:
                    //Se manda una lista de providers(como en la instancia borrador), deben reemplazar a los ya existentes
                    //en la requestNote.Se manda un campo monto final OC(number) para agregarse a la requestNote.
                    //Se debe asignar el estado de todas las analíticas asociadas a la requestNote como “Pendiente Aprobación”.
                    req.PurchaseOrderAmmount = requestNote.PurchaseOrderAmmount;
                    foreach (var analitica in req.Analytics)
                    {
                        analitica.Status = "Pendiente Aprobación";
                    }
                    break;
                case RequestNoteStates.PendienteAprobaciónAbastecimiento:
                    if (req.StatusId == (int)RequestNoteStates.PendienteAprobaciónGerentesAnalítica)
                    {
                        //Se deben marcar todas las analíticas del gerente asociado como “Aprobada” 
                        //(son suyas si el userId es el managerId de la analítica)
                        //Si todas las analíticas asociadas a la requestNote ya están con estado “Aprobada”,
                        //entonces cambia el estado de la requestNote a “Pendiente Aprobación Abastecimiento”,
                        //de lo contrario el estado de la requestNote no cambia
                        foreach (var analitica in req.Analytics.Where(a => a.Analytic.ManagerId == user.Id))
                        {
                            analitica.Status = "Aprobada";
                        }
                        if (req.Analytics.Any(a => a.Status != "Aprobada"))
                            nuevoEstado = req.StatusId; //Si alguna falta, dejo el estado como ya estaba
                    }
                    else if (req.StatusId == (int)RequestNoteStates.PendienteAprobaciónDAF)
                    {
                        //Se cambia el estado de la requestNote a “Pendiente Aprobación Abastecimiento”.
                        //Se manda un campo (string) con comentario para guardar en histories.

                    }
                    break;
                case RequestNoteStates.PendienteAprobaciónDAF:
                    //Se manda un provider que va a quedar como seleccionado, de ahora en adelante es
                    //el único que se tiene que devolver (sino puede traer todos pero que tenga un campo
                    //que lo identifique para solo mostrar ese).
                    //Se manda un campo numeroOC y un fileId para la orden de compra. 
                    req.PurchaseOrderNumber = requestNote.PurchaseOrderNumber;
                    var provider = req.Providers.SingleOrDefault(p => p.ProviderId == requestNote.ProviderSelectedId);
                    //Agregar campo en la tabla y ponerlo selected

                    if (requestNote.Attachments != null && requestNote.Attachments.Any(f => f.FileId.HasValue))
                        req.Attachments.Add(requestNote.Attachments.Where(f => f.FileId.HasValue).Select(p => new RequestNoteFile()
                        {
                            Type = (int) RequestNoteFileTypes.OrdenDeCompra,
                            FileId = p.FileId.Value
                        }).First());
                    break;
                case RequestNoteStates.Aprobada:
                    break;
                case RequestNoteStates.SolicitadaAProveedor:
                    //Se manda un array con los fileIds de los archivos subidos, son documentación para proveedor.
                    //Se cambia el estado a “Solicitada a Proveedor”.
                    //Dice que se tiene que notificar al proveedor seleccionado
                    if (requestNote.Attachments != null)
                        foreach (var file in requestNote.Attachments.Where(f => f.FileId.HasValue))
                        {
                            req.Attachments.Add(new RequestNoteFile()
                            {
                                Type = (int)RequestNoteFileTypes.DocumentacionParaProveedor,
                                FileId = file.FileId.Value
                            });
                        }
                    break;
                case RequestNoteStates.RecibidoConforme:
                    //Se manda un array con los fileIds de los archivos subidos, son documentación recibido conforme,
                    //puede ser null ya que es posible no subir archivos en esta instancia.
                    //Se cambia el estado de la requestNote a “Recibido Conforme”.
                    if (requestNote.Attachments != null)
                        foreach (var file in requestNote.Attachments.Where(f => f.FileId.HasValue))
                        {
                            req.Attachments.Add(new RequestNoteFile()
                            {
                                Type = (int)RequestNoteFileTypes.DocumentacionRecibidoConforme,
                                FileId = file.FileId.Value
                            });
                        }
                    break;
                case RequestNoteStates.FacturaPendienteAprobaciónGerente:
                    //Se manda un array de objetos, cada objeto tiene un fileId y un campo (es funcionalidad a futuro,
                    //el campo va a mandar siempre null pero creería que va a terminar mandando string después), estos son facturas.
                    //Se cambia el estado de la requestNote a “Factura Pendiente Aprobación Gerente”.
                    if (requestNote.Attachments != null)
                        foreach (var file in requestNote.Attachments.Where(f => f.FileId.HasValue))
                        {
                            req.Attachments.Add(new RequestNoteFile()
                            {
                                Type = (int)RequestNoteFileTypes.Facturas,
                                FileId = file.FileId.Value
                            });
                        }
                        
                    break;
                case RequestNoteStates.PendienteProcesarGAF:
                    //Aprobar → Se manda un array de analíticas. Dichas analíticas pasan al estado “Aprobada Facturación”.
                    //Si todas las analíticas de la requestNote están con ese estado, se cambia el estado de la requestNote
                    //a “Pendiente Procesar GAF”.
                    foreach (var analitica in req.Analytics.Where(a => a.Analytic.ManagerId == user.Id))
                    {
                        analitica.Status = "Aprobada Facturación";
                    }
                    if (req.Analytics.Any(a => a.Status != "Aprobada Facturación"))
                        nuevoEstado = req.StatusId; //Si alguna falta, dejo el estado como ya estaba

                    break;
                case RequestNoteStates.Rechazada:
                    break;
                case RequestNoteStates.Cerrada:
                    //Se cambia el estado de la requestNote a  “Cerrada”. Se manda un campo (string) con comentario para guardar en histories.

                    break;
                default:
                    break;
            }
            if(req.StatusId != nuevoEstado) //Si cambia el estado, guardamos historial
            {
                if (req.Histories == null)
                    req.Histories = new List<RequestNoteHistory>();

                req.Histories.Add(new RequestNoteHistory()
                {
                    Comment = requestNote.Comments,
                    CreatedDate = DateTime.UtcNow,
                    StatusFromId = req.StatusId,
                    StatusToId = nuevoEstado,
                    UserName = user.UserName
                });
            }
            req.StatusId = nuevoEstado;

            this.unitOfWork.RequestNoteRepository.UpdateRequestNote(req);
            this.unitOfWork.RequestNoteRepository.Save();
        }
    }
}
