using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdaptiveBlocks.Transformers
{
    public class AdaptiveBlockContentConsumer
    {
        public AdaptiveBlockContent Content { get; private set; }

        public AdaptiveBlockConsumerProperties Properties { get; private set; }

        public AdaptiveBlockContentConsumer(AdaptiveBlockContent content, AdaptiveBlockConsumerProperties properties)
        {
            Content = content;
            Properties = properties;
        }

        public bool ConsumeText(out AdaptiveBlockText text)
        {
            text = Content.Text?.FirstOrDefault();
            if (text != null)
            {
                Content.Text.RemoveAt(0);
                return true;
            }
            return false;
        }

        public bool ConsumeImage(Func<AdaptiveBlockImage, bool> predicate, out AdaptiveBlockImage image)
        {
            if (Content.Images != null)
            {
                image = Content.Images.FirstOrDefault(predicate);
                if (image != null)
                {
                    Content.Images.Remove(image);
                    return true;
                }
            }
            image = null;
            return false;
        }

        public bool ConsumeImage(AdaptiveBlockImageCategoryHints category, out AdaptiveBlockImage image)
        {
            return ConsumeImage(i => i.Hints.Category.Contains(category), out image);
        }

        public void ConsumeImages(params ImageMatchRequest[] requests)
        {
            MatchImages(Content, requests);

            foreach (var r in requests)
            {
                if (r.ImageResult != null)
                {
                    Content.Images.Remove(r.ImageResult);
                }
            }
        }

        public static void MatchImages(AdaptiveBlockContent content, params ImageMatchRequest[] requests)
        {
            if (content.Images == null)
            {
                return;
            }

            foreach (var img in content.Images)
            {
                PlaceImageMatch(requests, img, new ImageMatchRequest[0]);

                if (requests.All(i => i.ImageResult != null))
                {
                    break;
                }
            }
        }

        private static bool PlaceImageMatch(ImageMatchRequest[] requests, AdaptiveBlockImage img, IEnumerable<ImageMatchRequest> alreadySwapped)
        {
            List<ImageMatchRequest> swappables = new List<ImageMatchRequest>();
            for (int i = 0; i < requests.Length; i++)
            {
                var req = requests[i];
                if (req.Matches(img))
                {
                    if (req.ImageResult == null)
                    {
                        req.ImageResult = img;
                        return true;
                    }
                    else if (!alreadySwapped.Contains(req))
                    {
                        swappables.Add(req);
                    }
                }
            }

            foreach (var swappable in swappables)
            {
                var swappingImg = swappable.ImageResult;

                swappable.ImageResult = img;

                if (PlaceImageMatch(requests, swappingImg, alreadySwapped.Concat(new ImageMatchRequest[] { swappable })))
                {
                    return true;
                }

                swappable.ImageResult = swappingImg;
            }

            return false;
        }

        private bool ConsumeImage(AdaptiveBlockImageCategoryHints preferredCategory, List<AdaptiveBlockImageCategoryHints> categoriesToAvoid, out AdaptiveBlockImage image)
        {
            return ConsumeImage(i => i.Hints.Category.Contains(preferredCategory) && i.Hints.Category.All(c => !categoriesToAvoid.Contains(c)), out image);
        }



        public class ImageMatchRequest
        {
            /// <summary>
            /// Joined together with OR.
            /// </summary>
            public List<AdaptiveBlockImageCategoryHints> RequiredHint { get; private set; } = new List<AdaptiveBlockImageCategoryHints>();

            public List<AdaptiveBlockImageCategoryHints> HintsToAvoid { get; private set; } = new List<AdaptiveBlockImageCategoryHints>();

            public AdaptiveBlockImage ImageResult { get; set; }

            public static ImageMatchRequest ForBackgroundImage()
            {
                return new ImageMatchRequest()
                {
                    HintsToAvoid =
                    {
                        AdaptiveBlockImageCategoryHints.Document,
                        AdaptiveBlockImageCategoryHints.Icon
                    }
                };
            }

            internal bool Matches(AdaptiveBlockImage image)
            {
                if (RequiredHint.Count > 0)
                {
                    if (!RequiredHint.Any(required => image.Hints.Category.Contains(required)))
                    {
                        return false;
                    }
                }

                if (HintsToAvoid.Any(avoid => image.Hints.Category.Contains(avoid)))
                {
                    return false;
                }

                return true;
            }
        }
    }
}
