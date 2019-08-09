using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks
{
    public abstract class AdaptiveBlockReference<T>
    {
        public abstract Task<AdaptiveBlockEmbeddedReference<T>> ResolveAsync();
    }

    public class AdaptiveBlockReferenceConverter<T> : JsonConverter
        where T : new()
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.String)
            {
                var answer = new AdaptiveBlockRemoteReference<T>()
                {
                    Url = token.Value<string>()
                };
                return answer;
            }
            else if (token.Type == JTokenType.Object)
            {
                var val = token.ToObject<T>(serializer);
                return new AdaptiveBlockEmbeddedReference<T>()
                {
                    Value = val
                };
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is AdaptiveBlockEmbeddedReference<T> embedded)
            {
                serializer.Serialize(writer, embedded.Value);
            }
            else if (value is AdaptiveBlockRemoteReference<T> remote)
            {
                writer.WriteValue(remote.Url);
            }
        }
    }

    public class AdaptiveBlockRemoteReference<T> : AdaptiveBlockReference<T>
    {
        public string Url { get; set; }

        public override async Task<AdaptiveBlockEmbeddedReference<T>> ResolveAsync()
        {
            using (HttpClient c = new HttpClient())
            {
                string json = await c.GetStringAsync(Url);

                return new AdaptiveBlockEmbeddedReference<T>()
                {
                    Value = JsonConvert.DeserializeObject<T>(json)
                };
            }
        }
    }

    public class AdaptiveBlockEmbeddedReference<T> : AdaptiveBlockReference<T>
    {
        public T Value { get; set; }

        public override Task<AdaptiveBlockEmbeddedReference<T>> ResolveAsync()
        {
            return Task.FromResult(this);
        }
    }
}
