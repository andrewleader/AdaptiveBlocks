using AdaptiveBlocks.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks
{
    public class AdaptiveBlockView
    {
        public AdaptiveBlockContent Content { get; set; }

        public AdaptiveBlockRichContent[] RichContent { get; set; }

        public AdaptiveBlockAttributes Attributes { get; set; } = new AdaptiveBlockAttributes();
    }
}
