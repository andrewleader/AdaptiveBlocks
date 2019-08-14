using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Transformers.WebNotification
{
    public class AdaptiveBlockToWebNotificationTransformer : AdaptiveBlockTransformer<WebNotification>
    {
        protected override async Task TransformAsync(AdaptiveBlock block, AdaptiveBlockTransformResult<WebNotification> result)
        {
            var content = block?.View?.Content;
            if (content?.Title != null)
            {
                WebNotification notification = new WebNotification()
                {
                    Title = content.Title,
                    Body = content.Subtitle,
                    Image = content.Images?.FirstOrDefault()?.Url
                };

                foreach (var action in content.GetSimplifiedActions())
                {
                    if (action.Inputs.Count == 0 && action.Command != null)
                    {
                        notification.Actions.Add(new WebNotificationAction()
                        {
                            Title = action.Title,
                            Action = JsonConvert.SerializeObject(action.Command)
                        });
                    }
                }

                result.Result = notification;
            }
            else
            {
                result.Errors.Add("There wasn't a title");
            }
        }
    }
}
