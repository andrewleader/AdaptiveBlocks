using AdaptiveCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Transformers.AdaptiveCards
{

    public class AdaptiveActionSet : AdaptiveElement
    {
        public List<AdaptiveAction> Actions { get; set; } = new List<AdaptiveAction>();

        public override string Type { get => "ActionSet"; set { } }
    }
}
