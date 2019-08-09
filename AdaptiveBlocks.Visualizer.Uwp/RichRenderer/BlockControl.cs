using AdaptiveBlocks.Visualizer.Uwp.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace AdaptiveBlocks.Visualizer.Uwp.RichRenderer
{
    public class BlockControl : Panel
    {
        private BlockColumnControl m_columnControl = new BlockColumnControl();
        private BlockRowControl m_rowControl = new BlockRowControl();
        private FactSetItemControl m_factSetItemControl = new FactSetItemControl();

        public BlockControl()
        {
            DataContextChanged += BlockControl_DataContextChanged;

            Children.Add(m_columnControl);
            Children.Add(m_factSetItemControl);
            Children.Add(m_rowControl);

            SetBinding(OpacityProperty, new Binding()
            {
                Path = new Windows.UI.Xaml.PropertyPath("IsDropped"),
                Converter = new BoolToOpacityConverter(),
                ConverterParameter = "Invert"
            });
        }

        private void BlockControl_DataContextChanged(Windows.UI.Xaml.FrameworkElement sender, Windows.UI.Xaml.DataContextChangedEventArgs args)
        {
            m_columnControl.DataContext = DataContext;
            m_rowControl.DataContext = DataContext;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Data == null)
            {
                return new Size(0, 0);
            }

            m_columnControl.Measure(availableSize);
            m_factSetItemControl.Measure(availableSize);
            m_rowControl.Measure(availableSize);

            if (Data.IsColumn)
            {
                return m_columnControl.DesiredSize;
            }
            else if (Data.IsFactSet)
            {
                return m_factSetItemControl.DesiredSize;
            }
            else
            {
                return m_rowControl.DesiredSize;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Data == null)
            {
                return new Size(0, 0);
            }

            if (Data.IsColumn)
            {
                m_rowControl.Arrange(new Rect());
                m_factSetItemControl.Arrange(new Rect());
                m_columnControl.Arrange(new Rect(new Point(), finalSize));
            }
            else if (Data.IsFactSet)
            {
                m_rowControl.Arrange(new Rect());
                m_columnControl.Arrange(new Rect());
                m_factSetItemControl.Arrange(new Rect(new Point(), finalSize));
            }
            else
            {
                m_columnControl.Arrange(new Rect());
                m_factSetItemControl.Arrange(new Rect());
                m_rowControl.Arrange(new Rect(new Point(), finalSize));
            }

            return finalSize;
        }

        public RenderableBlockData Data
        {
            get { return DataContext as RenderableBlockData; }
        }
    }
}
