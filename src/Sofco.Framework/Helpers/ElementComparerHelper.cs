using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sofco.Framework.Helpers
{
    public class ElementComparerHelper
    {
        private const string ModifiedPropertyName = "Modified";

        private static readonly Type ListType;

        static ElementComparerHelper()
        {
            ListType = typeof(IList<>);
        }

        public static string[] CompareModification<T>(T item1, T item2, string[] fields = null)
        {
            var modifiedFields = new List<string>();

            var propertyInfos = item1.GetType().GetProperties().Where(s => s.Name != ModifiedPropertyName);
            if (fields != null && fields.Any())
            {
                propertyInfos = item1.GetType().GetProperties().Where(s => fields.Contains(s.Name));
            }

            foreach (var propertyInfo in propertyInfos)
            {
                var valueA = propertyInfo.GetValue(item1, null);

                var valueB = propertyInfo.GetValue(item2, null);

                var propType = propertyInfo.PropertyType;

                if (propType.IsGenericParameter &&
                    propType.GetGenericTypeDefinition() == ListType)
                {
                    continue;
                }

                if (valueA != null && valueB != null && valueA.ToString() != valueB.ToString())
                {
                    modifiedFields.Add(propertyInfo.Name);
                }
            }

            return modifiedFields.ToArray();
        }

        public static void ApplyModifications<T>(T currentItem, T newItem, string[] fields)
        {
            var propertyInfos = currentItem.GetType().GetProperties().Where(s => fields.Contains(s.Name));
            foreach (var propertyInfo in propertyInfos)
            {
                var newValue = propertyInfo.GetValue(newItem, null);

                propertyInfo.SetValue(currentItem, newValue);
            }
        }
    }
}