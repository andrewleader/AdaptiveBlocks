using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Actions
{
    [JsonConverter(typeof(BaseAdaptiveActionConverter))]
    public abstract class BaseAdaptiveAction
    {
        public abstract string Type { get; }

        public List<BaseAdaptiveBlockElement> Inputs { get; set; } = new List<BaseAdaptiveBlockElement>();

        public abstract IEnumerable<AdaptiveAction> GetSimplifiedActions();
    }
}
