using System;
using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Admin;
using Sofco.Core.Services.Billing;
using Sofco.Model.Utils;
using AreaModel = Sofco.Core.Models.Billing.AreaModel;

namespace Sofco.Service.Implementations.Billing
{
    public class AreaService : IAreaService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;

        private readonly ILogMailer<AreaService> logger;

        public AreaService(IUnitOfWork unitOfWork, IMapper mapper, ILogMailer<AreaService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }

        public Response<List<AreaModel>> GetAll()
        {
            var result = Translate(unitOfWork.AreaRepository.GetAll());

            return new Response<List<AreaModel>> { Data = result };
        }

        public Response<AreaAdminModel> GetById(int id)
        {
            var response = new Response<AreaAdminModel>();
            var area = unitOfWork.AreaRepository.Get(id);

            if (area != null)
            {
                response.Data = new AreaAdminModel { Id = area .Id, Text = area.Text, ResponsableId = area.ResponsableUserId };
                return response;
            }

            response.AddError(Resources.Admin.Area.NotFound);
            return response;
        }

        public Response Add(AreaAdminModel model)
        {
            var response = new Response();

            Validate(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var area = new Area
                {
                    Text = model.Text,
                    ResponsableUserId = model.ResponsableId,
                    Active = true,
                    StartDate = DateTime.UtcNow
                };

                unitOfWork.AreaRepository.Insert(area);
                unitOfWork.Save();

                response.AddSuccess(Resources.Admin.Area.Created);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Update(AreaAdminModel model)
        {
            var response = new Response();
            var entity = unitOfWork.AreaRepository.Get(model.Id);

            if (entity == null)
            {
                response.AddError(Resources.Admin.Area.NotFound);
                return response;
            }

            Validate(model, response);

            if (response.HasErrors()) return response;

            try
            {
                entity.Text = model.Text;
                entity.ResponsableUserId = model.ResponsableId;

                unitOfWork.AreaRepository.Update(entity);
                unitOfWork.Save();

                response.AddSuccess(Resources.Admin.Area.Updated);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Active(int id, bool active)
        {
            var response = new Response();
            var entity = unitOfWork.AreaRepository.Get(id);

            if (entity != null)
            {
                entity.Active = active;

                if (active)
                {
                    entity.StartDate = DateTime.UtcNow;
                    entity.EndDate = null;
                }
                else
                {
                    entity.EndDate = DateTime.UtcNow;
                }

                unitOfWork.AreaRepository.Update(entity);
                unitOfWork.Save();

                response.AddSuccess(active ? Resources.Admin.Area.Enabled : Resources.Admin.Area.Disabled);
                return response;
            }

            response.AddError(Resources.Admin.Category.NotFound);
            return response;
        }

        private void Validate(AreaAdminModel model, Response response)
        {
            if (string.IsNullOrWhiteSpace(model.Text))
                response.AddError(Resources.Admin.Area.TextRequired);

            if(model.ResponsableId <= 0) 
                response.AddError(Resources.Admin.Area.ResponsableRequired);
        }

        private List<AreaModel> Translate(List<Area> data)
        {
            return mapper.Map<List<Area>, List<AreaModel>>(data);
        }
    }
}
