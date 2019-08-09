using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal class JsonTilePeekContent
    {
        public ExpandoObject[] Content { get; set; }

        public JsonTileBackgroundImage BackgroundImage { get; set; }
    }
}
