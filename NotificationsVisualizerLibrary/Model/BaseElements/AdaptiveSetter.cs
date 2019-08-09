using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Parsers;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    internal enum SetterProperties
    {
        [XmlValue("hint-visible")]
        HintVisible
    }

    internal class AdaptiveSetter : AdaptiveChildElement
    {
        public const string ELEMENT_NAME = "setter";

        public AdaptiveSetter(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        internal const string ATTR_TARGET = "target";
        internal const string ATTR_PROPERTY = "property";
        internal const string ATTR_VALUE = "value";

        public string Target { get; set; }

        public SetterProperties Property { get; set; }

        public string Value { get; set; }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[] { };
        }

        internal void Parse(ParseResult result, XElement node)
        {
            AttributesHelper attributes = new AttributesHelper(node.Attributes());

            ParseKnownAttributes(node, attributes, result);

            HandleRemainingAttributes(attributes, result);
        }

        internal virtual void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result)
        {
            // target is required
            XAttribute attrTarget = attributes.PopAttribute(ATTR_TARGET);
            if (attrTarget == null)
            {
                result.AddError($"{ATTR_TARGET} attribute on {ELEMENT_NAME} element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }

            this.Target = attrTarget.Value;


            // property is required
            SetterProperties property;
            if (TryParseEnum(result, attributes, ATTR_PROPERTY, out property))
                this.Property = property;
            else
            {
                result.AddErrorButRenderAllowed($"{ATTR_PROPERTY} attribute on {ELEMENT_NAME} element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }


            // value is required
            XAttribute attrValue = attributes.PopAttribute(ATTR_VALUE);
            if (attrValue == null)
            {
                result.AddErrorButRenderAllowed($"{ATTR_VALUE} attribute on {ELEMENT_NAME} element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }

            this.Value = attrValue.Value;
        }
    }
}
