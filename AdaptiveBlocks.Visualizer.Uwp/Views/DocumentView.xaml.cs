using AdaptiveBlocks.Visualizer.Uwp.ViewModels.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AdaptiveBlocks.Visualizer.Uwp.Views
{
    public sealed partial class DocumentView : UserControl
    {
        public BaseDocumentViewModel ViewModel => DataContext as BaseDocumentViewModel;

        public DocumentView()
        {
            this.InitializeComponent();
        }

        private void ButtonRoundTrip_Click(object sender, RoutedEventArgs e)
        {
            (ViewModel as BlocksDocumentViewModel)?.RoundTripSerialize();
        }
    }
}
