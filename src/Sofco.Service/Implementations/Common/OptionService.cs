using Sofco.Core.DAL.Common;
using Sofco.Core.Logger;
using Sofco.Core.Services.Common;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Managers;

namespace Sofco.Service.Implementations.Common
{
    public class OptionService<TEntity> : IOptionService<TEntity> where TEntity : Option
    {
        private readonly IOptionRepository<TEntity> repository;
        private readonly ILogMailer<OptionService<TEntity>> logger;
        private readonly IGenericOptionManager genericOptionManager;

        public OptionService(IOptionRepository<TEntity> repository, ILogMailer<OptionService<TEntity>> logger, IGenericOptionManager genericOptionManager)
        {
            this.repository = repository;
            this.logger = logger;
            this.genericOptionManager = genericOptionManager;
        }

        public Response<string> Add(string description, Dictionary<string, string> parameters)
        {
            var response = new Response<string>();

            ValidateDescription(description, response);
            if (response.HasErrors()) return response;

            try
            {
                var domain = (TEntity)Activator.CreateInstance(typeof(TEntity));

                domain.Text = description;
                domain.Active = true;

                genericOptionManager.SetParameters(domain, parameters);

                repository.Insert(domain);

                repository.Save();

                response.Data = domain.Id.ToString();

                response.AddSuccess(Resources.Common.SaveSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Update(int id, string description, Dictionary<string, string> parameters)
        {
            var response = new Response();

            ValidateDescription(description, response);
            if (response.HasErrors()) return response;

            var domain = repository.Get(id);

            if (domain == null)
            {
                response.AddError(Resources.Common.OptionNotFound);
                return response;
            }

            try
            {
                domain.Text = description;

                genericOptionManager.SetParameters(domain, parameters);

                repository.Update(domain);

                repository.Save();

                response.AddSuccess(Resources.Common.UpdateSuccess);
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

            var domain = repository.Get(id);

            if (domain == null)
            {
                response.AddError(Resources.Common.OptionNotFound);
                return response;
            }

            try
            {
                domain.Active = active;

                repository.Update(domain);

                repository.Save();

                response.AddSuccess(Resources.Common.UpdateSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<IList<TEntity>> Get()
        {
            var response = new Response<IList<TEntity>> { Data = new List<TEntity>() };

            var list = repository.GetAll();

            if (list.Any())
            {
                response.Data = list;
            }

            return response;
        }

        public Response<IList<TEntity>> GetActives()
        {
            var response = new Response<IList<TEntity>> { Data = new List<TEntity>() };

            var list = repository.GetAll();

            if (list.Any())
            {
                response.Data = list.Where(x => x.Active).ToList();
            }

            return response;
        }

        private void ValidateDescription(string description, Response response)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                response.AddError(Resources.Common.TextRequired);
            }
        }
    }
}
