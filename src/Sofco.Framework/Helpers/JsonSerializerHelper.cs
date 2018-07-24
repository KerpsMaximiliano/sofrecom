using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Sofco.Framework.Helpers
{
    public static class JsonSerializeHelper
    {
        private static readonly JsonSerializerSettings SerializerSettings;

        static JsonSerializeHelper()
        {
            SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            SerializerSettings.Converters.Add(new StringEnumConverter(true));
        }

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, SerializerSettings);
        }

        public static T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, SerializerSettings);
        }
    }
}
