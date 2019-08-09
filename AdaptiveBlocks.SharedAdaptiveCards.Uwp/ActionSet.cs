using AdaptiveCards.Rendering.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AdaptiveBlocks.SharedAdaptiveCards.Uwp
{

    public class AdaptiveActionSet : IAdaptiveCardElement
    {
        public List<IAdaptiveActionElement> Actions { get; set; } = new List<IAdaptiveActionElement>();

        public JsonObject ToJson()
        {
            throw new NotImplementedException();
            //var answer = new JsonObject();
            //answer.SetNamedValue("type", ElementTypeString);
            //answer.SetNamedValue
        }

        public JsonObject AdditionalProperties { get; set; }

        public ElementType ElementType => ElementType.Custom;

        public string ElementTypeString => "ActionSet";

        public HeightType Height { get; set; }
        public string Id { get; set; }
        public bool Separator { get; set; }
        public Spacing Spacing { get; set; }
    }

    public class AdaptiveActionSetRenderer : IAdaptiveElementRenderer
    {
        private AdaptiveCardRenderer m_cardRenderer;

        public AdaptiveActionSetRenderer(AdaptiveCardRenderer cardRenderer)
        {
            m_cardRenderer = cardRenderer;
        }

        public UIElement Render(IAdaptiveCardElement element, AdaptiveRenderContext context, AdaptiveRenderArgs renderArgs)
        {
            var card = new AdaptiveCard();
            foreach (var a in (element as AdaptiveActionSet).Actions)
            {
                card.Actions.Add(a);
            }
            
            var originalPadding = m_cardRenderer.HostConfig.Spacing.Padding;
            m_cardRenderer.HostConfig.Spacing.Padding = 0;
            var result = m_cardRenderer.RenderAdaptiveCard(card).FrameworkElement;
            m_cardRenderer.HostConfig.Spacing.Padding = originalPadding;

            return result;
        }
    }
}
