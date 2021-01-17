using System;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Wallet.Common
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }

        public static string ToJson(this object self, bool enforceCamelCase = false)
        {
            if (self == null)
                return null;

            var settings = new JsonSerializerSettings
            {
                ContractResolver = enforceCamelCase ? new CamelCasePropertyNamesContractResolver() : new DefaultContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return JsonConvert.SerializeObject(self, settings);
        }

        public static T FromJson<T>(this string self)
        {
            if (self.IsEmpty())
                return default(T);

            return JsonConvert.DeserializeObject<T>(self);
        }

        public static object FromJson(this string self, Type type, bool safe = false)
        {
            if (self.IsEmpty())
                return null;

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
            };

            if (safe)
                settings.Error = (sender, args) => args.ErrorContext.Handled = true;

            if (type == typeof(ExpandoObject))
                settings.Converters.Add(new ExpandoObjectConverter());

            return JsonConvert.DeserializeObject(self, type, settings);
        }

        public static string ToCommaSeparated(this IEnumerable<string> self)
        {
            return string.Join(", ", self);
        }
    }
}