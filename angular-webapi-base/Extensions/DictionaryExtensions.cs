using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace angular_webapi_base.Services
{
    public static class DictionaryExtensions
    {
        static public Dictionary<string, object> ToDict<TFrom>(this TFrom poco)
        {
            return poco.ToDictExcept(null);
        }

        static public Dictionary<string, object> ToDictExcept<TFrom>(this TFrom poco, params string[] exclude)
        {
            if (poco == null)
                return new Dictionary<string, object>();

            //make sure no archive partition values get passed through
            if (exclude != null)
            {
                List<string> excludeList = exclude.SafeAdd("ArchivePartition");

                return poco.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(prop => !excludeList.Contains(prop.Name))
                    .ToDictionary(prop => "@" + prop.Name, prop => prop.GetValue(poco, null));
            }
            else
            {
                return poco.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => "@" + prop.Name, prop => prop.GetValue(poco, null));
            }
        }

        static public IDictionary<string, object> ToDictionary(this SqlDataReader reader)
        {
            var dict = new Dictionary<string, object>();

            for (var i = 0; i < reader.FieldCount; i++)
                dict.Add(reader.GetName(i), reader[i]);

            return dict;
        }

        static public void Each<TKey, TValue>(this IDictionary<TKey, TValue> dict, Action<KeyValuePair<TKey, TValue>> action)
        {
            if (dict == null)
                return;

            dict.ForEach(keyValuePair => action(keyValuePair));
        }

        public static Dictionary<string, string> ToDictionary(this NameValueCollection nvc)
        {
            return nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);
        }
    }
}
