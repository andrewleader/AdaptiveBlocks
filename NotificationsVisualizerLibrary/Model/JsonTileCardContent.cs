using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal class JsonTileCardContent
    {
        public ExpandoObject[] Content { get; set; }

        public JsonTileBackgroundImage BackgroundImage { get; set; }

        public JsonTilePeekContent PeekContent { get; set; }

        public JsonTileBranding? Branding { get; set; }

        public JsonTileDisplayName DisplayName { get; set; }

        public string Arguments { get; set; }

        public Uri BaseUrl { get; set; }

        public bool? AddImageQuery { get; set; }

        public string Lang { get; set; }
    }
}
