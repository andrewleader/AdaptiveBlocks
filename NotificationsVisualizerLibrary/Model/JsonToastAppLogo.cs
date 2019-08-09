using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal class JsonToastAppLogo
    {
        public string Url { get; set; }

        public JsonToastAppLogoImageStyle Style { get; set; }

        public string AltText { get; set; }

        //public bool AddImageQuery { get; set; }
    }

    internal enum JsonToastAppLogoImageStyle
    {
        Default,
        Person
    }
}
