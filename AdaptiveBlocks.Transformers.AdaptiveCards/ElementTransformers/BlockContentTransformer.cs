using AdaptiveCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Transformers.AdaptiveCards.ElementTransformers
{
    public class BlockContentTransformer : BaseElementTransformer<AdaptiveBlockContent>
    {
        public override List<AdaptiveElement> Transform(AdaptiveBlockContent block, AdaptiveBlockTransformContext context)
        {
            if (context.Transformer.Properties.AllowsDetailedBlocks && block.DetailedBlocks.Any())
            {
                return context.Transformer.TransformBlocksToElements(block.DetailedBlocks, context);
            }

            if (context.IsColumnContent)
            {
                List<AdaptiveElement> columnAnswer = new List<AdaptiveElement>();
                if (block.Images.Any())
                {
                    columnAnswer.Add(new AdaptiveImage()
                    {
                        Url = new Uri(block.Images.First().Url),
                        PixelWidth = 40,
                        PixelHeight = 40,
                        HorizontalAlignment = AdaptiveHorizontalAlignment.Center
                    });
                }
                if (block.Title != null)
                {
                    columnAnswer.Add(new AdaptiveTextBlock()
                    {
                        Text = block.Title,
                        Weight = AdaptiveTextWeight.Bolder,
                        Spacing = AdaptiveSpacing.None,
                        HorizontalAlignment = AdaptiveHorizontalAlignment.Center
                    });

                    if (block.Subtitle != null)
                    {
                        columnAnswer.Add(new AdaptiveTextBlock()
                        {
                            Text = block.Subtitle,
                            Spacing = AdaptiveSpacing.None,
                            HorizontalAlignment = AdaptiveHorizontalAlignment.Center
                        });
                    }
                }

                columnAnswer.AddRange(CreateActions(block, context));

                return columnAnswer;
            }

            AdaptiveColumnSet columnSet = new AdaptiveColumnSet();

            List<AdaptiveBlockImage> remainingImages = block.Images.ToList();
            var profileImage = remainingImages.FirstOrDefault(i => i.Hints.Category.Contains(AdaptiveBlockImageCategoryHints.Profile));
            if (profileImage != null)
            {
                columnSet.Columns.Add(new AdaptiveColumn()
                {
                    Width = "auto",
                    Items =
                    {
                        new AdaptiveImage()
                        {
                            Url = new Uri(profileImage.Url),
                            PixelWidth = 60,
                            PixelHeight = 60,
                            Style = AdaptiveImageStyle.Person
                        }
                    }
                });

                remainingImages.Remove(profileImage);
            }

            AdaptiveColumn textColumn = new AdaptiveColumn()
            {
                Width = "stretch",
                VerticalContentAlignment = profileImage != null ? AdaptiveVerticalContentAlignment.Center : AdaptiveVerticalContentAlignment.Top
            };
            {
                if (block.Title != null)
                {
                    textColumn.Items.Add(new AdaptiveTextBlock()
                    {
                        Text = block.Title
                    });

                    if (block.Subtitle != null)
                    {
                        textColumn.Items.Add(new AdaptiveTextBlock()
                        {
                            Text = block.Subtitle,
                            Spacing = AdaptiveSpacing.None,
                            IsSubtle = true
                        });
                    }
                }
            }

            if (remainingImages.Any())
            {
                AdaptiveColumn imageColumn = new AdaptiveColumn()
                {
                    Width = "auto",
                    Items = {
                        new AdaptiveImage()
                        {
                            Url = new Uri(remainingImages.First().Url),
                            PixelWidth = 60,
                            PixelHeight = 60
                        }
                    }
                };

                if (textColumn.Items.Any())
                {
                    columnSet.Columns.Add(textColumn);
                }
                columnSet.Columns.Add(imageColumn);
            }
            else
            {
                if (textColumn.Items.Any())
                {
                    columnSet.Columns.Add(textColumn);
                }
            }

            var answer = new List<AdaptiveElement>();

            if (columnSet.Columns.Count > 0)
            {
                answer.Add(columnSet);
            }

            answer.AddRange(CreateActions(block, context));

            return answer;
        }

        private List<AdaptiveElement> CreateActions(AdaptiveBlockContent block, AdaptiveBlockTransformContext context)
        {
            if (!block.Actions.Any() || !context.Transformer.Properties.AllowsInteractivity)
            {
                return new List<AdaptiveElement>();
            }

            var answer = new List<AdaptiveElement>();

            var inputGroups = block.GetActionInputGroups().ToArray();

            bool alreadyRenderedInputs = false;

            // If there's just one input group, render it inline
            if (inputGroups.Length == 1)
            {
                var inputs = inputGroups[0];

                var simplifedActions = block.GetSimplifiedActions();
                if (inputs.Count == 1 && simplifedActions.Count() == 1 && inputs.First() is AdaptiveBlocks.Inputs.AdaptiveTextInputBlock textInputBlock)
                {
                    // Inline the action
                    List<AdaptiveElement> cardTextInputElements = TextInputTransformer.Transform(textInputBlock);
                    var cardTextInput = cardTextInputElements.OfType<AdaptiveTextInput>().FirstOrDefault();
                    if (cardTextInput != null)
                    {
                        var blockAction = simplifedActions.First();
                        cardTextInput.AdditionalProperties["inlineAction"] = new Dictionary<string, string>()
                        {
                            { "type", "Action.Submit" },
                            { "title", blockAction.Title },
                            { "iconUrl", blockAction.Icon?.GetIconForTheme(Attributes.AdaptiveThemes.Light)?.Url }
                        };
                        answer.AddRange(cardTextInputElements);
                        return answer;
                    }
                }

                answer.AddRange(context.Transformer.TransformInputsToElements(inputs, context));

                alreadyRenderedInputs = true;
            }

            var actionSet = new AdaptiveActionSet();

            if (alreadyRenderedInputs || inputGroups.Length == 0)
            {
                foreach (var a in block.GetSimplifiedActions())
                {
                    actionSet.Actions.Add(new AdaptiveSubmitAction()
                    {
                        Title = a.Title
                    });
                }
            }
            else
            {
                foreach (var a in block.GetSimplifiedActions())
                {
                    if (a.Inputs.Any())
                    {
                        var card = new AdaptiveCard();
                        foreach (var el in context.Transformer.TransformInputsToElements(a.Inputs, context))
                        {
                            card.Body.Add(el);
                        }

                        card.Actions.Add(new AdaptiveSubmitAction()
                        {
                            Title = a.Title
                        });

                        // Note that constructing a ShowCard fails in UWP for some reason...
                        actionSet.Actions.Add(new AdaptiveShowCardAction()
                        {
                            Title = a.Title,
                            Card = card
                        });
                    }
                    else
                    {
                        actionSet.Actions.Add(new AdaptiveSubmitAction()
                        {
                            Title = a.Title
                        });
                    }
                }

            }

            

            answer.Add(actionSet);

            return answer;
        }
    }
}
