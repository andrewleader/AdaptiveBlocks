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
    public sealed partial class PreviewTimelineWebHighDensity : UserControl, IPreviewBlockHost
    {
        public PreviewTimelineWebHighDensity()
        {
            this.InitializeComponent();
        }

        public void Update(AdaptiveBlockContent block, AdaptiveBlock sourceBlock, PreviewBlockHostViewModel args)
        {
            TextBlockAppNameOrAttribution.Text = args.AppName;
            LogoOrAttributionIcon.Source = args.GetAppLogoImageSource();

            TextBlockTitle.Text = block.Text.FirstOrDefault()?.Text;

            try
            {
                if (sourceBlock.View.Attributes != null)
                {
                    if (sourceBlock.View.Attributes.AttributionText?.Text != null)
                    {
                        TextBlockAppNameOrAttribution.Text = sourceBlock.View.Attributes.AttributionText.Text;
                    }

                    if (sourceBlock.View.Attributes.AttributionIcon?.Url != null)
                    {
                        LogoOrAttributionIcon.Source = new BitmapImage(new Uri(sourceBlock.View.Attributes.AttributionIcon?.Url));
                    }
                }
            }
            catch { }
        }
    }
}
