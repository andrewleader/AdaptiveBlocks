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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AdaptiveBlocks.Visualizer.Uwp.Previews
{
    public sealed partial class PreviewTimelineWebFullSize : UserControl, IPreviewBlockHost
    {
        private Lazy<AdaptiveCards.Rendering.Uwp.AdaptiveCardRenderer> _cardRenderer = new Lazy<AdaptiveCards.Rendering.Uwp.AdaptiveCardRenderer>(CreateRenderer);
        private Lazy<AdaptiveCards.Rendering.Uwp.AdaptiveCardRenderer> _cardRendererLight = new Lazy<AdaptiveCards.Rendering.Uwp.AdaptiveCardRenderer>(CreateRendererLight);

        private static AdaptiveCards.Rendering.Uwp.AdaptiveCardRenderer CreateRenderer()
        {
            var renderer = new AdaptiveCards.Rendering.Uwp.AdaptiveCardRenderer();

            string path = Package.Current.InstalledLocation.Path + "\\Previews\\Microsoft.UserActivities.Hero.HostConfig.json";
            renderer.HostConfig = AdaptiveCards.Rendering.Uwp.AdaptiveHostConfig.FromJsonString(File.ReadAllText(path)).HostConfig;

            renderer.SetFixedDimensions(320, 176);

            return renderer;
        }

        private static AdaptiveCards.Rendering.Uwp.AdaptiveCardRenderer CreateRendererLight()
        {
            var renderer = new AdaptiveCards.Rendering.Uwp.AdaptiveCardRenderer();

            string path = Package.Current.InstalledLocation.Path + "\\Previews\\Microsoft.UserActivities.Hero.HostConfig.Light.json";
            renderer.HostConfig = AdaptiveCards.Rendering.Uwp.AdaptiveHostConfig.FromJsonString(File.ReadAllText(path)).HostConfig;

            renderer.SetFixedDimensions(320, 176);

            return renderer;
        }

        public PreviewTimelineWebFullSize()
        {
            this.InitializeComponent();
        }

        public void Update(AdaptiveBlockContent block, AdaptiveBlock sourceBlock, PreviewBlockHostViewModel args)
        {
            DataContext = args;

            GenericContent.Visibility = Visibility.Collapsed;
            CardContent.Child = null;
            CardContent.Visibility = Visibility.Collapsed;
            DocumentContent.Visibility = Visibility.Collapsed;
            MessageTemplate.Visibility = Visibility.Collapsed;
            BackgroundImageBrush.ImageSource = null;
            GenericContent.RequestedTheme = ElementTheme.Light;
            BackgroundOverlay.Visibility = Visibility.Collapsed;
            TextBlockAttribution.Text = args.AppName;
            LogoOrAttributionIcon.Source = args.GetAppLogoImageSource();

            try
            {
                if (sourceBlock.View.Attributes != null)
                {
                    if (sourceBlock.View.Attributes.AttributionText?.Text != null)
                    {
                        TextBlockAttribution.Text = sourceBlock.View.Attributes.AttributionText.Text;
                    }

                    if (sourceBlock.View.Attributes.AttributionIcon?.Url != null)
                    {
                        LogoOrAttributionIcon.Source = new BitmapImage(new Uri(sourceBlock.View.Attributes.AttributionIcon?.Url));
                    }
                }
            }
            catch { }

            try
            {
                var cardObj = sourceBlock.View.RichContent.FirstOrDefault(i => i.TargetedExperiences.Contains("Microsoft.UserActivities.Hero") && i.ContentType == "application/vnd.microsoft.card.adaptive")?.Content;
                if (cardObj != null)
                {
                    string cardJson = cardObj.ToString();
                    var cardParseResult = AdaptiveCards.Rendering.Uwp.AdaptiveCard.FromJsonString(cardJson);
                    string errorsAndWarnings = string.Join("\n", cardParseResult.Errors.Select(i => i.Message));
                    if (cardParseResult.AdaptiveCard != null)
                    {
                        if (!string.IsNullOrWhiteSpace(cardParseResult.AdaptiveCard.BackgroundImage) && Uri.IsWellFormedUriString(cardParseResult.AdaptiveCard.BackgroundImage, UriKind.Absolute))
                        {
                            BackgroundImageBrush.ImageSource = new BitmapImage(new Uri(cardParseResult.AdaptiveCard.BackgroundImage));
                            cardParseResult.AdaptiveCard.BackgroundImage = "";
                            BackgroundOverlay.Visibility = Visibility.Visible;
                        }

                        var renderedElement = (BackgroundImageBrush.ImageSource != null ? _cardRenderer : _cardRendererLight).Value.RenderAdaptiveCard(cardParseResult.AdaptiveCard).FrameworkElement;
                        CardContent.Child = renderedElement;
                        CardContent.Visibility = Visibility.Visible;
                        GenericContent.Visibility = Visibility.Collapsed;
                        return;
                    }
                }
            }
            catch { }

            TextBlockTitle.Text = block.Title ?? "";
            TextBlockSubtitle.Text = block.Subtitle ?? "";

            var backgroundImg = block.GetBackgroundImage();
            if (backgroundImg?.Url != null)
            {
                try
                {
                    BackgroundImageBrush.ImageSource = new BitmapImage(new Uri(backgroundImg.Url));
                    GenericContent.RequestedTheme = ElementTheme.Dark;
                    BackgroundOverlay.Visibility = Visibility.Visible;
                }
                catch { }
            }

            DocumentImageBrush.ImageSource = block.GetHeroImage()?.CreateImageSource();

            if (sourceBlock.Hints.HasCategory("Thing.CreativeWork.DigitalDocument"))
            {
                DocumentContent.Visibility = Visibility.Visible;
            }
            else if (sourceBlock.Hints.HasCategory("Thing.CreativeWork.Message"))
            {
                MessageTemplate.Visibility = Visibility.Visible;
                MessageTemplate.DataContext = PreviewTimelineWebFullSizeTemplates.PreviewTimelineWebFullSizeMessageViewModel.Create(block);
            }
            else
            {
                GenericContent.Visibility = Visibility.Visible;
            }
        }
    }
}
