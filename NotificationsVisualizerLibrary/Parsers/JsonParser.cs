using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NotificationsVisualizerLibrary.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Dynamic;
using Windows.Data.Xml.Dom;
using NotificationsVisualizerLibrary.Renderers;
using Windows.UI.Popups;
using AdaptiveCards.Rendering.Uwp;
using Windows.Data.Json;
using AdaptiveBlocks.SharedAdaptiveCards.Uwp;

namespace NotificationsVisualizerLibrary.Parsers
{
    internal static class JsonParser
    {
        /// <summary>
        /// 5 KB (5120 bytes) is too much, only up to 5119 bytes are allowed
        /// </summary>
        private const int PAYLOAD_SIZE_LIMIT = 5119;

        internal static bool IsJson(string text)
        {
            return text.TrimStart().StartsWith("{");
        }

        internal static JsonParseToastResult ParseToast(string json, FeatureSet currFeatureSet)
        {
            JsonToastContent toastContent = null;

            JsonParseToastResult result = new JsonParseToastResult();

            int payloadSize = System.Text.Encoding.UTF8.GetByteCount(json);

            if (payloadSize > PAYLOAD_SIZE_LIMIT)
                result.Errors.Add(new ParseError(ParseErrorType.ErrorButRenderAllowed, $"Your payload exceeds the 5 KB size limit (it is {payloadSize.ToString("N0")} Bytes). Please reduce your payload, or else Windows will not display it."));

            var settings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Error,
                Error = new EventHandler<ErrorEventArgs>((sender, args) =>
                {
                    HandleError(result.Errors, args);
                }),
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new JsonToastNamingStrategy()
                }
            };

            try
            {
                toastContent = JsonConvert.DeserializeObject<JsonToastContent>(json, settings);

                if (toastContent.Type != "AdaptiveCard")
                {
                    result.Errors.Add(new ParseError(ParseErrorType.ErrorButRenderAllowed, "\"type\": \"AdaptiveCard\" must be specified in your payload."));
                }
                if (toastContent.Version == null)
                {
                    result.Errors.Add(new ParseError(ParseErrorType.ErrorButRenderAllowed, "\"version\" property must be specified in your payload."));
                }

                var actionReg = new AdaptiveActionParserRegistration();
                // Can't override the Submit parser so can't know whether dev actually sent -ms-systemActivationType="dismiss" or if we translated it
                actionReg.Set("MsAction.Dismiss", new DismissActionParser(result.Errors));
                actionReg.Set("MsAction.Snooze", new SnoozeActionParser(result.Errors));


                AdaptiveElementParserRegistration elementRegistration = new AdaptiveElementParserRegistration();
                elementRegistration.Set("ActionSet", new ActionSetParser());

                var cardParseResult = AdaptiveCard.FromJsonString(json, elementRegistration, actionReg);
                toastContent.Card = cardParseResult.AdaptiveCard;

                foreach (var error in cardParseResult.Errors)
                {
                    result.Errors.Add(new ParseError(ParseErrorType.ErrorButRenderAllowed, error.Message));
                }
                foreach (var warning in cardParseResult.Warnings)
                {
                    result.Errors.Add(new ParseError(ParseErrorType.Warning, warning.Message));
                }
            }
            catch (Exception ex)
            {
                // Json parse exceptions are handled by the error handler and already reported
                if (!(ex is JsonReaderException))
                {
                    result.Errors.Add(new ParseError(ParseErrorType.Error, ex.Message));
                }
            }

            result.Content = toastContent;

