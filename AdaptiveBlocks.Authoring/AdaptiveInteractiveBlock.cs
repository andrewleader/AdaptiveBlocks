using AdaptiveBlocks.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks
{
    public class AdaptiveInteractiveBlock : AdaptiveBlockContent
    {
        public override string Type => "InteractiveBlock";

        public List<BaseAdaptiveBlockElement> Inputs { get; set; } = new List<BaseAdaptiveBlockElement>();

        public List<AdaptiveAction> Actions { get; set; } = new List<AdaptiveAction>();
    }
}
