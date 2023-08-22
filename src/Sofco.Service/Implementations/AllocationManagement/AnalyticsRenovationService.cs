using Sofco.Core.DAL;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Logger;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.DAL;
using Sofco.Data.Billing;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;
using Sofco.Framework.Managers;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class AnalyticsRenovationService : IAnalyticsRenovationService
    {
        private readonly IAnalyticsRenovationRepository _analyticsRenovationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogMailer<AnalyticsRenovationService> _logger;

        public AnalyticsRenovationService(IAnalyticsRenovationRepository analyticsRenovationRepository,
                                          IUnitOfWork unitOfWork,
                                          ILogMailer<AnalyticsRenovationService> logger)
        {
            _analyticsRenovationRepository = analyticsRenovationRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public Response<List<AnalyticsRenovation>> GetAllByAnalyticId(int analyticId)
        {
            var response = new Response<List<AnalyticsRenovation>>();
            response.Data = _analyticsRenovationRepository.GetAllByAnalyticId(analyticId);
            return response;
        }

        public Response<AnalyticsRenovation> Add(AnalyticsRenovation renovation)
        {
            var response = new Response<AnalyticsRenovation>();

            AnalyticsRenovationsValidationHelper.Exist(response, _unitOfWork, renovation);

            if (response.HasErrors()) return response;

            try
            {
                _unitOfWork.AnalyticsRenovationRepository.Insert(renovation);
                _unitOfWork.Save();

                response.AddSuccess(Resources.AllocationManagement.AnalyticsRenovation.SaveSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<AnalyticsRenovation> Update(AnalyticsRenovationModel renovationModel)
        {
            var response = new Response<AnalyticsRenovation>();
            try
            {
                var renovation = AnalyticsRenovationsValidationHelper.Find(response, _unitOfWork, renovationModel.Id);                

                //ToDo - Agregar validaciones
                AnalyticsRenovationsValidationHelper.Exist(response, _unitOfWork, renovation);
                AnalyticsRenovationsValidationHelper.CheckDates(response, renovationModel);
                
                if (response.HasErrors()) return response;

                #region Update AnalyticsRenovation
                renovationModel.UpdateDomain(renovation);
                if (renovationModel.EndDate != renovation.EndDate)
                {
                    renovation.EndDate = renovationModel.EndDate;
                    _unitOfWork.AnalyticsRenovationRepository.Update(renovation);
                }
                
                #endregion

                #region Update Analytics
                var analytic = AnalyticValidationHelper.Find(response, _unitOfWork, renovationModel.AnalyticId);

                AnalyticValidationHelper.CheckName(response, analytic);
                AnalyticValidationHelper.CheckDirector(response, analytic);
                AnalyticValidationHelper.CheckDates(response, analytic);
                
                if (response.HasErrors()) return response;

                if(renovationModel.EndDate != analytic.EndDateContract)
                {
                    analytic.EndDateContract = renovationModel.EndDate;
                    _unitOfWork.AnalyticRepository.Update(analytic);
                }                
                #endregion

                _unitOfWork.Save();

                response.AddSuccess(Resources.AllocationManagement.AnalyticsRenovation.UpdateSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }
    }
}