            return result;
        }

        private class DismissActionParser : BaseCustomActionParser
        {
            public DismissActionParser(IList<ParseError> errors) : base(errors) { }

            protected override string[] KnownProperties => new string[]
            {
                "title"
            };

            protected override IAdaptiveActionElement ParseFromJson(JsonObject inputJson, AdaptiveElementParserRegistration elementParsers, AdaptiveActionParserRegistration actionParsers)
            {
                // If developer didn't provide title, use system provided title
                string title = GetString(inputJson, "title") ?? "Dismiss";

                return new AdaptiveSubmitAction()
                {
                    Title = title,
                    AdditionalProperties = new JsonObject()
                    {
                        { "-ms-systemActivationType", JsonValue.CreateStringValue("dismiss") }
                    }
                };
            }
        }

        private class SnoozeActionParser : BaseCustomActionParser
        {
            protected override string[] KnownProperties => new string[]
            {
                "title",
                "snoozeTimeInputId"
            };

            public SnoozeActionParser(IList<ParseError> errors) : base(errors) { }

            protected override IAdaptiveActionElement ParseFromJson(JsonObject inputJson, AdaptiveElementParserRegistration elementParsers, AdaptiveActionParserRegistration actionParsers)
            {
                // If developer didn't provide title, use system provided title
                string title = GetString(inputJson, "title") ?? "Snooze";

                // See if developer provided a snooze input id
                string inputId = GetString(inputJson, "snoozeTimeInputId") ?? "";

                return new AdaptiveSubmitAction()
                {
                    Title = title,
                    AdditionalProperties = new JsonObject()
                    {
                        { "-ms-systemActivationType", JsonValue.CreateStringValue("snooze") },
                        { "snoozeTimeInputId", JsonValue.CreateStringValue(inputId) }
                    }
                };
            }
        }

        private abstract class BaseCustomActionParser : IAdaptiveActionParser
        {
            protected IList<ParseError> Errors { get; private set; }

            public BaseCustomActionParser(IList<ParseError> errors)
            {
                Errors = errors;
            }

            protected abstract string[] KnownProperties { get; }

            public IAdaptiveActionElement FromJson(JsonObject inputJson, AdaptiveElementParserRegistration elementParsers, AdaptiveActionParserRegistration actionParsers, IList<IAdaptiveWarning> warnings)
            {
                WarnAboutUnknownProperties(inputJson);

                return ParseFromJson(inputJson, elementParsers, actionParsers);
            }

            protected abstract IAdaptiveActionElement ParseFromJson(JsonObject inputJson, AdaptiveElementParserRegistration elementParsers, AdaptiveActionParserRegistration actionParsers);

            private void WarnAboutUnknownProperties(JsonObject inputJson)
            {
                if (KnownProperties == null)
                {
                    throw new NotImplementedException("Implementing class must return known properties");
                }

                WarnAboutUnknownProperties(inputJson, KnownProperties);
            }

            private void WarnAboutUnknownProperties(JsonObject inputJson, string[] knownProperties)
            {
                string type = GetActionType(inputJson);

                foreach (string key in inputJson.Keys)
                {
                    // Properties are case-sensitive
                    if (!knownProperties.Contains(key) && key != "type")
                    {
                        Errors.Add(new ParseError(ParseErrorType.Warning, $"Unknown property {key} on {type}."));
                    }
                }
            }

            /// <summary>
            /// Returns null if not found.
            /// </summary>
            /// <param name="inputJson"></param>
            /// <param name="propertyName"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            protected string GetString(JsonObject inputJson, string propertyName)
            {
                if (inputJson.TryGetValue(propertyName, out IJsonValue jsonValue))
                {
                    if (jsonValue.ValueType != JsonValueType.String)
                    {
                        Errors.Add(new ParseError(ParseErrorType.Warning, $"{propertyName} property on {GetActionType(inputJson)} should be a string, but it was {jsonValue.ValueType}"));
                        return null;
                    }

                    return jsonValue.GetString();
                }

                return null;
            }

            protected string GetActionType(JsonObject inputJson)
            {
                return GetString(inputJson, "type") ?? "UnspecifiedType";
            }
        }

        private class JsonToastNamingStrategy : NamingStrategy
        {
            protected override string ResolvePropertyName(string name)
            {
                if (name.StartsWith("Ms"))
                {
                    return "-ms-" + LowerCamelCase(name.Substring(2));
                }

                return LowerCamelCase(name);
            }

            private static string LowerCamelCase(string name)
            {
                if (name.Length > 0)
                {
                    return char.ToLower(name[0]) + name.Substring(1);
                }

                return name;
            }
        }

        internal static JsonParseTileResult ParseTile(string json, FeatureSet currFeatureSet)
        {
            JsonTileContent tileContent = null;

            JsonParseTileResult result = new JsonParseTileResult();

            var settings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Error,
                Error = new EventHandler<ErrorEventArgs>((sender, args) =>
                {
                    HandleError(result.Errors, args);
                })
            };

            try
            {
                tileContent = JsonConvert.DeserializeObject<JsonTileContent>(json, settings);

                CompileProperties(tileContent);
            }
            catch (Exception ex)
            {
                RemoveAllButFirstParseError(result.Errors);
            }

            result.Content = tileContent;

            return result;
        }

        private static void RemoveAllButFirstParseError(IList<ParseError> errors)
        {
            while (errors.Count > 1)
            {
                if (errors[0].Type != ParseErrorType.ErrorButRenderAllowed)
                {
                    errors.RemoveAt(0);
                }
                else
                {
                    errors.RemoveAt(1);
                }
            }
        }

        private static void HandleError(IList<ParseError> errors, ErrorEventArgs errorArgs)
        {
            errorArgs.ErrorContext.Handled = true;

            try
            {
                ErrorContext context = errorArgs.ErrorContext;
                string message = context.Error.Message;

                // Invalid value
                // Error converting value "na" to type 'NotificationsVisualizerLibrary.Model.JsonToastAppLogoImageStyle'. Path 'visual.appLogoOverride.style', line 19, position 25.
                var regInvalidValue = new Regex("^Error converting value (.+) to type '");
                var matchInvalidValue = regInvalidValue.Match(message);
                if (matchInvalidValue.Success)
                {
                    string value = matchInvalidValue.Groups[1].Value;
                    AddError(errors, ParseErrorType.Warning, $@"Invalid value {value} on {context.Member} attribute.", context);
                    return;
                }

                // Unknown property
                // Could not find member 'tacos' on object of type 'JsonToastTextBlock'. Path 'visual.title.tacos', line 10, position 43.
                if (message.StartsWith("Could not find member"))
                {
                    string property = context.Path;
                    int dotIndex = property.LastIndexOf('.');
                    if (dotIndex != -1)
                    {
                        property = property.Substring(dotIndex + 1);
                    }

                    var regexPropertyParent = new Regex("(\\w+)\\.\\w+$");
                    var matchParent = regexPropertyParent.Match(context.Path);

                    string finalMessage = $@"Unknown property ""{property}""";
                    if (matchParent.Success)
                    {
                        finalMessage += $" on {matchParent.Groups[1].Value}.";
                    }
                    else
                    {
                        // Expected unknown properties (since they're Card properties)
                        switch (property)
                        {
                            case "$schema":
                            case "body":
                            case "actions":
                                return;
                        }

                        finalMessage += ".";
                    }

                    AddError(errors, ParseErrorType.Warning, finalMessage, context);
                    return;
                }

                // Otherwise, critical error
                AddError(errors, ParseErrorType.Error, message.Replace("NotificationsVisualizerLibrary.Model.Json", ""), context);
                errorArgs.ErrorContext.Handled = false;
            }
            catch { }
        }

        private static void AddError(IList<ParseError> errors, ParseErrorType type, string message, ErrorContext context)
        {
            // Extract line info
            var regexLine = new Regex(", line (\\d+), position \\d+\\.$");
            var matchLine = regexLine.Match(context.Error.Message);

            var errorPosInfo = new ErrorPositionInfo();
            if (matchLine.Success)
            {
                errorPosInfo.LineNumber = int.Parse(matchLine.Groups[1].Value);
            }

            errors.Add(new Parsers.ParseError(type, message, errorPosInfo));
        }

        private static void CompileProperties(object root)
        {
            CompileProperties(root, new Uri("ms-appx:///"), false);
        }

        private static void CompileProperties(object root, Uri baseUri, bool addImageQuery)
        {
            if (root == null)
            {
                return;
            }

            Uri newBaseUri = GetBaseUri(root);
            if (newBaseUri != null)
            {
                if (!newBaseUri.IsAbsoluteUri)
                {
                    newBaseUri = new Uri(new Uri("ms-appx:///"), newBaseUri);
                    SetBaseUri(root, newBaseUri);
                }

                baseUri = newBaseUri;
            }
            else
            {
                SetBaseUri(root, baseUri);
            }

            bool? newAddImageQuery = GetAddImageQuery(root);
            if (newAddImageQuery != null)
            {
                addImageQuery = newAddImageQuery.Value;
            }

            var urlProp = root.GetType().GetTypeInfo().GetDeclaredProperty("Url");
            if (urlProp != null && urlProp.PropertyType == typeof(string))
            {
                string urlVal = urlProp.GetValue(root) as string;
                if (urlVal != null)
                {
                    // Add base url
                    if (baseUri != null)
                    {
                        // But not if uri if already an absolute URI
                        if (!urlVal.Contains(":"))
                        {
                            urlVal = new Uri(baseUri, urlVal).ToString();
                            urlProp.SetValue(root, urlVal);
                        }
                    }

                    // Add image query
                    if (addImageQuery)
                    {
                        // TODO
                    }
                }
            }

            // Recursively continue compiling for next items
            foreach (var child in GetChildren(root))
            {
                CompileProperties(child, baseUri, addImageQuery);
            }
        }

        private static Uri GetBaseUri(object obj)
        {
            var p = obj?.GetType().GetTypeInfo().GetDeclaredProperty("BaseUrl");
            if (p != null && p.PropertyType == typeof(Uri))
            {
                var baseUri = p.GetValue(obj) as Uri;
                if (baseUri != null)
                {
                    return baseUri;
                }
            }

            return null;
        }

        private static void SetBaseUri(object obj, Uri value)
        {
            var p = obj?.GetType().GetTypeInfo().GetDeclaredProperty("BaseUrl");
            if (p != null && p.PropertyType == typeof(Uri))
            {
                p.SetValue(obj, value);
            }
        }

        private static bool? GetAddImageQuery(object obj)
        {
            var p = obj?.GetType().GetTypeInfo().GetDeclaredProperty("AddImageQuery");
            if (p != null && p.PropertyType == typeof(bool?))
            {
                var addImageQuery = p.GetValue(obj);
                if (addImageQuery != null)
                {
                    return (bool?)addImageQuery;
                }
            }

            return null;
        }

        private static IEnumerable<object> GetChildren(object root)
        {
            if (root == null)
            {
                yield break;
            }

            var properties = root.GetType().GetTypeInfo().DeclaredProperties;

            foreach (var p in properties.Where(p => p.PropertyType.Namespace.StartsWith("NotificationsVisualizerLibrary")))
            {
                var pVal = p.GetValue(root);

                if (pVal != null)
                {
                    yield return pVal;
                }
            }
        }

        public static bool HasProperty(dynamic item, string property)
        {
            return HasProperty(item, property.Split('.'));
        }

        private static bool HasProperty(dynamic item, params string[] propertyPaths)
        {
            if (propertyPaths.Length == 0)
            {
                return true;
            }

            var property = propertyPaths[0];

            if (((IDictionary<string, Object>)item).ContainsKey(property))
            {
                return HasProperty(((IDictionary<string, Object>)item)[property], propertyPaths.Skip(1).ToArray());
            }

            return false;
        }

        public static bool RemoveProperty(dynamic item, string property)
        {
            return ((IDictionary<string, Object>)item).Remove(property);
        }

        public static IEnumerable<KeyValuePair<string, Object>> GetAllProperties(dynamic item)
        {
            return ((IDictionary<string, Object>)item);
        }
    }
}
