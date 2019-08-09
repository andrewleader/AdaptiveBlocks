using AdaptiveBlocks.Inputs;
using AdaptiveBlocks.Transformers.AdaptiveCards.ElementTransformers;
using AdaptiveCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Transformers.AdaptiveCards
{
    public class AdaptiveBlockToCardTransformer : AdaptiveBlockTransformer<AdaptiveCard>
    {
        public AdaptiveBlockToCardTransformerProperties Properties { get; set; } = new AdaptiveBlockToCardTransformerProperties();

        public Dictionary<Type, IBaseElemementTransformer> ElementTransformers { get; set; } = new Dictionary<Type, IBaseElemementTransformer>()
        {
            { typeof(AdaptiveBlockContent), new BlockContentTransformer() },
            { typeof(AdaptiveInteractiveBlock), new InteractiveBlockTransformer() },
            { typeof(AdaptiveInputsGroup), new InputsGroupTransformer() },
            { typeof(AdaptiveChoiceSetInputBlock), new ChoiceSetInputTransformer() },
            { typeof(AdaptiveTextInputBlock), new TextInputTransformer() }
        };

        private AdaptiveCard TransformToCard(AdaptiveBlock block)
        {
            AdaptiveCard card = new AdaptiveCard("1.0");

            if (block != null)
            {
                bool first = true;
                foreach (var el in TransformBlockToElements(block, new AdaptiveBlockTransformContext(this)))
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        el.Separator = true;
                    }

                    card.Body.Add(el);
                }
            }

            return card;
        }

        internal List<AdaptiveElement> TransformBlockToElements(AdaptiveBlock block, AdaptiveBlockTransformContext context)
        {
            var content = block.View?.Content;
            if (content != null)
            {
                return ElementTransformers[typeof(AdaptiveBlockContent)].Transform(content, context);
            }

            return new List<AdaptiveElement>();
        }

        internal List<AdaptiveElement> TransformBlocksToElements(IEnumerable<AdaptiveBlock> blocks, AdaptiveBlockTransformContext context)
        {
            List<AdaptiveElement> elements = new List<AdaptiveElement>();

            int count = blocks.Count();
            for (int i = 0; i < count; i++)
            {
                var b = blocks.ElementAt(i);

                if (b.Hints.Column && context.Transformer.Properties.AllowsColumns)
                {
                    var columnGroup = blocks.Skip(i).TakeWhile(x => x.Hints.Column).ToList();
                    var innerContext = context.CloneForChildren(
                        isColumnContent: true);
                    elements.AddRange(TransformGroupAsColumnsToElements(columnGroup, innerContext));
                    i += columnGroup.Count - 1;
                }
                else if (b.Hints.FactSet && context.Transformer.Properties.AllowsFactSets)
                {
                    var factSetGroup = blocks.Skip(i).TakeWhile(x => x.Hints.FactSet).ToList();
                    var innerContext = context.CloneForChildren();
                    elements.AddRange(TransformGroupAsFactSetToElements(factSetGroup, innerContext));
                    i += factSetGroup.Count - 1;
                }
                else
                {
                    elements.AddRange(TransformBlockToElements(b, context.CloneForChildren()));
                }
            }

            return elements;
        }

        internal List<AdaptiveElement> TransformInputsToElements(IEnumerable<BaseAdaptiveBlockElement> inputs, AdaptiveBlockTransformContext context)
        {
            List<AdaptiveElement> elements = new List<AdaptiveElement>();

            foreach (var input in inputs)
            {
                elements.AddRange(TransformBlockElementToElements(input, context.CloneForChildren()));
            }

            return elements;
        }

        private List<AdaptiveElement> TransformGroupAsColumnsToElements(List<AdaptiveBlock> group, AdaptiveBlockTransformContext context)
        {
            AdaptiveColumnSet columnSet = new AdaptiveColumnSet();

            foreach (var block in group)
            {
                AdaptiveColumn column = new AdaptiveColumn();

                var innerContext = context.CloneForChildren(
                    isColumnContent: true);

                foreach (var el in context.Transformer.TransformBlockToElements(block, innerContext))
                {
                    column.Items.Add(el);
                }

                columnSet.Columns.Add(column);
            }

            return new List<AdaptiveElement>() { columnSet };
        }

        private List<AdaptiveElement> TransformGroupAsFactSetToElements(List<AdaptiveBlock> group, AdaptiveBlockTransformContext context)
        {
            AdaptiveFactSet factSet = new AdaptiveFactSet();

            foreach (var block in group)
            {
                AdaptiveFact fact = new AdaptiveFact()
                {
                    Title = block.View?.Content?.Title,
                    Value = block.View?.Content?.Subtitle
                };

                factSet.Facts.Add(fact);
            }

            return new List<AdaptiveElement>() { factSet };
        }

        public List<AdaptiveElement> TransformBlockElementToElements(BaseAdaptiveBlockElement element, AdaptiveBlockTransformContext context)
        {
            if (element != null && ElementTransformers.TryGetValue(element.GetType(), out IBaseElemementTransformer elementRenderer))
            {
                return elementRenderer.Transform(element, context);
            }
            else
            {
                return new List<AdaptiveElement>()
                {
                    new AdaptiveTextBlock()
                    {
                        Text = "Unknown element"
                    }
                };
            }
        }

        protected override Task TransformAsync(AdaptiveBlock block, AdaptiveBlockTransformResult<AdaptiveCard> result)
        {
            try
            {
                result.Result = TransformToCard(block);
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.ToString());
            }
            return Task.CompletedTask;
        }
    }

    public class AdaptiveBlockTransformContext
    {
        public AdaptiveBlockTransformContext(AdaptiveBlockToCardTransformer transformer)
        {
            Transformer = transformer;
        }

        public AdaptiveBlockToCardTransformer Transformer { get; private set; }

        public bool IsColumnContent { get; private set; }

        public AdaptiveBlockTransformContext CloneForChildren(bool isColumnContent = false)
        {
            var answer = new AdaptiveBlockTransformContext(Transformer)
            {
                // Skip IsColumContent since that gets reset
                //IsColumnContent = IsColumnContent
                IsColumnContent = isColumnContent
            };

            return answer;
        }
    }

    public class AdaptiveBlockToCardTransformerProperties
    {
        public bool AllowsDetailedBlocks { get; set; } = true;

        public bool AllowsColumns { get; set; } = true;

        public bool AllowsFactSets { get; set; } = true;

        public bool AllowsInteractivity { get; set; } = true;

        //public Dictionary
    }

    public class AdaptiveBlockRendererDefaultTemplate : AdaptiveBlockRendererTemplate
    {
        private Dictionary<AdaptiveBlockCategoryHints, AdaptiveBlockRendererTemplate> m_templates;

        public AdaptiveBlockRendererTemplate Featured
        {
            get { return m_templates.FirstOrDefault(i => i.Key == AdaptiveBlockCategoryHints.Featured).Value; }
            set { m_templates[AdaptiveBlockCategoryHints.Featured] = value; }
        }

        //public AdaptiveBlockRendererTemplate GetTemplate(IEnumerable<AdaptiveBlockCategoryHints> category)
        //{

        //}
    }

    public class AdaptiveBlockRendererTemplate
    {
        
    }
}
