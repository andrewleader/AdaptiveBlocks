using AdaptiveBlocks.Visualizer.Uwp.ViewModels.Documents;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Visualizer.Uwp.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        public MainPageViewModel()
        {
            Initialize();
        }

        private HamburgerMenuGlyphItem _menuItemBlocks = new HamburgerMenuGlyphItem()
        {
            Label = "Blocks payload",
            Glyph = "Message"
        };

        public List<HamburgerMenuGlyphItem> MenuItems => new List<HamburgerMenuGlyphItem>()
        {
            _menuItemBlocks
        };

        private HamburgerMenuGlyphItem _selectedItem;
        public HamburgerMenuGlyphItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value);

                if (value == _menuItemBlocks)
                {
                    ContentViewModel = BlocksDocuments;
                    //Settings.StoredSelectedMenuItem = StoredSelectedMenuItem.Toast;
                }
            }
        }

        private object _contentViewModel;
        public object ContentViewModel
        {
            get { return _contentViewModel; }
            private set { SetProperty(ref _contentViewModel, value); }
        }

        public PropertiesViewModel Properties { get; private set; }

        public BlocksDocumentsViewModel BlocksDocuments { get; private set; }

        private void Initialize()
        {
            try
            {
                Properties = new PropertiesViewModel();
                BlocksDocuments = new BlocksDocumentsViewModel(Properties);

                //switch (Settings.StoredSelectedMenuItem)
                //{
                //    case StoredSelectedMenuItem.Tile:
                //        SelectedItem = _menuItemTiles;
                //        break;

                //    case StoredSelectedMenuItem.Toast:
                //        SelectedItem = _menuItemBlocks;
                //        break;
                //}

                SelectedItem = _menuItemBlocks;
            }
            catch { }
        }
    }
}
