using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Billing
{
    public class SolfacService : ISolfacService
    {
        private readonly ISolfacRepository _solfacRepository;

        public SolfacService(ISolfacRepository solfacRepository)
        {
            _solfacRepository = solfacRepository;
        }

        public Response<Solfac> Add(Solfac solfac)
        {
            var response = Validate(solfac);

            if (response.HasErrors()) return response;

            try
            {
                solfac.UpdatedDate = DateTime.Now;
                solfac.ModifiedByUserId = solfac.UserApplicantId;

                _solfacRepository.Insert(solfac);
                _solfacRepository.Save(string.Empty);

                response.Messages.Add(new Message(Resources.es.Billing.Solfac.SolfacCreated, MessageType.Success));
            }
            catch (Exception e)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public IList<Solfac> Search(SolfacParams parameter)
        {
            return _solfacRepository.SearchByParams(parameter);
        }

        public IList<Hito> GetHitosByProject(string projectId)
        {
            return _solfacRepository.GetHitosByProject(projectId);
        }

        public IList<Solfac> GetByProject(string projectId)
        {
            return _solfacRepository.GetByProject(projectId);
        }

        public Response<Solfac> GetById(int id)
        {
            var response = new Response<Solfac>();

            var solfac = _solfacRepository.GetById(id);

            if (solfac == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.NotFound, MessageType.Error));
                return response;
            }

            response.Data = solfac;
            return response;
        }

        private Response<Solfac> Validate(Solfac solfac)
        {
            var response = new Response<Solfac>();

            if(solfac.OtherProvince1Percentage > 0 && solfac.Province1Id == 0)
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.ProvinceRequired, MessageType.Error));

            if (solfac.OtherProvince2Percentage > 0 && solfac.Province2Id == 0)
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.ProvinceRequired, MessageType.Error));

            if (solfac.OtherProvince3Percentage > 0 && solfac.Province3Id == 0)
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.ProvinceRequired, MessageType.Error));

            foreach (var hito in solfac.Hitos)
            {
                if(hito.Quantity <= 0)
                    response.Messages.Add(new Message(Resources.es.Billing.Solfac.HitoQuantityRequired, MessageType.Error));

                if(hito.UnitPrice <= 0)
                    response.Messages.Add(new Message(Resources.es.Billing.Solfac.HitoUnitPriceRequired, MessageType.Error));
            }

            var totalPercentage = solfac.BuenosAiresPercentage + solfac.CapitalPercentage +
                                  solfac.OtherProvince1Percentage + solfac.OtherProvince2Percentage +
                                  solfac.OtherProvince3Percentage;

            if(totalPercentage != 100)
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.TotalPercentageError, MessageType.Error));

            if (solfac.CapitalPercentage < 0 || solfac.BuenosAiresPercentage < 0 ||
                solfac.OtherProvince1Percentage < 0 || solfac.OtherProvince2Percentage < 0 ||
                solfac.OtherProvince3Percentage < 0)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.PercentageLessThan0, MessageType.Error));
            }

            if (solfac.TimeLimit <= 0)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.TimeLimitLessThan0, MessageType.Error));
            }

            return response;
        }
    }
}
