using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace AdaptiveBlocks.Visualizer.Uwp.Settings
{
    public static class OverallSettings
    {
        public static BlocksPayloadEditorSettings BlocksPayloadEditor { get; private set; } = new BlocksPayloadEditorSettings();

        internal static IPropertySet _values = ApplicationData.Current.LocalSettings.CreateContainer("Settings", ApplicationDataCreateDisposition.Always).Values;
    }
}
