using AdaptiveBlocks.Visualizer.Uwp.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Controls;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AdaptiveBlocks.Visualizer.Uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel => DataContext as MainPageViewModel;

        public MainPage()
        {
            this.InitializeComponent();

            DataContext = new MainPageViewModel();
        }

        private void HamburgerMenu_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = e.ClickedItem as HamburgerMenuGlyphItem;
        }

        private void HamburgerMenu_Loaded(object sender, RoutedEventArgs e)
        {
            HamburgerMenu.SelectedItem = ViewModel.SelectedItem;
        }
    }
}
