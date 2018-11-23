using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Sofco.Service.Crm.TranslatorMaps.Interfaces;
using Sofco.Service.Crm.Translators.Interfaces;

namespace Sofco.Service.Crm.Translators
{
    public class CrmTranslator<T, T2> : ICrmTranslator<T, T2> where T2 : ITranslatorMap, new()
    {
        private T2 translatorMap;

        public CrmTranslator()
        {
            translatorMap = new T2();
        }

        public T Translate(JObject data)
        {
            var item = data.ToObject<T>();

            var keyMaps = translatorMap.KeyMaps();

            if (keyMaps != null)
            {
                TranslateMap(item, data, keyMaps);
            }

            return item;
        }

        private void TranslateMap(T item, JObject data, Dictionary<string, string> keyMaps)
        {
            var typeName = typeof(T).Name;

            var propertyInfos = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var propertyInfo in propertyInfos)
            {
                var key = propertyInfo.Name;
                if (!keyMaps.ContainsKey(key)) continue;

                var value = data[keyMaps[key]];

                var propertyType = propertyInfo.PropertyType;

                var typeCode = Type.GetTypeCode(propertyType);
                try
                {
                    switch (typeCode)
                    {
                        case TypeCode.Int32:
                            propertyInfo.SetValue(item, Convert.ToInt32(value), null);
                            break;
                        case TypeCode.Int64:
                            propertyInfo.SetValue(item, Convert.ToInt64(value), null);
                            break;
                        case TypeCode.String:
                            propertyInfo.SetValue(item, value.ToString(), null);
                            break;
                        case TypeCode.Object:
                            if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
                            {
                                propertyInfo.SetValue(item, Guid.Parse(value.ToString()), null);
                            }
                            break;
                        default:
                            propertyInfo.SetValue(item, value, null);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to set property value for type: "+ typeName + " - key: "+ key +" exception:"+ ex.Message);
                }
            }
        }
    }
}
