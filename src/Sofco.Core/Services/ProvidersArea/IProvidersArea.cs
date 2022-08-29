using Sofco.Core.Models.Admin;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Services
{
    public interface IProvidersAreaService
    {

       
        Response<IList<ProvidersAreaModel>> GetAll();

        Response<ProvidersAreaModel> GetById(int providersAreaid);

        Response<ProvidersAreaModel> Post(ProvidersAreaModel model);

        Response Put(int id, ProvidersAreaModel model);

       
    }
}
