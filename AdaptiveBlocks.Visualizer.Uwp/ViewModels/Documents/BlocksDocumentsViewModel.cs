using AdaptiveBlocks.Visualizer.Uwp.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.UserActivities;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AdaptiveBlocks.Visualizer.Uwp.ViewModels.Documents
{
    public class BlocksDocumentsViewModel : BaseDocumentsViewModel
    {
        public BlocksDocumentsViewModel(PropertiesViewModel properties) : base(properties)
        {
            ShowRichRendererPreview();
            ShowTransformedCardPreview();

            PropertyChanged += BlocksDocumentsViewModel_PropertyChanged;
        }

        private void BlocksDocumentsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CurrentDocument):
                    if (CurrentDocument != null)
                    {
                        RichRendererPreviewPage.RenderNewBlock((CurrentDocument as BlocksDocumentViewModel).LastBlockSource);
                        TransformedCardPreviewPage.RenderNewBlock((CurrentDocument as BlocksDocumentViewModel).LastBlockSource);
                    }
                    break;
            }
        }

        public override string CategoryTitle => "Blocks";

        protected override string SamplesFolderName => "BlocksSamples";

        protected override BaseDocumentViewModel CreateNewDocument()
        {
            return BlocksDocumentViewModel.CreateNew(Properties);
        }

        protected override PayloadEditorSettings GetSettings()
        {
            return OverallSettings.BlocksPayloadEditor;
        }

        protected override async Task<BaseDocumentViewModel> LoadFromFileAsync(StorageFile file, string token, PropertiesViewModel properties, bool isSaveable)
        {
            return await BaseDocumentViewModel.CreateFromFileAsync<BlocksDocumentViewModel>(file, token, properties, isSaveable);
        }

        public void ShowRichRendererPreview()
        {
            ShowPageAsWindow<RichRendererPreviewPage>("Rich preview");
        }

        public void ShowTransformedCardPreview()
        {
            ShowPageAsWindow<TransformedCardPreviewPage>("Transformed Adaptive Card");
        }

        public async void ShowPageAsWindow<TPage>(string windowTitle)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(TPage), null);
                Window.Current.Content = frame;
                ApplicationView.GetForCurrentView().SetPreferredMinSize(new Windows.Foundation.Size(20, 20));
                //ApplicationView.PreferredLaunchViewSize = new Windows.Foundation.Size(300, 400);
                //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
                ApplicationView.GetForCurrentView().Title = windowTitle;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }

        public async Task ShareToAllAsync()
        {
            await ShareToUserActivitiesAsync();
            await ShareToToastNotificationAsync();
        }

        public async Task ShareToUserActivitiesAsync()
        {
            try
            {
                if (CurrentDocument is BlocksDocumentViewModel currentBlocksDocument && currentBlocksDocument.CurrentBlock != null)
                {
                    var activity = await UserActivityChannel.GetDefault().GetOrCreateUserActivityAsync(CurrentDocument.Name);

                    await new Transformers.UserActivities.AdaptiveBlockToUserActivityTransformer(activity).TransformAsync(currentBlocksDocument.CurrentBlock);
                    if (activity.ActivationUri == null)
                    {
                        activity.ActivationUri = new Uri("https://adaptivecards.io");
                    }

                    await activity.SaveAsync();

                    var session = activity.CreateSession();

                    await Task.Delay(1);
                    session.Dispose();
                }
            }
            catch { }
        }

        public async Task ShareToToastNotificationAsync()
        {
            try
            {
                if (CurrentDocument is BlocksDocumentViewModel currentBlocksDocument && currentBlocksDocument.CurrentBlock != null)
                {
                    var toastContent = (await new Transformers.ToastContentTransformer.AdaptiveBlockToToastContentTransformer().TransformAsync(currentBlocksDocument.CurrentBlock)).Result;
                    if (toastContent != null)
                    {
                        var toast = new ToastNotification(toastContent.GetXml());
                        ToastNotificationManager.CreateToastNotifier().Show(toast);
                    }
                }
            }
            catch { }
        }
    }
}
