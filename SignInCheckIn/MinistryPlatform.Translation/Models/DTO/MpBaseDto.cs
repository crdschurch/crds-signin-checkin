using System.Collections.Generic;
using System.Runtime.Serialization;
using MinistryPlatform.Translation.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Translation.Models.DTO
{
    public abstract class MpBaseDto
    {
        [JsonExtensionData]
#pragma warning disable 649
        private IDictionary<string, JToken> _unmappedData;
#pragma warning restore 649

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_unmappedData == null || _unmappedData.Count == 0)
            {
                return;
            }

            ProcessUnmappedData(_unmappedData, context);
        }

        protected virtual void ProcessUnmappedData(IDictionary<string, JToken> unmappedData, StreamingContext context)
        {
        }

        protected T GetUnmappedField<T>(string fieldName)
        {
            return _unmappedData.GetUnmappedDataField<T>(fieldName);
        }
    }
}
