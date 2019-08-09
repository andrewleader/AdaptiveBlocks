using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdaptiveBlocks
{
    public class AdaptiveBlock
    {
        public JObject Data { get; set; }

        public object Entity { get; set; }

        [JsonIgnore]
        public AdaptiveBlockView View => (ViewReference as AdaptiveBlockEmbeddedReference<AdaptiveBlockView>)?.Value;

        [JsonProperty(PropertyName = "view")]
        [JsonConverter(typeof(AdaptiveBlockReferenceConverter<AdaptiveBlockView>))]
        private AdaptiveBlockReference<AdaptiveBlockView> ViewReference { get; set; }

        public AdaptiveBlock() { }

        public AdaptiveBlock(AdaptiveBlockContent content)
        {
            ViewReference = new AdaptiveBlockEmbeddedReference<AdaptiveBlockView>()
            {
                Value = new AdaptiveBlockView()
                {
                    Content = content
                }
            };
        }

        public bool RemoveBlock(AdaptiveBlock blockToDelete)
        {
            if (blockToDelete == this)
            {
                throw new InvalidOperationException("Cannot delete self!");
            }

            var detailedBlocks = View?.Content?.DetailedBlocks;
            if (detailedBlocks != null)
            {
                if (detailedBlocks.Remove(blockToDelete))
                {
                    return true;
                }

                foreach (var childBlock in detailedBlocks)
                {
                    if (childBlock.RemoveBlock(blockToDelete))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Optional { get; set; }

        public AdaptiveBlockTopLevelHints Hints { get; set; } = new AdaptiveBlockTopLevelHints();

        public class AdaptiveBlockTopLevelHints : BaseAdaptiveBlockElementHints
        {
            public List<string> Category { get; set; } = new List<string>();

            public bool HasCategory(string category)
            {
                return Category.Any(i => i.StartsWith(category));
            }
        }

        public AdaptiveBlock GetFirstFinalBlock()
        {
            return GetFinalBlocks().First();
        }

        /// <summary>
        /// Returns the final blocks. If no detailed blocks, returns self.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AdaptiveBlock> GetFinalBlocks()
        {
            var detailedBlocks = View?.Content?.DetailedBlocks;
            if (detailedBlocks != null && detailedBlocks.Count > 0)
            {
                foreach (var detailed in detailedBlocks)
                {
                    foreach (var finalBlock in detailed.GetFinalBlocks())
                    {
                        yield return finalBlock;
                    }
                }
            }
            else
            {
                yield return this;
            }
        }

        public bool ConsumeBlockDescendant(AdaptiveBlock block)
        {
            if (block == this)
            {
                throw new InvalidOperationException("Can't consume yourself!");
            }

            var detailedBlocks = View?.Content?.DetailedBlocks;
            if (detailedBlocks != null)
            {
                if (detailedBlocks.Remove(block))
                {
                    return true;
                }

                foreach (var detailed in detailedBlocks)
                {
                    if (detailed.ConsumeBlockDescendant(block))
                    {
                        if (detailed.View.Content.DetailedBlocks.Count == 0)
                        {
                            detailedBlocks.Remove(detailed);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        public static AdaptiveBlockParseResult Parse(string json)
        {
            var result = new AdaptiveBlockParseResult();

            try
            {
                result.Block = JsonConvert.DeserializeObject<AdaptiveBlock>(json);

                //int startingColumnIndex = -1;
                //for (int i = 0; i < result.Block.DetailedBlocks.Count; i++)
                //{
                //    var b = result.Block.DetailedBlocks[i];

                //    if (b.GetHints().Column)
                //    {
                //        if (startingColumnIndex == -1)
                //        {
                //            startingColumnIndex = i;
                //        }
                //    }
                //    else
                //    {
                //        if (startingColumnIndex != -1)
                //        {
                //            AdaptiveBlockGroup group = new AdaptiveBlockGroup();
                //            group.Hints.Category.Add(AdaptiveBlockGroupCategoryHints.Columns);

                //            for (int x = startingColumnIndex; x < i; x++)
                //            {
                //                group.Blocks.Add(result.Block.DetailedBlocks[x]);
                //            }

                //            result.Block.DetailedBlocks.RemoveRange(startingColumnIndex, i - startingColumnIndex);
                //            result.Block.DetailedBlocks.Insert(startingColumnIndex, group);
                //            i = startingColumnIndex + 1;
                //            startingColumnIndex = -1;
                //        }
                //    }
                //}

                //if (startingColumnIndex != -1)
                //{
                //    AdaptiveBlockGroup group = new AdaptiveBlockGroup();
                //    group.Hints.Category.Add(AdaptiveBlockGroupCategoryHints.Columns);

                //    for (int x = startingColumnIndex; x < result.Block.DetailedBlocks.Count; x++)
                //    {
                //        group.Blocks.Add(result.Block.DetailedBlocks[x]);
                //    }

                //    result.Block.DetailedBlocks.RemoveRange(startingColumnIndex, result.Block.DetailedBlocks.Count - startingColumnIndex);
                //    result.Block.DetailedBlocks.Add(group);
                //}
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ParseError(ParseErrorType.Error, ex.ToString()));
            }

            return result;
        }

        public async Task ResolveRemoteReferencesAsync()
        {
            ViewReference = await ViewReference.ResolveAsync();
        }

        public void ResolveDataBinding()
        {
            if (Data == null)
            {
                return;
            }

            ViewReference = new AdaptiveBlockEmbeddedReference<AdaptiveBlockView>()
            {
                Value = ResolveDataBinding(View)
            };
        }

        private string GetDataFieldValue(string dataField)
        {
            if (Data != null)
            {
                if (Data.TryGetValue(dataField, out JToken value))
                {
                    return value.Value<string>();
                }
            }

            return null;
        }

        private T ResolveDataBinding<T>(T source)
        {
            try
            {
                string json = JsonConvert.SerializeObject(source, new JsonSerializerSettings()
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                });

                var regex = new Regex(@"{{(\w+)}}");

                var matches = regex.Matches(json);

                var dataFields = matches.OfType<Match>().Select(i => i.Groups[1].Value).Distinct();

                foreach (string dataField in dataFields)
                {
                    string value = GetDataFieldValue(dataField);

                    if (value != null)
                    {
                        json = json.Replace("{{" + dataField + "}}", value);
                    }
                }

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                return source;
            }
        }

        public async Task ResolveAsync()
        {
            await ResolveRemoteReferencesAsync();

            ResolveDataBinding();
        }

        public string ToJson()
        {
            var serializer = new JsonSerializer()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new ShouldSerializeContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            serializer.Converters.Add(new StringEnumConverter(namingStrategy: new CamelCaseNamingStrategy()));

            JToken jsonObj = JToken.FromObject(this, serializer);

            jsonObj = JsonHelper.RemoveEmptyChildren(jsonObj);

            return jsonObj.ToString();
        }

        private static class JsonHelper
        {
            // https://stackoverflow.com/a/36902293/1454643

            public static JToken RemoveEmptyChildren(JToken token)
            {
                if (token.Type == JTokenType.Object)
                {
                    JObject copy = new JObject();
                    foreach (JProperty prop in token.Children<JProperty>())
                    {
                        JToken child = prop.Value;
                        if (child.HasValues)
                        {
                            child = RemoveEmptyChildren(child);
                        }
                        if (!IsEmptyOrDefault(child))
                        {
                            copy.Add(prop.Name, child);
                        }
                    }
                    return copy;
                }
                else if (token.Type == JTokenType.Array)
                {
                    JArray copy = new JArray();
                    foreach (JToken item in token.Children())
                    {
                        JToken child = item;
                        if (child.HasValues)
                        {
                            child = RemoveEmptyChildren(child);

                            if (child is JObject childObj && childObj.Properties().Count() == 1)
                            {
                                var firstProp = childObj.Properties().First();
                                if (firstProp.Value.Type == JTokenType.String)
                                {
                                    switch (firstProp.Name)
                                    {
                                        case "text":
                                        case "url":
                                            child = firstProp.Value;
                                            break;
                                    }
                                }
                            }
                        }
                        if (!IsEmptyOrDefault(child))
                        {
                            copy.Add(child);
                        }
                    }
                    return copy;
                }
                return token;
            }

            public static bool IsEmptyOrDefault(JToken token)
            {
                return (token.Type == JTokenType.Array && !token.HasValues) ||
                       (token.Type == JTokenType.Object && !token.HasValues) ||
                       (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                       (token.Type == JTokenType.Boolean && token.Value<bool>() == false) ||
                       (token.Type == JTokenType.Integer && token.Value<int>() == 0) ||
                       (token.Type == JTokenType.Float && token.Value<double>() == 0.0) ||
                       (token.Type == JTokenType.Null);
            }
        }

        public override string ToString()
        {
            return ToJson();
        }

        public class ShouldSerializeContractResolver : CamelCasePropertyNamesContractResolver
        {
            public static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                // Skip empty lists: https://stackoverflow.com/a/52528338/1454643
                JsonProperty property = base.CreateProperty(member, memberSerialization);

                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                    if (property.PropertyType.Name.StartsWith(nameof(IEnumerable)) || typeof(ICollection).IsAssignableFrom(property.PropertyType))
                        property.ShouldSerialize =
                            instance => EnumerableHasElements(instance?.GetType().GetProperty(property.UnderlyingName).GetValue(instance) as IEnumerable);

                return property;
            }

            private static bool EnumerableHasElements(IEnumerable enumerable)
            {
                return enumerable != null && enumerable.GetEnumerator().MoveNext();
            }
        }
    }
}
