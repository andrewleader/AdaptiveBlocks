using AdaptiveCards.Rendering.Uwp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal class JsonToastContent
    {
        public string Type { get; set; }
        public string Version { get; set; }

        public JsonToastTextBlock Title { get; set; }
        public JsonToastTextBlock Subtitle { get; set; }

        public JsonToastImages Images { get; set; }

        public ExpandoObject SelectAction { get; set; }
        
        public JsonToastScenario MsToastScenario { get; set; }
        
        public JsonToastPerson[] MsPeople { get; set; }

        public JsonToastHeader MsHeader { get; set; }

        public JsonToastAttributionText MsAttributionText { get; set; }

        public JsonToastAudio MsAudio { get; set; }

        public DateTime MsTimestamp { get; set; }

        public JsonToastProgressBar[] MsToastProgressBars { get; set; }

        [JsonIgnore]
        public AdaptiveCard Card { get; set; }
    }
}
