using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal class JsonToastAudio
    {
        public Uri Url { get; set; }

        public bool Loop { get; set; }

        public bool Silent { get; set; }
    }
}
