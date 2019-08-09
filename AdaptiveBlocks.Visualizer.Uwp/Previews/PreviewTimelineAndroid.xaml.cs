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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AdaptiveBlocks.Visualizer.Uwp.Previews
{
    public sealed partial class PreviewTimelineAndroid : UserControl, IPreviewBlockHost
    {
        public PreviewTimelineAndroid()
        {
            this.InitializeComponent();
        }

        public void Update(AdaptiveBlockContent block, AdaptiveBlock sourceBlock, PreviewBlockHostViewModel args)
        {
            DataContext = args;

            RectangleThumbnail.Visibility = Visibility.Collapsed;

            TextBlockTitle.Text = block.Title;

            TextBlockAttributionOrSubtitle.Text = block.Subtitle ?? sourceBlock.View.Attributes?.AttributionText?.Text ?? "";

            if (block.FirstImage?.Url != null)
            {
                try
                {
                    ImageBrushThumbnail.ImageSource = new BitmapImage(new Uri(block.FirstImage.Url));
                    RectangleThumbnail.Visibility = Visibility.Visible;
                }
                catch { }
            }
        }
    }
}
