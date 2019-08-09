using System;
using System.Collections.Generic;
using System.Text;

namespace AdaptiveBlocks.Commands
{
    public class AdaptiveOpenUrlCommand : BaseAdaptiveCommand
    {
        public override string Type => "Command.OpenUrl";

        public string Url { get; set; }
    }
}
