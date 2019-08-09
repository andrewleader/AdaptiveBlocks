using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using AdaptiveBlocks.Attributes;
using AdaptiveBlocks.Visualizer.Uwp.Renderer;
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
    public sealed partial class PreviewRecentActivities : UserControl, IPreviewBlockHost
    {
        public PreviewRecentActivities()
        {
            this.InitializeComponent();
        }

        public void Update(AdaptiveBlockContent block, AdaptiveBlock sourceBlock, PreviewBlockHostViewModel args)
        {
            TextBlockTitle.Text = block.Title;
            TextBlockSubtitle.Text = block.Subtitle ?? sourceBlock.View.Attributes.AttributionText?.Text ?? args.AppName;
            TextBlockTimestamp.Text = "Opened " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

            var img = block.GetIconImageOrBestFit();
            if (img != null)
            {
                ImageOrIcon.Source = img.CreateImageSource();
            }
            else
            {
                var attrIcon = sourceBlock.View.Attributes.AttributionIcon?.GetIconForTheme(AdaptiveThemes.Dark);
                if (attrIcon != null)
                {
                    ImageOrIcon.Source = attrIcon.CreateImageSource();
                }
                else
                {
                    ImageOrIcon.Source = args.GetAppLogoImageSource();
                }
            }
        }
    }
}
