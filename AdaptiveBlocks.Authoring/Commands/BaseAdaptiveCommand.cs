using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdaptiveBlocks.Commands
{
    [JsonConverter(typeof(AdaptiveCommandConverter))]
    public abstract class BaseAdaptiveCommand
    {
        [JsonProperty(Order = int.MinValue)]
        public abstract string Type { get; }
    }
}
