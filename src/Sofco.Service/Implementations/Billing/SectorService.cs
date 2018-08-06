using System;
using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Billing
{
    public class SectorService : ISectorService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;

        private readonly ILogMailer<SectorService> logger;

        private readonly ISectorData sectorData;

        public SectorService(IUnitOfWork unitOfWork, IMapper mapper, ILogMailer<SectorService> logger, ISectorData sectorData)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
            this.sectorData = sectorData;
        }

        public Response<List<SectorModel>> GetAll()
        {
            var result = Translate(unitOfWork.SectorRepository.GetAll());

            return new Response<List<SectorModel>> { Data = result };
        }

        public Response<SectorAdminModel> GetById(int id)
        {
            var response = new Response<SectorAdminModel>();
            var sector = unitOfWork.SectorRepository.Get(id);

            if (sector != null)
            {
                response.Data = new SectorAdminModel { Id = sector.Id, Text = sector.Text, ResponsableId = sector.ResponsableUserId };
                return response;
            }

            response.AddError(Resources.Admin.Sector.NotFound);
            return response;
        }

        public Response Add(SectorAdminModel model)
        {
            var response = new Response();

            Validate(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var sector = new Sector
                {
                    Text = model.Text,
                    ResponsableUserId = model.ResponsableId,
                    Active = true,
                    StartDate = DateTime.UtcNow
                };

                unitOfWork.SectorRepository.Insert(sector);
                unitOfWork.Save();

                sectorData.ClearKeys();

                response.AddSuccess(Resources.Admin.Sector.Created);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Update(SectorAdminModel model)
        {
            var response = new Response();
            var entity = unitOfWork.SectorRepository.Get(model.Id);

            if (entity == null)
            {
                response.AddError(Resources.Admin.Sector.NotFound);
                return response;
            }

            Validate(model, response);

            if (response.HasErrors()) return response;

            try
            {
                entity.Text = model.Text;
                entity.ResponsableUserId = model.ResponsableId;

                unitOfWork.SectorRepository.Update(entity);
                unitOfWork.Save();

                sectorData.ClearKeys();

                response.AddSuccess(Resources.Admin.Sector.Updated);
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
            var entity = unitOfWork.SectorRepository.Get(id);

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

                unitOfWork.SectorRepository.Update(entity);
                unitOfWork.Save();

                response.AddSuccess(active ? Resources.Admin.Sector.Enabled : Resources.Admin.Sector.Disabled);
                return response;
            }

            response.AddError(Resources.Admin.Sector.NotFound);
            return response;
        }

        private void Validate(SectorAdminModel model, Response response)
        {
            if (string.IsNullOrWhiteSpace(model.Text))
                response.AddError(Resources.Admin.Sector.TextRequired);

            if (model.ResponsableId <= 0)
                response.AddError(Resources.Admin.Sector.ResponsableRequired);
        }

        private List<SectorModel> Translate(List<Sector> data)
        {
            return mapper.Map<List<Sector>, List<SectorModel>>(data);
        }
    }
}
