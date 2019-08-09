using AdaptiveBlocks.Transformers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class PreviewEchoSpot : UserControl, IPreviewBlockHost
    {
        public PreviewEchoSpot()
        {
            this.InitializeComponent();
        }

        public void Update(AdaptiveBlockContent block, AdaptiveBlock sourceBlock, PreviewBlockHostViewModel args)
        {
            var viewModel = new PreviewEchoSpotViewModel();

            var consumer = new AdaptiveBlockConsumer(AdaptiveBlock.Parse(sourceBlock.ToJson()).Block, new AdaptiveBlockConsumerProperties());

            if (consumer.ConsumeContent(out var contentConsumer))
            {
                if (contentConsumer.ConsumeText(out var text))
                {
                    viewModel.Title = text.Text;
                }

                if (contentConsumer.ConsumeText(out var textSubtitle))
                {
                    viewModel.Subtitle = textSubtitle.Text;
                }

                // In this case, if we only have a profile image, we'd rather use it as the background image
                // But if we have something to use as both the background and profile, then profile should be used.

                var backgroundImageRequest = AdaptiveBlockContentConsumer.ImageMatchRequest.ForBackgroundImage();

                var profileOrIconImageRequest = new AdaptiveBlockContentConsumer.ImageMatchRequest()
                {
                    RequiredHint =
                    {
                        AdaptiveBlockImageCategoryHints.Profile,
                        AdaptiveBlockImageCategoryHints.Icon
                    }
                };

                contentConsumer.ConsumeImages(backgroundImageRequest, profileOrIconImageRequest);

                if (backgroundImageRequest.ImageResult != null)
                {
                    viewModel.BackgroundImage = backgroundImageRequest.ImageResult.Url;
                }

                if (profileOrIconImageRequest.ImageResult != null)
                {
                    viewModel.ProfileImage = profileOrIconImageRequest.ImageResult.Url;
                }
            }

            DataContext = viewModel;

            //DataContext = sourceBlock;
        }

        public class PreviewEchoSpotViewModel
        {
            public string Title { get; set; }

            public string Subtitle { get; set; }

            public string BackgroundImage { get; set; }

            public string ProfileImage { get; set; }
        }
    }
}
