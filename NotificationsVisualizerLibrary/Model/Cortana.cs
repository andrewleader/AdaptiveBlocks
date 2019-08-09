using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Model.BaseElements;
using NotificationsVisualizerLibrary.Parsers;

/// <summary>
/// This file is excluded from the project when not in experimental mode
/// </summary>
namespace NotificationsVisualizerLibrary.Model
{
    internal interface ICortana
    {
        FeatureSet SupportedFeatures { get; }

        Visual Visual { get; set; }

        ActionDefinitions ActionDefinitions { get; set; }
    }

    internal class Cortana : BaseElement, ICortana
    {
        public Cortana(FeatureSet supportedFeatures)
        {
            SupportedFeatures = supportedFeatures;
        }

        public FeatureSet SupportedFeatures { get; private set; }

        public Visual Visual { get; set; }

        public ActionDefinitions ActionDefinitions { get; set; }

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

                Visual visual = new Visual(NotificationType.Cortana, SupportedFeatures);
                visual.Parse(result, child);

                Visual = visual;
            }

            else if (child.IsType("actionDefinitions"))
            {
                if (ActionDefinitions != null)
                {
                    result.AddWarning("An actionDefinitions element was already provided. Only the first one will be used.", GetErrorPositionInfo(child));
                    return;
                }

                ActionDefinitions actions = new ActionDefinitions(NotificationType.Cortana, SupportedFeatures);
                actions.Parse(result, child);

                ActionDefinitions = actions;
            }

            else
                result.AddError($"Invalid child {child.Name.LocalName} under toast element.", GetErrorPositionInfo(child));
        }
    }
}
