using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.DAL;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Rh.Tiger;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Repository.Rh.Repositories.Interfaces;

namespace Sofco.Service.Implementations.Jobs
{
    public class HealthInsuranceJobService : IHealthInsuranceJobService
    {
        private readonly ITigerHealthInsuranceRepository tigerHealthInsuranceRepository;

        private readonly ITigerPrepaidHealthRepository tigerPrepaidHealthRepository;

        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;

        public HealthInsuranceJobService(IUnitOfWork unitOfWork, IMapper mapper, ITigerHealthInsuranceRepository tigerHealthInsuranceRepository, ITigerPrepaidHealthRepository tigerPrepaidHealthRepository)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.tigerHealthInsuranceRepository = tigerHealthInsuranceRepository;
            this.tigerPrepaidHealthRepository = tigerPrepaidHealthRepository;
        }


        public void SyncHealthInsurance()
        {
            var list = Translate(tigerHealthInsuranceRepository.GetAll());

            unitOfWork.HealthInsuranceRepository.Save(list);
        }

        public void SyncPrepaidHealth()
        {
            var list = Translate(tigerPrepaidHealthRepository.GetAll());

            unitOfWork.PrepaidHealthRepository.Save(list);
        }

        public List<PrepaidHealth> Translate(List<TigerPrepaidHealth> tigerPrepaidHealths)
        {
            return mapper.Map<List<TigerPrepaidHealth>, List<PrepaidHealth>>(tigerPrepaidHealths);
        }

        public List<HealthInsurance> Translate(List<TigerHealthInsurance> tigerHealthInsurances)
        {
            return mapper.Map<List<TigerHealthInsurance>, List<HealthInsurance>>(tigerHealthInsurances);
        }
    }
}
