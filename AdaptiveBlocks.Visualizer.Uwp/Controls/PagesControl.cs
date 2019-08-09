using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace AdaptiveBlocks.Visualizer.Uwp.Controls
{
    public class PagesControl : Panel
    {
        private TranslateTransform m_translateTransform;

        public PagesControl()
        {
            m_translateTransform = new TranslateTransform();
            RenderTransform = m_translateTransform;

            Background = new SolidColorBrush(Colors.Transparent);
            ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.TranslateX;
            ManipulationDelta += PagesControl_ManipulationDelta;
            ManipulationCompleted += PagesControl_ManipulationCompleted;
        }

        public int Page
        {
            get
            {
                return (int)(((m_translateTransform.X * -1) + ActualHeight * 0.5) / ActualHeight);
            }
            set
            {
                SetTransform(value * ActualHeight * -1);
            }
        }

        private void PagesControl_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            Page = Page;
        }

        private void PagesControl_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            SetTransform(m_translateTransform.X + e.Delta.Translation.X);
        }

        private void SetTransform(double x)
        {
            if (x > 0)
            {
                x = 0;
            }
            else if (x * -1 > (ActualWidth - ActualHeight))
            {
                x = (ActualWidth - ActualHeight) * -1;
            }

            m_translateTransform.X = x;
            UpdateClip();
        }

        private void UpdateClip()
        {
            Clip = new RectangleGeometry()
            {
                Rect = new Rect(m_translateTransform.X * -1, 0, ActualHeight, ActualHeight)
            };
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (double.IsInfinity(availableSize.Height))
            {
                throw new NotImplementedException("Only horizontal is supported");
            }

            Size pageSize = new Size(availableSize.Height, availableSize.Height);

            foreach (var c in Children)
            {
                c.Measure(pageSize);
            }

            return new Size(
                width: availableSize.Height * Math.Max(Children.Count, 1),
                height: availableSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (double.IsInfinity(finalSize.Height))
            {
                throw new NotImplementedException("Only horizontal is supported");
            }

            Size pageSize = new Size(finalSize.Height, finalSize.Height);

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Arrange(new Rect(
                    location: new Point(
                        x: pageSize.Width * i,
                        y: 0),
                    size: pageSize));
            }

            Clip = new RectangleGeometry()
            {
                Rect = new Rect(m_translateTransform.X * -1, 0, finalSize.Height, finalSize.Height)
            };

            return new Size(
                width: finalSize.Height * Math.Max(Children.Count, 1),
                height: finalSize.Height);
        }
    }
}
