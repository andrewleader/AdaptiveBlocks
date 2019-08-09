using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace AdaptiveBlocks.Visualizer.Uwp.ViewModels.Documents
{
    public class AddDocumentListItem
    {
        public static readonly AddDocumentListItem NewDocument = new AddDocumentListItem() { DisplayName = "\u2795 New document" };
        public static readonly AddDocumentListItem OpenDocument = new AddDocumentListItem() { DisplayName = "\uD83D\uDCC2 Open document" };

        public string DisplayName { get; set; }

        public StorageFile File { get; set; }
    }
}
