using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Model.BaseElements;
using NotificationsVisualizerLibrary.Parsers;


/// <summary>
/// This file is excluded when not in experimental mode
/// </summary>
namespace NotificationsVisualizerLibrary.Model
{
    internal sealed class Notification : 
        BaseElement
        , IToast
        , ITile
        , ICortana 
    {
        private const string ATTR_LAUNCH = "launch";
        private const string ATTR_DURATION = "duration";
        private const string ATTR_ACTIVATIONTYPE = "activationType";
        private const string ATTR_SCENARIO = "scenario";
        private const string ATTR_HINT_PEOPLE = "hint-people";

        public FeatureSet SupportedFeatures { get; private set; }

        public NotificationType Context
        {
            get { return NotificationType.Notification; }
        }

        public Notification(FeatureSet supportedFeatures)
        {
            SupportedFeatures = supportedFeatures;
        }

        public Header Header { get; set; }

        public Visual Visual { get; set; }
        
        public ActionDefinitions ActionDefinitions { get; set; }

        public Audio Audio { get; set; }
        
        public States States { get; set; }

        public string Launch { get; set; } = "";

        public Scenario Scenario { get; set; } = Scenario.Default;

        public ActivationType ActivationType { get; set; } = ActivationType.Foreground;

        public Duration Duration { get; set; } = Duration.Short;

        public string People { get; set; }

        public Actions Actions { get; set; }
        ActivationType? IToast.ActivationType { get { return ActivationType.Foreground; } set { } }
        Duration? IToast.Duration { get { return Duration.Short; } set { } }

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
            // Launch is optional
            var attrLaunc = attributes.PopAttribute(ATTR_LAUNCH);
            if (attrLaunc != null)
                Launch = attrLaunc.Value;

            // TODO - check duration is valid

            // activationType is optional
            ActivationType activationType;
            if (TryParseEnum<ActivationType>(result, attributes, ATTR_ACTIVATIONTYPE, out activationType))
                ActivationType = activationType;

            // scenario is optional
            Scenario scenario;
            if (TryParseEnum(result, attributes, ATTR_SCENARIO, out scenario))
                Scenario = scenario;

            // duration is optional
            Duration duration;
            if (TryParseEnum(result, attributes, ATTR_DURATION, out duration))
                Duration = duration;

            // hint-people is optional
            var attrPeople = attributes.PopAttribute(ATTR_HINT_PEOPLE);
            if (attrPeople != null)
                People = attrPeople.Value;
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

                Visual visual = new Visual(NotificationType.Notification, SupportedFeatures);
                visual.Parse(result, child);

                Visual = visual;
            }

            else if (child.IsType("actions"))
            {
                if (Actions != null)
                {
                    result.AddWarning("An actions element was already provided. Only the first one will be used.", GetErrorPositionInfo(child));
                    return;
                }

                Actions actions = new Actions(NotificationType.Toast, SupportedFeatures);
                actions.Parse(result, child);

                Actions = actions;
            }
            
            else if (child.IsType("actionDefinitions"))
            {
                if (ActionDefinitions != null)
                {
                    result.AddWarning("An actionDefinitions element was already provided. Only the first one will be used.", GetErrorPositionInfo(child));
                    return;
                }

                ActionDefinitions actions = new ActionDefinitions(NotificationType.Notification, SupportedFeatures);
                actions.Parse(result, child);

                ActionDefinitions = actions;
            }

            else if (child.IsType("audio"))
            {
                if (Audio != null)
                {
                    result.AddWarning("An audio element was already provided. Only the first one will be used.", GetErrorPositionInfo(child));
                    return;
                }

                Audio audio = new Audio(NotificationType.Notification, SupportedFeatures);
                audio.Parse(result, child);

                Audio = audio;
            }

            else
                result.AddError($"Invalid child {child.Name.LocalName} under notification element.", GetErrorPositionInfo(child));
        }
    }
}
