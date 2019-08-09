
using AdaptiveBlocks.SharedAdaptiveCards.Uwp;
using AdaptiveBlocks.Transformers.AdaptiveCards;
using AdaptiveBlocks.Visualizer.Uwp.Renderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AdaptiveBlocks.Visualizer.Uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TransformedCardPreviewPage : Page
    {
        public static TransformedCardPreviewPage Current { get; private set; }
        private static AdaptiveCards.Rendering.Uwp.AdaptiveCardRenderer m_cardRenderer;
        private static AdaptiveCards.Rendering.Uwp.AdaptiveCardRenderer m_cardRendererCortana;
        private static AdaptiveCards.Rendering.Uwp.AdaptiveCardRenderer m_cardRendererTeams;

        public TransformedCardPreviewPage()
        {
            this.InitializeComponent();


            m_cardRenderer = CreateRenderer("Microsoft.Outlook.ActionableMessage.HostConfig.json");
            m_cardRendererCortana = CreateRenderer("Microsoft.Cortana.BotMessage.HostConfig.json");
            m_cardRendererTeams = CreateRenderer("Microsoft.Teams.ChatMessage.HostConfig.json");

            Current = this;
        }

        private AdaptiveCards.Rendering.Uwp.AdaptiveCardRenderer CreateRenderer(string hostConfigFileName)
        {
            var renderer = SharedAdaptiveCardRenderer.Create();

            string path = Package.Current.InstalledLocation.Path + "\\Previews\\" + hostConfigFileName;
            renderer.HostConfig = AdaptiveCards.Rendering.Uwp.AdaptiveHostConfig.FromJsonString(File.ReadAllText(path)).HostConfig;

            return renderer;
        }

        public static async void RenderNewBlock(AdaptiveBlock block)
        {
            if (Current != null)
            {
                try
                {
                    await Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
                    {
                        try
                        {
                            Current.RenderBlock(block);
                        }
                        catch { }
                    });
                }
                catch { }
            }
        }

        private async void RenderBlock(AdaptiveBlock block)
        {
            var transformer = new AdaptiveBlockToCardTransformer();

            var sharedCard = (await transformer.TransformAsync(block)).Result;
            if (sharedCard != null)
            {
                TextBoxCardPayload.Text = sharedCard.ToJson();
            }

            var uwpCard = SharedAdaptiveCardRenderer.ConvertToUwpCard(sharedCard);

            RenderCard(CardPreview, uwpCard, m_cardRenderer);
            RenderCard(CardPreviewTeams, uwpCard, m_cardRendererTeams);
            RenderCard(CardPreviewCortana, uwpCard, m_cardRendererCortana);
        }

        private void RenderCard(Border cardPreviewContainer, AdaptiveCards.Rendering.Uwp.AdaptiveCard uwpCard, AdaptiveCards.Rendering.Uwp.AdaptiveCardRenderer renderer)
        {
            cardPreviewContainer.Child = renderer.RenderAdaptiveCard(uwpCard).FrameworkElement;
        }
    }
}
