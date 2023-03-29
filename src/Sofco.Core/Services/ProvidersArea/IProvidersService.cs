using Sofco.Core.Models.Providers;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Services.Providers
{
    public interface IprovidersService
    {
        Response<IList<ProvidersModelGet>> GetAll();
        Response<IList<ProvidersModelGet>> GetByParams(ProvidersGetByParamsModel param);
        Response<ProvidersModelGet> GetById(int providersAreaid);
        Response<ProvidersModel> Post(ProvidersModel model);
        Response Put(int id, ProvidersModel model);

    }
}
