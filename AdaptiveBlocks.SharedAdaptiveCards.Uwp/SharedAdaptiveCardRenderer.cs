using AdaptiveCards.Rendering.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Xaml;

namespace AdaptiveBlocks.SharedAdaptiveCards.Uwp
{
    public static class SharedAdaptiveCardRenderer
    {
        public static AdaptiveCardRenderer Create()
        {
            var renderer = new AdaptiveCardRenderer();
            renderer.ElementRenderers.Set("ActionSet", new AdaptiveActionSetRenderer(renderer));
            renderer.ElementRenderers.Set("Input.Text", new AdaptiveTextInputRenderer(renderer.ElementRenderers.Get("Input.Text")));
            return renderer;
        }

        public static UIElement RenderFromCard(AdaptiveCards.AdaptiveCard card)
        {
            try
            {
                var uwpCard = AdaptiveCard.FromJsonString(card.ToJson()).AdaptiveCard;

                var renderer = Create();
                return renderer.RenderAdaptiveCard(uwpCard).FrameworkElement;
            }
            catch (Exception ex)
            {
                return new Windows.UI.Xaml.Controls.TextBlock()
                {
                    Text = ex.ToString(),
                    TextWrapping = TextWrapping.Wrap
                };
            }
        }

        public static AdaptiveCard ConvertToUwpCard(AdaptiveCards.AdaptiveCard sharedCard)
        {
            if (sharedCard == null)
            {
                return null;
            }
            return Parse(sharedCard.ToJson());
        }

        public static AdaptiveCard Parse(string json)
        {
            AdaptiveElementParserRegistration elementRegistration = new AdaptiveElementParserRegistration();
            elementRegistration.Set("ActionSet", new ActionSetParser());

            return AdaptiveCard.FromJsonString(json, elementRegistration, null).AdaptiveCard;
        }
    }

    public class ActionSetParser : IAdaptiveElementParser
    {
        public IAdaptiveCardElement FromJson(JsonObject inputJson, AdaptiveElementParserRegistration elementParsers, AdaptiveActionParserRegistration actionParsers, IList<IAdaptiveWarning> warnings)
        {
            AdaptiveActionSet answer = new AdaptiveActionSet();

            var actionsArray = inputJson.GetNamedArray("actions");

            foreach (JsonObject a in actionsArray.Select(i => i.GetObject()))
            {
                string title = a.GetNamedString("title");
                answer.Actions.Add(new AdaptiveSubmitAction()
                {
                    Title = title
                });

                // AC bug: Default action parsers aren't present: https://github.com/Microsoft/AdaptiveCards/issues/2196
                //string type = a.GetNamedString("type");
                //var actionParser = actionParsers.Get("Action.OpenUrl");
                //if (actionParser != null)
                //{
                //    var action = actionParser.FromJson(a, elementParsers, actionParsers, warnings);
                //    if (action != null)
                //    {
                //        answer.Actions.Add(action);
                //    }
                //}
            }

            return answer;
        }
    }
}
