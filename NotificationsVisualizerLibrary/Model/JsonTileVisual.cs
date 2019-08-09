using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal class JsonTileVisual
    {
        public JsonTileCardContent TileSmall { get; set; }

        public JsonTileCardContent TileMedium { get; set; }

        public JsonTileCardContent TileWide { get; set; }

        public JsonTileCardContent TileLarge { get; set; }

        public JsonTileBranding Branding { get; set; }

        public JsonTileDisplayName DisplayName { get; set; }

        public string Arguments { get; set; }

        public JsonTileLockDetailedStatus LockDetailedStatus1 { get; set; }

        public JsonTileLockDetailedStatus LockDetailedStatus2 { get; set; }

        public JsonTileLockDetailedStatus LockDetailedStatus3 { get; set; }

        public Uri BaseUrl { get; set; }

        public bool? AddImageQuery { get; set; }

        public string Lang { get; set; }
    }
}
