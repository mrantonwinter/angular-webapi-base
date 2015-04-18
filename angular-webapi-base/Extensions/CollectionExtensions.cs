using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace angular_webapi_base.Services
{
    public static class CollectionsExtensions
    {
        static public void ForEach<T>(this IEnumerable<T> rows, Action<T> action)
        {
            if (rows != null)
                rows.ToList().ForEach(action);
        }


        static public List<T> SafeAdd<T>(this T[] rows, T row)
        {
            var list = new List<T>(rows);

            list.Add(row);

            return list;
        }



    }
}
