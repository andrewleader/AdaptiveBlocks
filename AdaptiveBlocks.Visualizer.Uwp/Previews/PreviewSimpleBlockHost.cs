using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaptiveBlocks.Transformers.AdaptiveCards;

using AdaptiveBlocks.Visualizer.Uwp.Renderer;
using Windows.UI.Xaml.Controls;

namespace AdaptiveBlocks.Visualizer.Uwp.Previews
{
    public class PreviewSimpleBlockHost : BasePreviewBlockHost
    {
        protected override AdaptiveBlockToCardTransformerProperties GetProperties()
        {
            return new AdaptiveBlockToCardTransformerProperties()
            {
                AllowsDetailedBlocks = false,
                AllowsColumns = false,
                AllowsInteractivity = false,
                AllowsFactSets = false
            };
        }
    }
}
