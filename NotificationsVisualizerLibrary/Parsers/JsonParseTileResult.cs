using NotificationsVisualizerLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Parsers
{
    public sealed class JsonParseTileResult
    {
        internal JsonTileContent Content { get; set; }

        /// <summary>
        /// Errors found when parsing the payload. If there were only warnings, like specifying two of the same bindings, the tile is still updated.
        /// </summary>
        public IList<ParseError> Errors { get; private set; } = new ObservableCollection<ParseError>();

        /// <summary>
        /// Returns true if there's no fatal error, meaning that the tile rendering can proceed (but there still may be warnings or critical system errors that Windows won't allow).
        /// </summary>
        /// <returns></returns>
        public bool IsOkForRender()
        {
            return !Errors.Any(i => i.Type == ParseErrorType.Error);
        }
    }
}
