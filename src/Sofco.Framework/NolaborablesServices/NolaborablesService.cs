using System;
using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Core.Logger;
using Sofco.Domain.Nolaborables;
using Sofco.Framework.NolaborablesServices.Interfaces;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Framework.NolaborablesServices
{
    public class NolaborablesService : INolaborablesService
    {
        private const string UrlApi = "http://nolaborables.com.ar/api/v2/feriados";

        private readonly INolaborablesHttpClient client;

        private readonly ILogMailer<NolaborablesService> logger;

        public NolaborablesService(INolaborablesHttpClient client, ILogMailer<NolaborablesService> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        public Result<List<Feriado>> Get(int year)
        {
            var result = new Result<List<Feriado>>();

            var url = $"{UrlApi}/{year}";

            try
            {
                result.Data = client.Get<List<Feriado>>(url).Data;

                result.Data.ForEach(x => x.Year = year);
            }
            catch (Exception ex)
            {
                logger.LogError(url, ex);

                result.AddError(ex.Message);
            }

            return result;
        }
    }
}
