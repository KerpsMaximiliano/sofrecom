﻿using System;
using System.Net.Http;
using System.Collections.Generic;
using Sofco.Common.Domains;

namespace Sofco.Service.Http.Interfaces
{
    public interface IBaseHttpClient
    {
        Result<T> Post<T>(string urlPath, HttpContent content);

        Result<T> Put<T>(string urlPath, HttpContent content);

        Result<T> Get<T>(string urlPath, string token = null, TimeSpan? timeOut = null);

        Result<List<T>> GetMany<T>(string urlPath, string token = null);
    }
}
