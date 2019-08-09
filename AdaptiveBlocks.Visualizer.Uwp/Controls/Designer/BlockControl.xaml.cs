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

namespace AdaptiveBlocks.Visualizer.Uwp.Controls.Designer
{
    public sealed partial class BlockControl : UserControl
    {
        public BlockControl()
        {
            this.InitializeComponent();
        }

        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            BlocksDocumentViewModel?.RemoveBlock(AdaptiveBlock);
        }

        public AdaptiveBlock AdaptiveBlock
        {
            get { return (AdaptiveBlock)GetValue(AdaptiveBlockProperty); }
            set { SetValue(AdaptiveBlockProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AdaptiveBlock.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AdaptiveBlockProperty =
            DependencyProperty.Register("AdaptiveBlock", typeof(AdaptiveBlock), typeof(BlockControl), new PropertyMetadata(null));

        public BlocksDocumentViewModel BlocksDocumentViewModel
        {
            get { return (BlocksDocumentViewModel)GetValue(BlocksDocumentViewModelProperty); }
            set { SetValue(BlocksDocumentViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BlocksDocumentViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BlocksDocumentViewModelProperty =
            DependencyProperty.Register("BlocksDocumentViewModel", typeof(BlocksDocumentViewModel), typeof(BlockControl), new PropertyMetadata(null));
    }
}
