#if CARDS
using AdaptiveCards.Rendering.Uwp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NotificationsVisualizerLibrary.Controls;
using NotificationsVisualizerLibrary.Helpers;
using NotificationsVisualizerLibrary.Parsers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Data.Json;
using AdaptiveBlocks.SharedAdaptiveCards.Uwp;

namespace NotificationsVisualizerLibrary.Renderers
{
    internal static class CardRenderer
    {
        private static string HOST_CONFIG;

        static CardRenderer()
        {
            HOST_CONFIG = AssemblyFileHelper.GetResourceText("HostConfigs.ToastHostConfig.json");
        }

        public static bool CardHasContents(dynamic card)
        {
            return JsonParser.HasProperty(card, "body") || JsonParser.HasProperty(card, "actions");
        }

        public static string NormalizeJson(dynamic card, Uri baseUrl, out bool hasActions)
        {
            hasActions = false;
            if (JsonParser.HasProperty(card, "actions"))
            {
                foreach (dynamic a in card.actions)
                {
                    hasActions = true;
                }
            }

            string json = JsonConvert.SerializeObject(card);
            return json;
        }

        public static RenderedAdaptiveCard RenderCard(dynamic card, Uri baseUrl, TypedEventHandler<RenderedAdaptiveCard, AdaptiveActionEventArgs> actionHandler, out string finalCardJson)
        {
            string json = NormalizeJson(card, baseUrl, out bool hasActions);
            finalCardJson = json;

            return Render(json, hasActions, actionHandler);
        }

        public static UIElement RenderBody(ExpandoObject[] body, Uri baseUrl, ExpandoObject[] actions, TypedEventHandler<RenderedAdaptiveCard, AdaptiveActionEventArgs> actionHandler, out string finalCardJson)
        {
            try
            {
                string json = JsonConvert.SerializeObject(body);
                string actionsJson = "";
                bool hasActions = false;
                if (actions != null && actions.Length > 0)
                {
                    actionsJson = @", ""actions"": " + JsonConvert.SerializeObject(actions) + " ";

                    hasActions = true;
                }
                json = @"{ ""type"": ""AdaptiveCard"", ""version"": ""1.0"", ""body"": " + json + actionsJson + " }";
                finalCardJson = json;

                return Render(json, hasActions, actionHandler)?.FrameworkElement;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                finalCardJson = null;
                return null;
            }
        }

        private static RenderedAdaptiveCard Render(string cardJson, bool hasActions, TypedEventHandler<RenderedAdaptiveCard, AdaptiveActionEventArgs> actionHandler = null)
        {
            try
            {
                var renderer = new AdaptiveCardRenderer();

                // Constructing HostConfig from object model isn't supported yet, have to use JSON
                var hostConfigResult = AdaptiveHostConfig.FromJsonString(HOST_CONFIG);
                if (hostConfigResult.HostConfig != null)
                {
                    renderer.HostConfig = hostConfigResult.HostConfig;
                }
                else
                {
                    throw new Exception("HostConfig failed to parse");
                }

                //renderer.SetHostConfig(new AdaptiveHostConfig()
                //{
                //    AdaptiveCard = new AdaptiveCardConfig()
                //    {
                //        BackgroundColor = Colors.Transparent,
                //        Padding = new AdaptiveSpacingDefinition()
                //        {
                //            Bottom = 0,
                //            Left = 0,
                //            Right = 0,
                //            Top = 0
                //        }
                //    },
                //    Colors = new AdaptiveColorsConfig()
                //    {
                //        Default = new AdaptiveColorConfig()
                //        {
                //            Normal = Colors.White,
                //            Subtle = new Color() { A = 153, R = 255, G = 255, B = 255 }
                //        }
                //    }
                //});

                var cardResult = AdaptiveCard.FromJsonString(cardJson);
                if (cardResult.AdaptiveCard == null)
                {
                    throw new Exception("Failed to parse card");
                }

                var result = renderer.RenderAdaptiveCard(cardResult.AdaptiveCard);
                if (result.FrameworkElement == null)
                {
                    throw new Exception("Failed to render card");
                }

                if (actionHandler != null)
                {
                    // Wire up action click handler
                    result.Action += actionHandler;
                }

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                PreviewToast.SendRenderingError(ex);
                return null;
            }
        }

        internal static RenderedAdaptiveCard RenderCard(AdaptiveCard card, TypedEventHandler<RenderedAdaptiveCard, AdaptiveActionEventArgs> actionHandler = null)
        {
            try
            {
                var renderer = SharedAdaptiveCardRenderer.Create();
                var inputTextRenderer = renderer.ElementRenderers.Get("Input.Text");
                renderer.ElementRenderers.Set("Input.Text", new ActionCenterInputTextRenderer(inputTextRenderer));

                // Constructing HostConfig from object model isn't supported yet, have to use JSON
                var hostConfigResult = AdaptiveHostConfig.FromJsonString(HOST_CONFIG);
                if (hostConfigResult.HostConfig != null)
                {
                    renderer.HostConfig = hostConfigResult.HostConfig;
                }
                else
                {
                    throw new Exception("HostConfig failed to parse");
                }

                var result = renderer.RenderAdaptiveCard(card);
                if (result.FrameworkElement == null)
                {
                    throw new Exception("Failed to render card");
                }

                if (actionHandler != null)
                {
                    // Wire up action click handler
                    result.Action += actionHandler;
                }

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                PreviewToast.SendRenderingError(ex);
                return null;
            }
        }

        private class ActionCenterInputTextRenderer : IAdaptiveElementRenderer
        {
            private IAdaptiveElementRenderer _originalRenderer;

            public ActionCenterInputTextRenderer(IAdaptiveElementRenderer originalRenderer)
            {
                _originalRenderer = originalRenderer;
            }

            public UIElement Render(IAdaptiveCardElement element, AdaptiveRenderContext context, AdaptiveRenderArgs renderArgs)
            {
                try
                {
                    var json = element.ToJson();
                    var obj = json.GetNamedObject("-ms-action");
                    return new TextBlock() { Text = "-ms-action" };
                }
                catch { }
                return _originalRenderer.Render(element, context, renderArgs);
            }
        }

        //private static void Renderer_RenderingAction(XamlCardRenderer sender, RenderingActionEventArgs args)
        //{
        //    try
        //    {
        //        args.CustomFrameworkElement = new AppBarButton()
        //        {
        //            Label = args.Action.Title,
        //            Icon = new BitmapIcon()
        //            {
        //                UriSource = new Uri((args.Action as AdaptiveSubmitAction).GetCustomPropertyValueAsString("imageUrl"))
        //            }
        //        };
        //    }
        //    catch { }
        //}

        private static IEnumerable<T> GetDescendantsOfType<T>(UIElement root) where T : UIElement
        {
            foreach (var child in GetChildrenOfType<UIElement>(root))
            {
                if (child is T)
                {
                    yield return child as T;
                }

                foreach (var descendant in GetDescendantsOfType<T>(child))
                {
                    yield return descendant;
                }
            }
        }

        private static IEnumerable<T> GetChildrenOfType<T>(UIElement root) where T : UIElement
        {
            int count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                if (child is T)
                {
                    yield return child as T;
                }
            }
        }
    }
}

#endif