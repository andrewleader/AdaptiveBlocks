using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Parsers;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    internal class ActionDefinitions : AdaptiveParentElement
    {
        internal ActionDefinitions(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }


        internal IList<ActionDefinition> Definitions { get; private set; } = new List<ActionDefinition>();


        internal void Parse(ParseResult result, XElement node)
        {
            if (!XmlTemplateParser.EnsureNodeOnlyHasElementsAsChildren(result, node))
                throw new IncompleteElementException();

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
            // Nothing for now
        }

        protected virtual void HandleChild(ParseResult result, XElement child)
        {
            switch (child.Name.LocalName.ToLower())
            {
                case "action":

                    var action = new ActionDefinition(Context, SupportedFeatures);
                    action.Parse(result, child);

                    if (!result.IsOkForRender())
                        return;

                    Definitions.Add(action);
                    action.Parent = this;

                    break;


                default:
                    result.AddWarning($"Invalid child {child.Name.LocalName} under actions element.", GetErrorPositionInfo(child));
                    break;
            }
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[] { };
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            return Definitions;
        }
    }
}
