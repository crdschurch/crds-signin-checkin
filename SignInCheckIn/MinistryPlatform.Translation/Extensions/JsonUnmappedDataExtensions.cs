using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Translation.Extensions
{
    public static class JsonUnmappedDataExtensions
    {
        public static T GetUnmappedDataField<T>(this IDictionary<string, JToken> unmappedData, string fieldName)
        {
            return unmappedData.ContainsKey(fieldName) ? unmappedData[fieldName].Value<T>() : default(T);
        }
    }
}
