using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using AdaptiveBlocks.SharedAdaptiveCards.Uwp;
using AdaptiveBlocks.Transformers.AdaptiveCards;

using AdaptiveBlocks.Visualizer.Uwp.Renderer;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AdaptiveBlocks.Visualizer.Uwp.Previews
{
    public sealed partial class PreviewOutlookActionableMessage : UserControl, IPreviewBlockHost
    {
        private Lazy<AdaptiveCards.Rendering.Uwp.AdaptiveCardRenderer> _cardRenderer = new Lazy<AdaptiveCards.Rendering.Uwp.AdaptiveCardRenderer>(CreateRenderer);

        private static AdaptiveCards.Rendering.Uwp.AdaptiveCardRenderer CreateRenderer()
        {
            var renderer = SharedAdaptiveCardRenderer.Create();

            string path = Package.Current.InstalledLocation.Path + "\\Previews\\Microsoft.Outlook.ActionableMessage.HostConfig.json";
            renderer.HostConfig = AdaptiveCards.Rendering.Uwp.AdaptiveHostConfig.FromJsonString(File.ReadAllText(path)).HostConfig;

            return renderer;
        }

        public PreviewOutlookActionableMessage()
        {
            this.InitializeComponent();
        }

        public async void Update(AdaptiveBlockContent block, AdaptiveBlock sourceBlock, PreviewBlockHostViewModel args)
        {
            AdaptiveCardContainer.Child = null;
            AdaptiveCards.Rendering.Uwp.AdaptiveCard card = null;

            try
            {
                var cardObj = sourceBlock.View.RichContent.FirstOrDefault(i => i.TargetedExperiences.Contains("Microsoft.Outlook.ActionableMessage") && i.ContentType == "application/vnd.microsoft.card.adaptive")?.Content;
                if (cardObj != null)
                {
                    string cardJson = cardObj.ToString();
                    var parsedCard = SharedAdaptiveCardRenderer.Parse(cardJson);
                    if (parsedCard != null)
                    {
                        card = parsedCard;
                    }
                }
            }
            catch { }

            if (card == null)
            {
                try
                {
                    var transformer = new AdaptiveBlockToCardTransformer()
                    {
                        Properties = new AdaptiveBlockToCardTransformerProperties()
                    };
                    var sharedCard = (await transformer.TransformAsync(sourceBlock)).Result;

                    card = SharedAdaptiveCardRenderer.ConvertToUwpCard(sharedCard);
                }
                catch
                {

                }
            }

            try
            {
                if (card != null)
                {
                    AdaptiveCardContainer.Child = _cardRenderer.Value.RenderAdaptiveCard(card).FrameworkElement;
                }
            }
            catch { }
        }
    }
}
