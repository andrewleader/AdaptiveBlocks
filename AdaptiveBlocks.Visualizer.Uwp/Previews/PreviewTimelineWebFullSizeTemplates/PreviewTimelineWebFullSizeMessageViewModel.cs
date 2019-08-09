
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AdaptiveBlocks.Visualizer.Uwp.Previews.PreviewTimelineWebFullSizeTemplates
{
    public class PreviewTimelineWebFullSizeMessageViewModel
    {
        public string ImageAttachment { get; set; }

        public ElementTheme RequestedTheme => ImageAttachment != null ? ElementTheme.Dark : ElementTheme.Light;

        public string From { get; set; }

        public string Message { get; set; }

        public string ProfileImage { get; set; }

        public static PreviewTimelineWebFullSizeMessageViewModel Create(AdaptiveBlockContent blockContent)
        {
            return new PreviewTimelineWebFullSizeMessageViewModel()
            {
                From = blockContent.Title,
                Message = blockContent.Subtitle,
                ProfileImage = blockContent.GetProfileImage()?.Url,
                ImageAttachment = blockContent.GetBackgroundImage()?.Url // TODO: This has risk of same image being used for both
            };
        }
    }
}
