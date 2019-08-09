using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using AdaptiveBlocks.Attributes;
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
    public sealed partial class PreviewJumplist : UserControl, IPreviewBlockHost
    {
        public PreviewJumplist()
        {
            this.InitializeComponent();
        }

        public void Update(AdaptiveBlockContent block, AdaptiveBlock sourceBlock, PreviewBlockHostViewModel args)
        {
            LogoOrAttributionIcon.Source = args.GetAppLogoImageSource();

            TextBlockTitle.Text = block.Text?.FirstOrDefault()?.Text;

            try
            {
                if (sourceBlock.View.Attributes != null)
                {
                    var attrUrl = sourceBlock.View.Attributes.AttributionIcon?.GetIconForTheme(AdaptiveThemes.Dark)?.Url;
                    if (attrUrl != null)
                    {
                        LogoOrAttributionIcon.Source = new BitmapImage(new Uri(attrUrl));
                    }
                }
            }
            catch { }
        }
    }
}
