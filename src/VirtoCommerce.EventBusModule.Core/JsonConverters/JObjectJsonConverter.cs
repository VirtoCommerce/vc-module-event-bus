using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VirtoCommerce.EventBusModule.Core.JsonConverters
{
    public class JObjectJsonConverterNewtonsoft : Newtonsoft.Json.JsonConverter<JObject>
    {
        public override JObject ReadJson(JsonReader reader, Type objectType, [AllowNull] JObject existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)=>JObject.Load(reader);

        public override void WriteJson(JsonWriter writer, [AllowNull] JObject value, Newtonsoft.Json.JsonSerializer serializer)=> value.WriteTo(writer);
    }

    public class JObjectJsonConverterSystemNet : System.Text.Json.Serialization.JsonConverter<JObject>
    {
        
        public override bool CanConvert(Type typeToConvert)
        {
            return true;// typeToConvert.Equals(typeof(JObject));
        }

        public override JObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => JObject.Parse(reader.GetString());

        public override void Write(Utf8JsonWriter writer, JObject value, JsonSerializerOptions options) => writer.WriteRawValue(value.ToString());
        
    }
}
