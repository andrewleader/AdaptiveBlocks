using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal class JsonToastHeader
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public ExpandoObject SelectAction { get; set; }
    }
}
