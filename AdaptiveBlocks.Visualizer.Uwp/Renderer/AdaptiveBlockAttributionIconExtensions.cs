using AdaptiveBlocks.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace AdaptiveBlocks.Visualizer.Uwp.Renderer
{
    public static class AdaptiveBlockAttributionIconExtensions
    {
        public static ImageSource CreateImageSource(this AdaptiveBlockAttributionIcon icon)
        {
            return new BitmapImage(new Uri(icon.Url));
        }
    }
}
