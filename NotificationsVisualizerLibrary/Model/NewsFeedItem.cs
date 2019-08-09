using NotificationsVisualizerLibrary.Model.BaseElements;
using NotificationsVisualizerLibrary.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NotificationsVisualizerLibrary.Model
{
    internal class NewsFeedItem : BaseElement
    {
        public FeatureSet SupportedFeatures { get; private set; }

        public Visual Visual { get; set; }

        public NewsFeedItem(FeatureSet supportedFeatures)
        {
            SupportedFeatures = supportedFeatures;
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[] { };
        }

        internal void Parse(ParseResult result, XElement node)
        {
            if (!XmlTemplateParser.EnsureNodeOnlyHasElementsAsChildren(result, node))
                throw new IncompleteElementException();

            AttributesHelper attributes = new AttributesHelper(node.Attributes());


            ParseKnownAttributes(attributes, result);

            HandleRemainingAttributes(attributes, result);

            foreach (XElement n in node.Elements())
            {
                try
                {
                    HandleChild(result, n);
                }

                catch (IncompleteElementException) { }
            }
        }

        private void ParseKnownAttributes(AttributesHelper attributes, ParseResult result)
        {
            // None right now
        }

        private void HandleChild(ParseResult result, XElement child)
        {
            if (child.IsType("visual"))
            {
                if (Visual != null)
                {
                    result.AddWarning("A visual element was already provided. Only the first one will be used.", GetErrorPositionInfo(child));
                    return;
                }

                Visual visual = new Visual(NotificationType.NewsFeed, SupportedFeatures);
                visual.Parse(result, child);

                Visual = visual;
            }

            else
                result.AddError($"Invalid child {child.Name.LocalName} under newsFeed element.", GetErrorPositionInfo(child));
        }
    }
}
