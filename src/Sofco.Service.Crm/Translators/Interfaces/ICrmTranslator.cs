using Newtonsoft.Json.Linq;
using Sofco.Service.Crm.TranslatorMaps.Interfaces;

namespace Sofco.Service.Crm.Translators.Interfaces
{
    public interface ICrmTranslator<out T, T2> where T2 : ITranslatorMap
    {
        T Translate(JObject data);
    }
}