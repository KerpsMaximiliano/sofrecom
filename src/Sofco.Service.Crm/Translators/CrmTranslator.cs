using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sofco.Service.Crm.TranslatorMaps.Interfaces;
using Sofco.Service.Crm.Translators.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sofco.Service.Crm.Translators
{
    public class CrmTranslator<T, T2> : ICrmTranslator<T, T2> where T2 : ITranslatorMap, new()
    {
        private const string ListKey = "value";

        private const string ErrorMapMessage = "Failed to set property value for type: {0} - key: {1} value:{2} source: {3}- exception: {4}";

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

        public List<T> TranslateList(JObject data)
        {
            var dataList = data[ListKey].Values<JObject>();

            return dataList.Select(Translate).ToList();
        }

        public string KeySelects()
        {
            return translatorMap.KeySelects();
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

                var stringValue = value != null ? value.ToString() : string.Empty;

                try
                {
                    switch (typeCode)
                    {
                        case TypeCode.Int32:
                            var intValue = stringValue != string.Empty ? value.Value<int>() : 0;

                            propertyInfo.SetValue(item, intValue, null);
                            break;
                        case TypeCode.Decimal:
                            var decimalValue = stringValue != string.Empty ? value.Value<decimal>() : 0;

                            propertyInfo.SetValue(item, decimalValue, null);
                            break;
                        case TypeCode.String:
                            propertyInfo.SetValue(item, stringValue, null);
                            break;
                        case TypeCode.Object:
                            if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
                            {
                                var guidValue = value != null
                                                && stringValue != string.Empty
                                    ? Guid.Parse(value.ToString())
                                    : Guid.Empty;

                                propertyInfo.SetValue(item, guidValue, null);
                            }
                            break;
                        case TypeCode.DateTime:
                            {
                                if (string.IsNullOrWhiteSpace(stringValue))
                                {
                                    propertyInfo.SetValue(item, DateTime.MinValue, null);
                                }
                                else
                                {
                                    if (DateTime.TryParse(stringValue,
                                        System.Globalization.CultureInfo.GetCultureInfo("es-AR"),
                                        System.Globalization.DateTimeStyles.None, out DateTime dateValue))
                                    {
                                        propertyInfo.SetValue(item, dateValue, null);
                                    }
                                    else
                                    {
                                        if (stringValue.Contains("/"))
                                        {
                                            var split = stringValue.Split('/');

                                            if (split.Length != 3)
                                                propertyInfo.SetValue(item, DateTime.MinValue, null);
                                            else
                                            {
                                                var day = Convert.ToInt32(split[1].Split(' ')[0]);
                                                var month = Convert.ToInt32(split[0].Split(' ')[0]);
                                                var year = Convert.ToInt32(split[2].Split(' ')[0]);

                                                propertyInfo.SetValue(item, new DateTime(year, month, day), null);
                                            }
                                        }
                                        else
                                        {
                                            var split = stringValue.Split('-');

                                            if (split.Length != 3)
                                                propertyInfo.SetValue(item, DateTime.MinValue, null);
                                            else
                                            {
                                                var day = Convert.ToInt32(split[2].Split(' ')[0]);
                                                var month = Convert.ToInt32(split[1].Split(' ')[0]);
                                                var year = Convert.ToInt32(split[0].Split(' ')[0]);

                                                propertyInfo.SetValue(item, new DateTime(year, month, day), null);
                                            }
                                        }
                                    }
                                }

                                break;
                            }

                        case TypeCode.Boolean:
                            var boolValue = stringValue != string.Empty && bool.Parse(stringValue);

                            propertyInfo.SetValue(item, boolValue, null);
                            break;
                        default:
                            propertyInfo.SetValue(item, value, null);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(ErrorMapMessage,
                        typeName, key, value, JsonConvert.SerializeObject(item),
                        ex.Message));
                }
            }
        }
    }
}
