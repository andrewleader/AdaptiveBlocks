using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json.Converters;

namespace AdaptiveBlocks
{
    public class JsonStringToObjectConverter<T> : CustomCreationConverter<T>
        where T : new()
    {
        public override T Create(Type objectType)
        {
            return new T();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                JToken token = JToken.Load(reader);
                var answer = new T();
                var propToSet = typeof(T).GetProperties().FirstOrDefault(i => i.CustomAttributes.Any(x => x.AttributeType == typeof(JsonImplicitTextAttribute)));
                if (propToSet != null)
                {
                    propToSet.SetValue(answer, token.Value<string>());
                }
                return answer;
            }

            return base.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }

    public class JsonImplicitTextAttribute : Attribute
    {

    }
}
