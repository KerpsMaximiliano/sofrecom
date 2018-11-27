using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sofco.Service.Crm.TranslatorMaps.Interfaces;

namespace Sofco.Service.Crm.Translators.Interfaces
{
    public interface ICrmTranslator<T, T2> where T2 : ITranslatorMap
    {
        T Translate(JObject data);

        List<T> TranslateList(JObject data);

        string KeySelects();
    }
}