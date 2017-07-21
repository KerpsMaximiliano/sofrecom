using System;
using System.Reflection;

namespace Sofco.Helper
{
    public static class ReflectionHelper
    {
        public static void SetPropertyValues<T>(T o, ref T d)
            where T : class
        {
            Type t = o.GetType();
            PropertyInfo[] propsO = t.GetProperties();

            foreach (PropertyInfo prpO in propsO)
            {
                if (prpO != null && prpO.CanRead && prpO.CanWrite)
                {
                    var value = prpO.GetValue(o);
                    prpO.SetValue(d, value);
                }

            }
        }

        //public static void SetSimplePropertyValues<T>(T o, ref T d)
        //    where T : class
        //{
        //    Type t = o.GetType();
        //    PropertyInfo[] propsO = t.GetProperties();

        //    foreach (PropertyInfo prpO in propsO)
        //    {
        //        if (prpO != null && prpO.CanRead && prpO.CanWrite)
        //        {
        //            if (prpO.PropertyType.BaseType != null && (
        //                prpO.PropertyType.BaseType.Name == "ValueType" || prpO.PropertyType.Name == "String")
        //                )
        //            {
        //                var value = prpO.GetValue(o);
        //                prpO.SetValue(d, value);
        //            }

        //        }

        //    }
        //}

        public static bool HasProperty<T>(string propName)
        {
            Type t = typeof(T);
            PropertyInfo[] propsO = t.GetProperties();
            bool rpta = false;

            foreach (PropertyInfo prpO in propsO)
            {
                if (prpO.Name == propName)
                {
                    rpta = true;
                    break;
                }
            }

            return rpta;
        }
    }
}
