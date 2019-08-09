using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks
{
    [JsonConverter(typeof(JsonStringToObjectConverter<AdaptiveBlockText>))]
    public class AdaptiveBlockText
    {
        [JsonImplicitText]
        public string Text { get; set; }
    }
}
