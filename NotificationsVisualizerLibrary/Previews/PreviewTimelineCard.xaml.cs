using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using NotificationsVisualizerLibrary.Model;
using NotificationsVisualizerLibrary.Parsers;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NotificationsVisualizerLibrary.Renderers;
using System.ComponentModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

/// <summary>
/// This file is excluded when not in experimental mode
/// </summary>
namespace NotificationsVisualizerLibrary
{
    public sealed partial class PreviewTimelineCard : UserControl
    {
        private static XmlTemplateParser _parser = new XmlTemplateParser();

        public PreviewTimelineCard()
        {
            this.InitializeComponent();
        }

        public PreviewTimelineCardProperties Properties
        {
            get { return (PreviewTimelineCardProperties)GetValue(PropertiesProperty); }
            set { SetValue(PropertiesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Properties.  This enables animation, styling, binding, etc...
        public static DependencyProperty PropertiesProperty { get; private set; } =
            DependencyProperty.Register("Properties", typeof(PreviewTimelineCardProperties), typeof(PreviewTimelineCard), new PropertyMetadata(null));



        public PreviewTimelineCardSize CardSize
        {
            get { return (PreviewTimelineCardSize)GetValue(CardSizeProperty); }
            set { SetValue(CardSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardSize.  This enables animation, styling, binding, etc...
        public static DependencyProperty CardSizeProperty { get; private set; } =
            DependencyProperty.Register("CardSize", typeof(PreviewTimelineCardSize), typeof(PreviewTimelineCard), new PropertyMetadata(PreviewTimelineCardSize.Hero, OnCardSizeChanged));

        private static void OnCardSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as PreviewTimelineCard).OnCardSizeChanged(e);
        }

        private void OnCardSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            switch (CardSize)
            {
                case PreviewTimelineCardSize.Hero:
                    VisualStateManager.GoToState(this, "Hero", true);
                    break;

                case PreviewTimelineCardSize.Detail:
                    VisualStateManager.GoToState(this, "Detail", true);
                    break;
            }
        }
    }

    public enum PreviewTimelineCardSize
    {
        Detail,
        Hero
    }

    public sealed class PreviewTimelineCardProperties : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _displayText = "";
        public string DisplayText
        {
            get { return _displayText; }
            set { BindableBaseHelper.SetProperty(this, ref _displayText, value, PropertyChanged); }
        }

        private string _appName = "";
        public string AppName
        {
            get { return _appName; }
            set { BindableBaseHelper.SetProperty(this, ref _appName, value, PropertyChanged); }
        }

        public PreviewTimelineCardVisualElements VisualElements { get; private set; } = new PreviewTimelineCardVisualElements();

        public void Import(PreviewTimelineCardProperties other)
        {
            DisplayText = other.DisplayText;
            AppName = other.AppName;

            VisualElements.Description = other.VisualElements.Description;
            VisualElements.ImageIcon = other.VisualElements.ImageIcon;

            VisualElements.Hero.AdaptiveContent = other.VisualElements.Hero.AdaptiveContent;
            VisualElements.Hero.BackgroundImage = other.VisualElements.Hero.BackgroundImage;
        }
    }

    public sealed class PreviewTimelineCardVisualElements : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _description = "";
        public string Description
        {
            get { return _description; }
            set { BindableBaseHelper.SetProperty(this, ref _description, value, PropertyChanged); }
        }

        private Uri _imageIcon;
        public Uri ImageIcon
        {
            get { return _imageIcon; }
            set { BindableBaseHelper.SetProperty(this, ref _imageIcon, value, PropertyChanged); }
        }

        public PreviewTimelineCardHeroVisualElements Hero { get; private set; } = new PreviewTimelineCardHeroVisualElements();
    }

    public sealed class PreviewTimelineCardHeroVisualElements : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Uri _backgroundImage;
        public Uri BackgroundImage
        {
            get { return _backgroundImage; }
            set { BindableBaseHelper.SetProperty(this, ref _backgroundImage, value, PropertyChanged); }
        }

        private string _adaptiveContent;
        public string AdaptiveContent
        {
            get { return _adaptiveContent; }
            set { BindableBaseHelper.SetProperty(this, ref _adaptiveContent, value, PropertyChanged); }
        }
    }

    internal class AdaptiveStringToVisualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string str = value as string;
            if (str != null)
            {
                var parseResult = new XmlTemplateParser().ParseAdaptiveContent(str, FeatureSet.GetExperimental());
                if (parseResult.IsOkForRender())
                {
                    return AdaptiveRenderer.Render(parseResult.AdaptiveContent, new Thickness(0));
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
