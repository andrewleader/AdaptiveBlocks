using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Actions
{
    public class BaseAdaptiveActionConverter : JsonConverter
    {
        /// <summary>
        /// Default types to support, register any new types to this list 
        /// </summary>
        private static readonly Lazy<Dictionary<string, Type>> TypedElementTypes = new Lazy<Dictionary<string, Type>>(() =>
        {
            // TODO: Should this be a static? It makes it impossible to have diff renderers support different elements
            var types = new Dictionary<string, Type>
            {
                ["Action"] = typeof(AdaptiveAction),
                ["SharedInputActions"] = typeof(AdaptiveSharedInputActions)
            };
            return types;
        });

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return typeof(BaseAdaptiveAction).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);

            var typeName = jObject["type"]?.Value<string>() ?? jObject["@type"]?.Value<string>();
            if (typeName == null)
            {
                typeName = "Action";
            }

            if (TypedElementTypes.Value.TryGetValue(typeName, out var type))
            {
                var result = (BaseAdaptiveAction)Activator.CreateInstance(type);
                serializer.Populate(jObject.CreateReader(), result);

                HandleAdditionalProperties(result);
                return result;
            }

            //Warnings.Add(new AdaptiveWarning(-1, $"Unknown element '{typeName}'"));
            return null;
        }

        private void HandleAdditionalProperties(BaseAdaptiveAction te)
        {
            // https://stackoverflow.com/questions/34995406/nullvaluehandling-ignore-influences-deserialization-into-extensiondata-despite

            // The default behavior of JsonExtensionData is to include properties if the VALUE could not be set, including abstract properties or default values
            // We don't want to deserialize any properties that exist on the type into AdditionalProperties, so this removes them
            //te.AdditionalProperties
            //    .Select(prop => te.GetType().GetRuntimeProperties()
            //        .SingleOrDefault(p => p.Name.Equals(prop.Key, StringComparison.OrdinalIgnoreCase)))
            //    .Where(p => p != null)
            //    .ToList()
            //    .ForEach(p => te.AdditionalProperties.Remove(p.Name));

            //foreach (var prop in te.AdditionalProperties)
            //{
            //    Warnings.Add(new AdaptiveWarning(-1, $"Unknown property '{prop.Key}' on '{te.Type}'"));
            //}
        }

        public static T CreateElement<T>(string typeName = null)
            where T : BaseAdaptiveAction
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
