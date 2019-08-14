using System;
using System.Collections.Generic;
using System.Text;

namespace AdaptiveBlocks.Transformers.WebNotification
{
    public class WebNotification
    {
        /// <summary>
        /// The title of the notification
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The body string of the notification
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The URL of an image to be displayed as part of the notification
        /// </summary>
        public string Image { get; set; }

        public List<WebNotificationAction> Actions { get; } = new List<WebNotificationAction>();
    }

    public class WebNotificationAction
    {
        /// <summary>
        /// The string describing the action that is displayed to the user.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The name of the action, which can be used to identify the clicked action similar to input names.
        /// </summary>
        public string Action { get; set; }
    }
}
