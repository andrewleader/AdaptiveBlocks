using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks
{
    public class AdaptiveBlockRichContent
    {
        public string[] TargetedExperiences { get; set; }

        public string ContentType { get; set; }

        public JToken Content { get; set; }
    }
}
