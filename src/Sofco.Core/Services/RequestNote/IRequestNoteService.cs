﻿using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.Providers;
using Sofco.Core.Models.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.RequestNoteStates;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using File = Sofco.Domain.Models.Common.File;

namespace Sofco.Core.Services.RequestNote
{
    public interface IRequestNoteService
    {
        Task<Response<List<File>>> AttachFiles(Response<List<File>> response, List<IFormFile> files);
        void CambiarAPendienteApobacionGerenteAnalitica(int requestNodeId);
        void RechazarRequestNote(int requestNodeId);
        Response<int> Add(RequestNoteModel requestNoteBorrador);
        Response<RequestNoteModel> GetById(int id);
        IList<RequestNoteGridModel> GetAll(RequestNoteGridFilters filters);
        IList<ProviderMinModel> GetProviders(int RequestNoteID);
        void SaveChanges(RequestNoteModel requestNote, int nextStatus);

        Response<IList<Option>> GetStates();
        Response<IList<Option>> GetUnits();
        Response<IList<Option>> GetCurrencies();


    }
}
