using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal class JsonToastProperties
    {
        public JsonToastTextBlock Title { get; set; }

        public JsonToastTextBlock DescriptionLine1 { get; set; }

        public JsonToastTextBlock DescriptionLine2 { get; set; }

        public JsonToastAppLogo AppLogoOverride { get; set; }

        public JsonToastHeroImage HeroImage { get; set; }

        public JsonToastAttributionText Attribution { get; set; }

        public ExpandoObject[] ContextMenuActions { get; set; }

        public ExpandoObject[] ProgressBars { get; set; }

        public Uri BaseUrl { get; set; } = new Uri("ms-appx:///");

        public bool IncludeQueryParameters { get; set; }

        public string Lang { get; set; }

        public JsonToastAudio Audio { get; set; }

        public JsonToastScenario Scenario { get; set; }

        public DateTime Timestamp { get; set; }

        public JsonToastHeader Header { get; set; }
    }
}
