using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Attributes
{
    [JsonConverter(typeof(JsonStringToObjectConverter<AdaptiveBlockAttributionText>))]
    public class AdaptiveBlockAttributionText
    {
        [JsonImplicitText]
        public string Text { get; set; }
    }
}
