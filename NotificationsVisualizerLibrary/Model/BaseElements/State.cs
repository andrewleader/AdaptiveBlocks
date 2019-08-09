using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Parsers;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    internal class State : AdaptiveParentElement
    {
        public const string ELEMENT_NAME = "state";

        internal const string ATTR_ID = "id";

        public State(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }
        
        public string Id { get; set; }

        internal IList<AdaptiveSetter> Setters { get; private set; } = new List<AdaptiveSetter>();

        internal void Parse(ParseResult result, XElement node)
        {
            AttributesHelper attributes = new AttributesHelper(node.Attributes());

            ParseKnownAttributes(node, attributes, result);

            HandleRemainingAttributes(attributes, result);

            // 0-n children
            foreach (XElement child in node.Elements())
            {
                if (!result.IsOkForRender())
                    break;

                try
                {
                    HandleChild(result, child);
                }

                catch (IncompleteElementException)
                {

                }
            }
        }

        internal void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result)
        {
            // id is required
            XAttribute attrId = attributes.PopAttribute(ATTR_ID);
            if (attrId == null)
            {
                result.AddError("id attribute on input element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }

            this.Id = attrId.Value;
        }

        protected virtual void HandleChild(ParseResult result, XElement child)
        {
            switch (child.Name.LocalName.ToLower())
            {
                case AdaptiveSetter.ELEMENT_NAME:

                    var setter = new AdaptiveSetter(Context, SupportedFeatures);
                    setter.Parse(result, child);

                    if (!result.IsOkForRender())
                        return;

                    Setters.Add(setter);
                    setter.Parent = this;

                    break;


                default:
                    result.AddWarning($"Invalid child {child.Name.LocalName} under {ELEMENT_NAME} element.", GetErrorPositionInfo(child));
                    break;
            }
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[] { };
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            foreach (var i in Setters)
                yield return i;
        }
    }
}
