using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdaptiveBlocks.Actions;
using AdaptiveBlocks.Attributes;
using AdaptiveBlocks.Inputs;
using Microsoft.Toolkit.Uwp.Notifications;

namespace AdaptiveBlocks.Transformers.ToastContentTransformer
{
    public class AdaptiveBlockToToastContentTransformer : AdaptiveBlockTransformer<ToastContent>
    {
        protected override Task TransformAsync(AdaptiveBlock block, AdaptiveBlockTransformResult<ToastContent> result)
        {
            ToastContent content = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                }
            };

            var finalBlocks = block.GetFinalBlocks();
            var firstBlock = finalBlocks.First();


            var firstBlockContent = firstBlock.View?.Content;
            if (firstBlockContent != null)
            {
                foreach (var t in firstBlockContent.Text.Take(3))
                {
                    content.Visual.BindingGeneric.Children.Add(new AdaptiveText()
                    {
                        Text = t.Text
                    });
                }

                var profileOrIconImageRequest = new AdaptiveBlockContentConsumer.ImageMatchRequest()
                {
                    RequiredHint =
                    {
                        AdaptiveBlockImageCategoryHints.Profile,
                        AdaptiveBlockImageCategoryHints.Icon
                    }
                };

                var heroImageRequest = new AdaptiveBlockContentConsumer.ImageMatchRequest()
                {
                    HintsToAvoid =
                    {
                        AdaptiveBlockImageCategoryHints.Icon
                    }
                };

                AdaptiveBlockContentConsumer.MatchImages(firstBlockContent, profileOrIconImageRequest, heroImageRequest);

                if (heroImageRequest.ImageResult != null)
                {
                    content.Visual.BindingGeneric.HeroImage = new ToastGenericHeroImage()
                    {
                        Source = heroImageRequest.ImageResult.Url
                    };
                }

                if (profileOrIconImageRequest.ImageResult != null)
                {
                    content.Visual.BindingGeneric.AppLogoOverride = new ToastGenericAppLogo()
                    {
                        Source = profileOrIconImageRequest.ImageResult.Url,
                        HintCrop = profileOrIconImageRequest.ImageResult.Hints.Category.Contains(AdaptiveBlockImageCategoryHints.Profile) ? ToastGenericAppLogoCrop.Circle : ToastGenericAppLogoCrop.Default
                    };
                }

                if (finalBlocks.Count() > 1)
                {
                    foreach (var extraBlock in finalBlocks.Skip(1))
                    {
                        if (extraBlock.View?.Content != null)
                        {
                            content.Visual.BindingGeneric.Children.Add(CreateGroup(extraBlock.View.Content));
                        }
                    }
                }

                if (block.View.Attributes?.AttributionText?.Text != null)
                {
                    content.Visual.BindingGeneric.Attribution = new ToastGenericAttributionText()
                    {
                        Text = block.View.Attributes?.AttributionText?.Text
                    };
                }

                if (firstBlockContent.Actions.Any())
                {
                    content.Actions = new ToastActionsCustom();
                    var toastActions = content.Actions as ToastActionsCustom;

                    foreach (var a in firstBlockContent.Actions)
                    {
                        // Quick reply style
                        if (a is AdaptiveAction singleFinalAction && a.Inputs.Count == 1 && a.Inputs.First() is AdaptiveTextInputBlock singleTextInput)
                        {
                            toastActions.Inputs.Add(new ToastTextBox(singleTextInput.Id)
                            {
                                PlaceholderContent = singleTextInput.Placeholder
                            });
                            toastActions.Buttons.Add(new ToastButton(singleFinalAction.Title, "args")
                            {
                                TextBoxId = singleTextInput.Id,
                                ImageUri = singleFinalAction.Icon?.GetIconForTheme(AdaptiveThemes.Dark)?.Url
                            });
                        }
                        else
                        {
                            foreach (var input in a.Inputs)
                            {
                                if (input is AdaptiveTextInputBlock textInput)
                                {
                                    toastActions.Inputs.Add(new ToastTextBox(textInput.Id)
                                    {
                                        PlaceholderContent = textInput.Placeholder,
                                        Title = textInput.Title
                                    });
                                }

                                else if (input is AdaptiveChoiceSetInputBlock choiceInput)
                                {
                                    var selectionBox = new ToastSelectionBox(choiceInput.Id)
                                    {
                                        Title = choiceInput.Title
                                    };
                                    foreach (var val in choiceInput.Choices)
                                    {
                                        selectionBox.Items.Add(new ToastSelectionBoxItem(val.Value, val.Title));
                                    }
                                    toastActions.Inputs.Add(selectionBox);
                                }
                            }

                            List<AdaptiveAction> finalActions;
                            if (a is AdaptiveSharedInputActions sharedActions)
                            {
                                finalActions = sharedActions.Actions;
                            }
                            else if (a is AdaptiveAction finalAction)
                            {
                                finalActions = new List<AdaptiveAction>() { finalAction };
                            }
                            else
                            {
                                finalActions = new List<AdaptiveAction>();
                            }

                            foreach (var finalAction in finalActions)
                            {
                                toastActions.Buttons.Add(new ToastButton(finalAction.Title, "args"));
                            }
                        }
                    }
                }
            }

            result.Result = content;
            return Task.CompletedTask;
        }

        private AdaptiveGroup CreateGroup(AdaptiveBlockContent block)
        {
            var group = new AdaptiveGroup();

            var subgroupText = new AdaptiveSubgroup()
            {
                HintTextStacking = AdaptiveSubgroupTextStacking.Center
            };
            group.Children.Add(subgroupText);

            var img = block.Images.FirstOrDefault();
            if (img != null)
            {
                var subgroupImage = new AdaptiveSubgroup()
                {
                    HintWeight = 8
                };
                group.Children.Add(subgroupImage);

                subgroupImage.Children.Add(new AdaptiveImage()
                {
                    Source = img.Url
                });
            }

            var firstText = block.Text.FirstOrDefault();
            if (firstText != null)
            {
                subgroupText.Children.Add(new AdaptiveText()
                {
                    Text = firstText.Text,
                    HintStyle = AdaptiveTextStyle.Body,
                    HintWrap = true
                });

                if (block.Text.Count > 1)
                {
                    foreach (var remainingText in block.Text.Skip(1))
                    {
                        subgroupText.Children.Add(new AdaptiveText()
                        {
                            Text = remainingText.Text,
                            HintStyle = AdaptiveTextStyle.CaptionSubtle,
                            HintWrap = true
                        });
                    }
                }
            }

            return group;
        }
    }
}
