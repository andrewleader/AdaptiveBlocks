using System;
using System.Collections.Generic;
using System.Text;

namespace AdaptiveBlocks.Commands
{
    public class AdaptiveHttpCommand : BaseAdaptiveCommand
    {
        public string Url { get; set; }

        public string Method { get; set; }

        public override string Type => "Command.Http";
    }
}
