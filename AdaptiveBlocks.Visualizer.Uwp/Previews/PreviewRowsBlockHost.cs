using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaptiveBlocks.Transformers.AdaptiveCards;
using AdaptiveBlocks.Visualizer.Uwp.Renderer;

namespace AdaptiveBlocks.Visualizer.Uwp.Previews
{
    public class PreviewRowsBlockHost : BasePreviewBlockHost
    {
        protected override AdaptiveBlockToCardTransformerProperties GetProperties()
        {
            return new AdaptiveBlockToCardTransformerProperties()
            {
                AllowsColumns = false,
                AllowsDetailedBlocks = true,
                AllowsInteractivity = false,
                AllowsFactSets = false
            };
        }
    }
}
