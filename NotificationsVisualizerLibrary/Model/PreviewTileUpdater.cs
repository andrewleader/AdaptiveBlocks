﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsVisualizerLibrary.Parsers;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace NotificationsVisualizerLibrary
{
    /// <summary>
    /// Changes the content of the specific tile that the updater is bound to.
    /// </summary>
    public sealed class PreviewTileUpdater
    {
        private static XmlTemplateParser _parser = new XmlTemplateParser();
        private PreviewTile _previewTile;

        internal PreviewTileUpdater(PreviewTile previewTile)
        {
            _previewTile = previewTile;
        }

        /// <summary>
        /// Updates the tile with the notification. If the tile payload has an error, the tile will not be updated. The first error, or multiple warnings, will be reported via the returned ParseResult.
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public ParseResult Update(TileNotification notification)
        {
            ParseResult result = _parser.Parse(notification.Content.GetXml(), _previewTile.CurrFeatureSet);

            if (result.IsOkForRender())
            {
                _previewTile.Show(result.Tile, true);
            }

            return result;
        }

        /// <summary>
        /// Updates the tile with the notification. If the tile payload has an error, the tile will not be updated. The first error, or multiple warnings, will be reported via the returned ParseResult.
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public JsonParseTileResult UpdateFromJson(string content)
        {
            JsonParseTileResult result = JsonParser.ParseTile(content, _previewTile.CurrFeatureSet);

            if (result.IsOkForRender())
            {
                _previewTile.Show(result.Content, true, result);
            }

            return result;
        }

#if EXPERIMENTAL
        /// <summary>
        /// Updates the tile with the notification. If the tile payload has an error, the tile will not be updated. The first error, or multiple warnings, will be reported via the returned ParseResult.
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public ParseResult UpdateFromOneNotification(XmlDocument content)
        {
            ParseResult result = _parser.ParseNotification(content.GetXml());

            if (result.IsOkForRender())
            {
                _previewTile.Show(result.Notification, true);
            }

            return result;
        }
#endif

        /// <summary>
        /// Removes all updates and causes the tile to display its default contentas declared in the tile properties.
        /// </summary>
        public void Clear()
        {
            _previewTile.Show(null, true);
        }
    }
}
