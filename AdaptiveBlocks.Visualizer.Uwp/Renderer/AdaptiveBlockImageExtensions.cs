using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace AdaptiveBlocks.Visualizer.Uwp.Renderer
{
    public static class AdaptiveBlockImageExtensions
    {
        public static ImageSource CreateImageSource(this AdaptiveBlockImage img)
        {
            return new BitmapImage(new Uri(img.Url));
        }
    }
}
