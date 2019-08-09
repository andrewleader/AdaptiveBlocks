using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AdaptiveBlocks.Commands
{
    /// <summary>
    ///     This handles using type field to instantiate strongly typed object on deserialization
    /// </summary>
    public class AdaptiveCommandConverter : Newtonsoft.Json.JsonConverter
    {
        /// <summary>
        /// Default types to support, register any new types to this list 
        /// </summary>
        private static readonly Lazy<Dictionary<string, Type>> TypedElementTypes = new Lazy<Dictionary<string, Type>>(() =>
        {
            // TODO: Should this be a static? It makes it impossible to have diff renderers support different elements
            var types = new Dictionary<string, Type>
            {
                ["Command.Http"] = typeof(AdaptiveHttpCommand),
                ["Command.OpenUrl"] = typeof(AdaptiveOpenUrlCommand)
            };
            return types;
        });

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return typeof(BaseAdaptiveCommand).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);

            var typeName = jObject["type"]?.Value<string>() ?? jObject["Type"]?.Value<string>();
            if (typeName == null)
            {
                throw new Exception("Required property 'type' not found on command");
            }

            if (TypedElementTypes.Value.TryGetValue(typeName, out var type))
            {
                var result = (BaseAdaptiveCommand)Activator.CreateInstance(type);
                serializer.Populate(jObject.CreateReader(), result);

                return result;
            }

            //Warnings.Add(new AdaptiveWarning(-1, $"Unknown element '{typeName}'"));
            return null;
        }

        public static T CreateElement<T>(string typeName = null)
            where T : BaseAdaptiveBlockElement
        {
            if (typeName == null)
                typeName = ((T)Activator.CreateInstance(typeof(T))).Type;

            if (TypedElementTypes.Value.TryGetValue(typeName, out var type))
            {
                return (T)Activator.CreateInstance(type);
            }
            return default(T);
        }
    }
}
