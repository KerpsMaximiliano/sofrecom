using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Workflow;
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
        private readonly AppSetting settings;
        private readonly IWorkflowStateRepository workflowStateRepository;
        public RequestNoteService(IUnitOfWork unitOfWork, ILogMailer<RequestNoteService> logger, IUserData userData,
            IFileService fileService, IOptions<FileConfig> fileOptions, IWorkflowStateRepository workflowStateRepository, IOptions<AppSetting> settingOptions)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.userData = userData;
            fileConfig = fileOptions.Value;
            this.fileService = fileService;
            this.settings = settingOptions.Value;

            this.workflowStateRepository = workflowStateRepository;
        }

        public void RechazarRequestNote(int requestNodeId)
        {
            Domain.Models.RequestNote.RequestNote requestNote = this.unitOfWork.RequestNoteRepository.GetById(requestNodeId);

            requestNote.StatusId = (int)RequestNoteStatus.Rechazada;

            this.unitOfWork.RequestNoteRepository.UpdateRequestNote(requestNote);
            this.unitOfWork.RequestNoteRepository.Save();
        }

        public void CambiarAPendienteApobacionGerenteAnalitica(int requestNodeId)
        {
            Domain.Models.RequestNote.RequestNote requestNote = this.unitOfWork.RequestNoteRepository.GetById(requestNodeId);
            requestNote.StatusId = (int)RequestNoteStatus.PendienteAprobaciónGerentesAnalítica;

            this.unitOfWork.RequestNoteRepository.UpdateRequestNote(requestNote);
            this.unitOfWork.RequestNoteRepository.Save();
        }

        public Response<int> Add(RequestNoteModel requestNoteBorrador)
        {

            var response = new Response<int>();
            if (requestNoteBorrador == null)
            {
                response.AddError(Resources.RequestNote.RequestNote.NullModel);
                return response;
            }

            var user = userData.GetCurrentUser();
            if (requestNoteBorrador.Id.HasValue)
            {
                var domain = this.unitOfWork.RequestNoteRepository.GetById(requestNoteBorrador.Id.Value);
                if (domain == null)
                {
                    response.AddError(Resources.RequestNote.RequestNote.NotFound);
                    return response;
                }
                if (domain.StatusId != settings.WorkflowStatusNPBorrador)
                {
                    response.AddError(Resources.RequestNote.RequestNote.NotFound);
                    return response;
                }


                domain.Description = requestNoteBorrador.Description;
                domain.RequiresEmployeeClient = requestNoteBorrador.RequiresEmployeeClient;
                domain.ConsideredInBudget = requestNoteBorrador.ConsideredInBudget;
                domain.EvalpropNumber = requestNoteBorrador.EvalpropNumber;
                domain.Comments = requestNoteBorrador.Comments;
                domain.TravelSection = requestNoteBorrador.Travel != null;
                domain.TrainingSection = requestNoteBorrador.Training != null;
                //domain.WorkflowId = requestNoteBorrador.WorkflowId;
                domain.UserApplicantId = requestNoteBorrador.UserApplicantId;
                domain.ProviderAreaId = requestNoteBorrador.ProviderAreaId;

                #region Providers
                if (requestNoteBorrador.Providers == null)
                    requestNoteBorrador.Providers = new List<Provider>();
                foreach (var prov in domain.ProvidersSugg.ToList())
                {
                    if (!requestNoteBorrador.Providers.Any(a => a.ProviderId == prov.ProviderId))
                        unitOfWork.RequestNoteProviderRepository.Delete(prov);
                }
                foreach (var provNuevo in requestNoteBorrador.Providers)
                {
                    var prov = domain.ProvidersSugg.SingleOrDefault(p => p.ProviderId == provNuevo.ProviderId);
                    if (prov == null)
                    {
                        prov = new RequestNoteProviderSugg() { ProviderId = provNuevo.ProviderId, FileId = provNuevo.FileId };
                        domain.ProvidersSugg.Add(prov);
                    }
                }
                #endregion
                #region Analytics
                if (requestNoteBorrador.Analytics == null)
                    requestNoteBorrador.Analytics = new List<Analytic>();
                foreach (var an in domain.Analytics.ToList())
                {
                    if (!requestNoteBorrador.Analytics.Any(a => a.AnalyticId == an.AnalyticId))
                        unitOfWork.RequestNoteAnalitycRepository.Delete(an);
                }
                foreach (var a in requestNoteBorrador.Analytics)
                {
                    var an = domain.Analytics.SingleOrDefault(p => p.AnalyticId == a.AnalyticId);
                    if (an == null)
                    {
                        an = new RequestNoteAnalytic() { AnalyticId = a.AnalyticId };
                        domain.Analytics.Add(an);
                    }
                    an.Percentage = a.Asigned;
                    an.Status = "Pendiente de Aprobación";
                }
                #endregion
                #region Product Services
                if (requestNoteBorrador.ProductsServices == null)
                    requestNoteBorrador.ProductsServices = new List<ProductsService>();
                foreach (var an in domain.ProductsServices.ToList())
                {
                    if (!requestNoteBorrador.ProductsServices.Any(a => a.ProductService == an.ProductService))
                        unitOfWork.RequestNoteProductServiceRepository.Delete(an);
                }
                foreach (var a in requestNoteBorrador.ProductsServices)
                {
                    var an = domain.ProductsServices.SingleOrDefault(p => p.ProductService == a.ProductService);
                    if (an == null)
                    {
                        an = new RequestNoteProductService() { ProductService = a.ProductService };
                        domain.ProductsServices.Add(an);
                    }
                    an.Quantity = a.Quantity;
                }
                #endregion
                #region Attachments
                if (requestNoteBorrador.Attachments == null)
                    requestNoteBorrador.Attachments = new List<Core.Models.RequestNote.File>();
                foreach (var an in domain.Attachments.ToList())
                {
                    if (!requestNoteBorrador.Attachments.Any(a => a.FileId == an.FileId))
                        unitOfWork.RequestNoteFileRepository.Delete(an);
                }
                foreach (var a in requestNoteBorrador.Attachments)
                {
                    var an = domain.Attachments.SingleOrDefault(p => p.FileId == a.FileId);
                    if (an == null)
                    {
                        an = new RequestNoteFile() { FileId = a.FileId.Value, Type = 1 };
                        domain.Attachments.Add(an);
                    }
                }
                #endregion
                #region Travel
                if (requestNoteBorrador.Travel != null)
                {
                    var travel = domain.Travels.FirstOrDefault();
                    if (travel == null)
                    {
                        travel = new RequestNoteTravel();
                        domain.Travels = new List<RequestNoteTravel>();
                        domain.Travels.Add(travel);
                    }
                    travel.Accommodation = requestNoteBorrador.Travel.Accommodation;
                    travel.Conveyance = requestNoteBorrador.Travel.Transportation;
                    travel.DepartureDate = requestNoteBorrador.Travel.DepartureDate;
                    travel.Destination = requestNoteBorrador.Travel.Destination;
                    travel.ItineraryDetail = requestNoteBorrador.Travel.Details;
                    travel.ReturnDate = requestNoteBorrador.Travel.ReturnDate;
                    if (travel.Employees == null)
                        travel.Employees = new List<RequestNoteTravelEmployee>();
                    if (travel.Employees?.Any() ?? false)
                    {
                        foreach (var an in travel.Employees.ToList())
                        {
                            if (!requestNoteBorrador.Travel.Passengers.Any(a => a.EmployeeId == an.EmployeeId))
                                unitOfWork.RequestNoteTravelEmployeeRepository.Delete(an);
                        }
                    }
                    foreach (var a in requestNoteBorrador.Travel.Passengers)
                    {
                        var an = travel.Employees.SingleOrDefault(p => p.EmployeeId == a.EmployeeId);
                        if (an == null)
                        {
                            an = new RequestNoteTravelEmployee() { EmployeeId = a.EmployeeId };
                            travel.Employees.Add(an);
                        }
                    }
                }
                else if (domain.Travels.Any())
                {
                    var travel = domain.Travels.FirstOrDefault();
                    if (travel?.Employees?.Any() ?? false)
                    {
                        foreach (var an in travel.Employees.ToList())
                        {
                            if (!requestNoteBorrador.Travel.Passengers.Any(a => a.EmployeeId == an.EmployeeId))
                                unitOfWork.RequestNoteTravelEmployeeRepository.Delete(an);
                        }
                    }
                    unitOfWork.RequestNoteTravelRepository.Delete(travel);
                }
                #endregion
                #region Training
                if (requestNoteBorrador.Training != null)
                {
                    var training = domain.Trainings.FirstOrDefault();
                    if (training == null)
                    {
                        training = new RequestNoteTraining();
                        domain.Trainings = new List<RequestNoteTraining>();
                        domain.Trainings.Add(training);
                    }
                   
                    training.Duration = requestNoteBorrador.Training.Duration;
                    training.TrainingDate = requestNoteBorrador.Training.Date;
                    training.Ammount = requestNoteBorrador.Training.Ammount;
                    training.Place = requestNoteBorrador.Training.Location;
                    training.Subject = requestNoteBorrador.Training.Subject;
                    training.Topic = requestNoteBorrador.Training.Name;
                    if (training.Employees == null)
                        training.Employees = new List<RequestNoteTrainingEmployee>();

                    if (training.Employees?.Any() ?? false)
                    {
                        foreach (var an in training.Employees.ToList())
                        {
                            if (!requestNoteBorrador.Training.Participants.Any(a => a.EmployeeId == an.EmployeeId))
                                unitOfWork.RequestNoteTrainingEmployeeRepository.Delete(an);
                        }
                    }
                    foreach (var a in requestNoteBorrador.Training.Participants)
                    {
                        var an = training.Employees.SingleOrDefault(p => p.EmployeeId == a.EmployeeId);
                        if (an == null)
                        {
                            an = new RequestNoteTrainingEmployee() { EmployeeId = a.EmployeeId, SectorProject = a.Sector };
                            training.Employees.Add(an);
                        }
                    }
                }
                else if (domain.Trainings.Any())
                {
                    var training = domain.Trainings.FirstOrDefault();
                    if (training?.Employees?.Any() ?? false)
                    {
                        foreach (var an in training.Employees.ToList())
                        {
                            if (!requestNoteBorrador.Training.Participants.Any(a => a.EmployeeId == an.EmployeeId))
                                unitOfWork.RequestNoteTrainingEmployeeRepository.Delete(an);
                        }
                    }
                    unitOfWork.RequestNoteTrainingRepository.Delete(training);
                }
                #endregion
                this.unitOfWork.RequestNoteRepository.UpdateRequestNote(domain);
                this.unitOfWork.RequestNoteRepository.Save();
                response.Data = domain.Id;

                response.AddSuccess(Resources.RequestNote.RequestNote.UpdateSuccess);
            }
            else
            {
                var workflow = unitOfWork.WorkflowRepository.GetLastByType(settings.RequestNoteWorkflowId);

                var domain = new Domain.Models.RequestNote.RequestNote()
                {
                    Description = requestNoteBorrador.Description,
                    RequiresEmployeeClient = requestNoteBorrador.RequiresEmployeeClient,
                    ConsideredInBudget = requestNoteBorrador.ConsideredInBudget,
                    EvalpropNumber = requestNoteBorrador.EvalpropNumber,
                    Comments = requestNoteBorrador.Comments,
                    TravelSection = requestNoteBorrador.Travel != null,
                    TrainingSection = requestNoteBorrador.Training != null,
                    CreationDate = DateTime.UtcNow,
                    WorkflowId = workflow.Id,
                    StatusId = settings.WorkflowStatusNPBorrador,
                    UserApplicantId = requestNoteBorrador.UserApplicantId,
                    InWorkflowProcess = true,
                    CreationUserId = user.Id,
                    ProviderAreaId = requestNoteBorrador.ProviderAreaId
                };
                if (requestNoteBorrador.Providers != null)
                    domain.ProvidersSugg = requestNoteBorrador.Providers.Select(p => new RequestNoteProviderSugg()
                    {
                        ProviderId = p.ProviderId,
                        FileId = p.FileId
                    }).ToList();
                if (requestNoteBorrador.Attachments != null)
                    domain.Attachments = requestNoteBorrador.Attachments.Where(f => f.FileId.HasValue).Select(p => new RequestNoteFile()
                    {
                        Type = 1, //Poner enum
                        FileId = p.FileId.Value
                    }).ToList();
                if (requestNoteBorrador.Analytics != null)
                    domain.Analytics = requestNoteBorrador.Analytics.Select(p => new RequestNoteAnalytic()
                    {
                        AnalyticId = p.AnalyticId,
                        Percentage = p.Asigned,
                        Status = "Pendiente de Aprobación"
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
                            EmployeeId = p.EmployeeId, SectorProject = p.Sector
                        }).ToList()
                    });

                }

                this.unitOfWork.RequestNoteRepository.InsertRequestNote(domain);
                this.unitOfWork.RequestNoteRepository.Save();
                response.Data = domain.Id;

                response.AddSuccess(Resources.RequestNote.RequestNote.AddSuccess);

            }

            return response;
        }
        public Response<RequestNoteModel> GetById(int id)
        {
            //TODO: acá hay que ver qué estados quedan, qué hace falta en cada uno, y cambiarlos por los ids de settings en vez de enum
            var response = new Response<RequestNoteModel>();

            var note = this.unitOfWork.RequestNoteRepository.GetById(id);
            if (note == null)
            {
                response.AddError(Resources.RequestNote.RequestNote.NotFound);
                return response;
            }
            var user = userData.GetCurrentUser();
            var permisos = unitOfWork.UserRepository.GetPermissions(user.Id, "NOPE");
            var datos = new RequestNoteModel(note, permisos, user.Id, settings);
            if (!datos.HasEditPermissions && !datos.HasReadPermissions)
            {
                response.AddError(Resources.RequestNote.RequestNote.NotAllowed);
                return response;
            }
            response.Data = datos;
            if (new List<int>() {
                settings.WorkflowStatusNPPendienteAprobacionGerente//,
                //settings.WorkflowStatusNPPendienteAprobacionCompras,
                //settings.WorkflowStatusNPPendienteAprobacionSAP
                //otros?
                }
            .Contains(note.StatusId))
            {
                response.Data.Analytics = response.Data.Analytics.Where(a => a.ManagerId == user.Id).ToList();
            }
            if (new List<int>() {
                settings.WorkflowStatusNPPendienteAprobacionDAF,
                settings.WorkflowStatusNPPendienteRecepcionMerc,
                settings.WorkflowStatusNPRecepcionParcial,
                settings.WorkflowStatusNPCerrado,
                //otros?
                }
            .Contains(note.StatusId))
            {
                response.Data.Providers = response.Data.Providers.Where(p => p.ProviderId == response.Data.ProviderSelectedId).ToList();
            }
            
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
        public Response<IList<Option>> GetStates()
        {
            var states = workflowStateRepository.GetStateByWorkflowTypeCode(settings.RequestNoteWorkflowTypeCode);

            var result = states.Select(x => new Option { Id = x.Id, Text = x.Name }).ToList();

            if (result.All(x => x.Id != settings.WorkflowStatusNPCerrado))
            {
                var finalizeState = workflowStateRepository.Get(settings.WorkflowStatusNPCerrado);

                result.Add(new Option { Id = finalizeState.Id, Text = finalizeState.Name });
            }
            /*
            var draft = result.SingleOrDefault(x => x.Id == settings.WorkflowStatusNPBorrador);

            if (draft != null) result.Remove(draft);
            */
            return new Response<IList<Option>>
            {
                Data = result
            };
        }
        public IList<RequestNoteGridModel> GetAll(RequestNoteGridFilters filters)
        {
            var user = userData.GetCurrentUser();
            var permisos = unitOfWork.UserRepository.GetPermissions(user.Id, "NOPE");
            return this.unitOfWork.RequestNoteRepository.GetAll(filters)
                    .Select(n=> new RequestNoteGridModel(n, permisos, user.Id, settings))
                    .Where(n=> n.HasEditPermissions || n.HasReadPermissions).ToList();
        }

        public void SaveChanges(RequestNoteModel requestNote, int nextStatus)
        {

            Domain.Models.RequestNote.RequestNote req = this.unitOfWork.RequestNoteRepository.GetById(requestNote.Id.Value);
            var user = userData.GetCurrentUser();
            if (req.StatusId == settings.WorkflowStatusNPPendienteAprobacionGerente) //Guardar analíticas
            {
                if (nextStatus == settings.WorkflowStatusNPPendienteAprobacionCompras)
                {
                    //Se marcan las analíticas del gerente logueado como “Rechazada”.
                    //Se cambia el estado de la requestNote a “PendienteAprobacionCompras”
                    //sólo si están todas las analíticas.
                    foreach (var analitica in req.Analytics.Where(a => a.Analytic.ManagerId == user.Id))
                    {
                        analitica.Status = "Aprobada";
                    }
                }
                else if (nextStatus == settings.WorkflowStatusNPRechazado)
                {
                    //Se marcan las analíticas del gerente logueado como “Rechazada”.
                    //Se cambia el estado de la requestNote a “Rechazada”
                    //sin importar el estado de las demás analíticas.
                    foreach (var analitica in req.Analytics.Where(a => a.Analytic.ManagerId == user.Id))
                    {
                        analitica.Status = "Rechazada";
                    }
                }
            }
            else if (req.StatusId == settings.WorkflowStatusNPPendienteAprobacionCompras 
                && nextStatus == settings.WorkflowStatusNPPendienteAprobacionDAF) //Guardar providers
            {
                if (requestNote.ProvidersSelected == null)
                    requestNote.ProvidersSelected = new List<Provider>();
                foreach (var prov in req.Providers.ToList())
                {
                    var nuevo = requestNote.ProvidersSelected.SingleOrDefault(a => a.ProviderId == prov.ProviderId);
                    if (nuevo == null) //existía pero ahora no viene => kaput
                        unitOfWork.RequestNoteProviderRepository.Delete(prov);
                    else
                    {
                        prov.FileId = nuevo.FileId;
                        prov.Price = nuevo.Ammount;
                    }
                }
                foreach (var provNuevo in requestNote.ProvidersSelected)
                {
                    var prov = req.Providers.SingleOrDefault(p => p.ProviderId == provNuevo.ProviderId);
                    if (prov == null)
                    {
                        prov = new RequestNoteProvider() { ProviderId = provNuevo.ProviderId, FileId = provNuevo.FileId, Price = provNuevo.Ammount };
                        req.Providers.Add(prov);
                    }
                }
            }
                /*
                case RequestNoteStatus.PendienteAprobaciónGerentesAnalítica:
                    //Se manda una lista de providers(como en la instancia borrador), deben reemplazar a los ya existentes
                    //en la requestNote.Se manda un campo monto final OC(number) para agregarse a la requestNote.
                    //Se debe asignar el estado de todas las analíticas asociadas a la requestNote como “Pendiente Aprobación”.
                    if (requestNote.Providers == null)
                        requestNote.Providers = new List<Provider>();

                    foreach (var prov in req.Providers.ToList())
                    {
                        if (!requestNote.Providers.Any(a => a.ProviderId == prov.ProviderId))
                            unitOfWork.RequestNoteProviderRepository.Delete(prov);
                    }
                    foreach (var provNuevo in requestNote.Providers)
                    {
                        var prov = req.Providers.SingleOrDefault(p => p.ProviderId == provNuevo.ProviderId);
                        if (prov == null)
                        {
                            prov = new RequestNoteProvider() { ProviderId = provNuevo.ProviderId };
                            req.Providers.Add(prov);
                        }
                        prov.FileId = provNuevo.FileId;
                        prov.IsSelected = false;
                    }
                    req.PurchaseOrderAmmount = requestNote.PurchaseOrderAmmount;
                    foreach (var analitica in req.Analytics)
                    {
                        analitica.Status = "Pendiente Aprobación";
                    }
                    break;
                case RequestNoteStatus.PendienteAprobaciónAbastecimiento:
                    if (req.StatusId == (int)RequestNoteStatus.PendienteAprobaciónGerentesAnalítica)
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
                    else if (req.StatusId == (int)RequestNoteStatus.PendienteAprobaciónDAF)
                    {
                        //Se cambia el estado de la requestNote a “Pendiente Aprobación Abastecimiento”.
                        //Se manda un campo (string) con comentario para guardar en histories.

                    }
                    break;
                case RequestNoteStatus.PendienteAprobaciónDAF:
                    //Se manda un provider que va a quedar como seleccionado, de ahora en adelante es
                    //el único que se tiene que devolver (sino puede traer todos pero que tenga un campo
                    //que lo identifique para solo mostrar ese).
                    //Se manda un campo numeroOC y un fileId para la orden de compra. 
                    req.PurchaseOrderNumber = requestNote.PurchaseOrderNumber;

                    foreach (var prov in req.Providers)
                    {
                        prov.IsSelected = prov.ProviderId == requestNote.ProviderSelectedId;
                    }

                    if (requestNote.Attachments != null && requestNote.Attachments.Any(f => f.FileId.HasValue))
                        req.Attachments.Add(requestNote.Attachments.Where(f => f.FileId.HasValue).Select(p => new RequestNoteFile()
                        {
                            Type = (int)RequestNoteFileTypes.OrdenDeCompra,
                            FileId = p.FileId.Value
                        }).First());
                    break;
                case RequestNoteStatus.Aprobada:
                    break;
                case RequestNoteStatus.SolicitadaAProveedor:
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
                case RequestNoteStatus.RecibidoConforme:
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
                case RequestNoteStatus.FacturaPendienteAprobaciónGerente:
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
                case RequestNoteStatus.PendienteProcesarGAF:
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
                case RequestNoteStatus.Rechazada:
                    break;
                case RequestNoteStatus.Cerrada:
                    //Se cambia el estado de la requestNote a  “Cerrada”. Se manda un campo (string) con comentario para guardar en histories.

                    break;
                default:
                    break;
            }*/
                this.unitOfWork.RequestNoteRepository.UpdateRequestNote(req);
            this.unitOfWork.RequestNoteRepository.Save();
        }
    }
}
