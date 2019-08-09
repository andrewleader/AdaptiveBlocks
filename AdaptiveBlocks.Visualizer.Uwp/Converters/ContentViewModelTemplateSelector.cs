using AdaptiveBlocks.Visualizer.Uwp.ViewModels.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AdaptiveBlocks.Visualizer.Uwp.Converters
{
    public class ContentViewModelTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TemplateEditor { get; set; }
        public DataTemplate TemplateProperties { get; set; }
        public DataTemplate TemplateAbout { get; set; }

        public ContentViewModelTemplateSelector()
        {
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is BaseDocumentsViewModel)
            {
                return TemplateEditor;
            }
            //if (item is PropertiesViewModel)
            //{
            //    return TemplateProperties;
            //}
            //if (item is AboutViewModel)
            //{
            //    return TemplateAbout;
            //}

            return null;
        }
    }
}
