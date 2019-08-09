using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks
{

    //[JsonConverter(typeof(AdaptiveBlockImageConverter))]
    public class AdaptiveBlockImage
    {
        public string Url { get; set; }

        public AdaptiveBlockImageHints Hints { get; set; } = new AdaptiveBlockImageHints();

        public string BackgroundColor { get; set; }
    }

    public class AdaptiveBlockImageHints
    {
        public List<AdaptiveBlockImageCategoryHints> Category { get; set; } = new List<AdaptiveBlockImageCategoryHints>();

        public List<AdaptiveBlockImagePlacementHints> Placement { get; set; } = new List<AdaptiveBlockImagePlacementHints>();
    }

    public enum AdaptiveBlockImageCategoryHints
    {
        Featured,
        Thumbnail,
        Document,
        Profile,
        Icon
    }

    public enum AdaptiveBlockImagePlacementHints
    {
        Default,
        Left,
        
    }

    public class AdaptiveBlockImageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AdaptiveBlockImage);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.String)
            {
                return new AdaptiveBlockImage()
                {
                    Url = token.Value<string>()
                };
            }
            // This causes infinite loop
            return token.ToObject<AdaptiveBlockImage>(serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }

    public class AdaptiveBlockImagesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            JArray array = (JArray)token;

            List<AdaptiveBlockImage> answer = new List<AdaptiveBlockImage>();

            foreach (JToken t in array)
            {
                if (t.Type == JTokenType.String)
                {
                    answer.Add(new AdaptiveBlockImage()
                    {
                        Url = t.Value<string>()
                    });
                }
                else
                {
                    try
                    {
                        answer.Add(t.ToObject<AdaptiveBlockImage>(serializer));
                    }
                    catch { }
                }
            }

            return answer;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
