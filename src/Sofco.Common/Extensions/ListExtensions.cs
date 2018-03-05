using System;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.Common.Extensions
{
    public static class ListExtensions
    {
        public static List<T> DistinctBy<T>(this List<T> list, Func<T, string> getKey)
        {
            return list
                .GroupBy(getKey)
                .Select(x => x.FirstOrDefault())
                .ToList();
        }
    }
}
