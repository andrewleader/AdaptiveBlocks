using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal class JsonTileBackgroundImage
    {
        public string Url { get; set; }

        public string AltText { get; set; }

        public int? Overlay { get; set; }

        public JsonTileBackgroundImageStyle Style { get; set; }

        public bool? AddImageQuery { get; set; }
    }
}
