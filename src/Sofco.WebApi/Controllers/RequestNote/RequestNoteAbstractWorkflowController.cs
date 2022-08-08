using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.RequestNote
{
    public abstract class RequestNoteAbstractWorkflowController<T, Y> : Controller
        where T : RequestNoteSubmitDTO
        where Y : RequestNoteLoadDTO
    {
        private readonly Dictionary<string, IRequestNoteCommand<T>> actions;

        public RequestNoteAbstractWorkflowController()
        {
            actions = GetActionDictionary();
        }

        [HttpGet("{id}")]
        public IActionResult AbstractGet(int id)
        {
            return this.CreateResponse(Get(id));
        }

        [HttpGet("actions")]
        public IActionResult Actions()
        {
            Response<List<string>> r = new Response<List<string>>();
            r.Data = new List<string>(this.GetActionDictionary().Keys);
            return this.CreateResponse(r);
        }

        [HttpPost]
        protected IActionResult Submit([FromBody] T rnsdto)
        {
            actions.TryGetValue(rnsdto.Action, out IRequestNoteCommand<T> c);

            Response r = c.CanExecute(rnsdto);
            if (!r.HasErrors())
            {
                r = c.Validate(rnsdto);
                if (!r.HasErrors())
                {
                    r = c.Execute(rnsdto);
                    if (!r.HasErrors())
                    {
                        r = c.Notify(rnsdto);
                    }
                }
            }

            return this.CreateResponse(r);
        }

        protected abstract Dictionary<string, IRequestNoteCommand<T>> GetActionDictionary();

        protected abstract Response<Y> Get(int id);
    }
}