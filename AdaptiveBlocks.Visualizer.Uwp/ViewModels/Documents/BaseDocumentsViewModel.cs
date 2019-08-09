using AdaptiveBlocks.Visualizer.Uwp.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Popups;

namespace AdaptiveBlocks.Visualizer.Uwp.ViewModels.Documents
{
    public abstract class BaseDocumentsViewModel : BindableBase
    {
        public BaseDocumentsViewModel(PropertiesViewModel properties)
        {
            Properties = properties;
            properties.PropertyChanged += Properties_PropertyChanged;
            Initialize();
        }

        /// <summary>
        /// The category title, like "Live Tiles" or "Toasts"
        /// </summary>
        public abstract string CategoryTitle { get; }

        private void Properties_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //switch (e.PropertyName)
            //{
            //    case nameof(Properties.SelectedBuildNumber):
            //        foreach (var d in OpenDocuments)
            //        {
            //            d.FlagBuildNumberChanged();
            //        }
            //        CurrentDocument?.ReloadIfNeeded();
            //        break;

            //    default:
            //        foreach (var d in OpenDocuments)
            //        {
            //            d.FlagPropertiesChanged();
            //        }
            //        CurrentDocument?.ReloadIfNeeded();
            //        break;
            //}
        }

        public PropertiesViewModel Properties { get; private set; }

        private async void Initialize()
        {
            try
            {
                _currentDocumentToken = new SettingsValueHelper<string>(this.GetType().Name + "CurrDocToken", null);
                Settings = GetSettings();

                await LoadFilesAsync();

                IsEnabled = true;
            }
            catch (Exception ex)
            {
                var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        public ObservableCollection<BaseDocumentViewModel> OpenDocuments { get; private set; } = new ObservableCollection<BaseDocumentViewModel>();

        private SettingsValueHelper<string> _currentDocumentToken;

        private BaseDocumentViewModel _currentDocument;
        public BaseDocumentViewModel CurrentDocument
        {
            get { return _currentDocument; }
            set
            {
                SetProperty(ref _currentDocument, value);

                if (value != null)
                {
                    _currentDocumentToken.Value = value.Token;
                    value.ReloadIfNeeded();
                }

                else
                {
                    _currentDocumentToken.Value = null;
                }
            }
        }

        public ObservableCollection<AddDocumentListItem> AddDocumentItems { get; private set; } = new ObservableCollection<AddDocumentListItem>()
        {
            AddDocumentListItem.NewDocument,
            AddDocumentListItem.OpenDocument
        };

        public PayloadEditorSettings Settings { get; private set; }

        protected abstract PayloadEditorSettings GetSettings();

        private void NewDocument()
        {
            AddDocument(CreateNewDocument());
            CurrentDocument = OpenDocuments.Last();
        }

        protected abstract BaseDocumentViewModel CreateNewDocument();

        private async void OpenDocument()
        {
            try
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.List;
                openPicker.FileTypeFilter.Add(".json");

                StorageFile file = await openPicker.PickSingleFileAsync();
                if (file != null)
                {
                    string token = StorageApplicationPermissions.FutureAccessList.Add(file);
                    AddAndSelectDocument(await LoadFromFileAsync(file, token, Properties, isSaveable: true));
                }
                else
                {
                }
            }
            catch { }
        }

        public async Task AddDocumentAsync(AddDocumentListItem doc)
        {
            if (doc == AddDocumentListItem.NewDocument)
            {
                //Microsoft.HockeyApp.HockeyClient.Current.TrackEvent("NewDocumentClicked");
                NewDocument();
                return;
            }

            if (doc == AddDocumentListItem.OpenDocument)
            {
                //Microsoft.HockeyApp.HockeyClient.Current.TrackEvent("OpenDocumentClicked");
                OpenDocument();
                return;
            }

            //Microsoft.HockeyApp.HockeyClient.Current.TrackEvent("OpenSampleClicked", new Dictionary<string, string>()
            //{
            //    { "SampleFile", doc.File.Name }
            //});

            AddAndSelectDocument(await LoadFromFileAsync(doc.File, "SampleFile:" + doc.File.Name, Properties, isSaveable: false));
        }

        private async void AddAndSelectDocument(BaseDocumentViewModel doc)
        {
            AddDocument(doc);
            CurrentDocument = OpenDocuments.LastOrDefault();
            await SaveFileTokensAsync();
        }

        private void AddDocument(BaseDocumentViewModel doc)
        {
            OpenDocuments.Add(doc);
            doc.OnRequestClose += Doc_OnRequestClose;
            doc.OnRequestSaveFileTokens += Doc_OnRequestSaveFileTokens;
        }

        private async void Doc_OnRequestSaveFileTokens(object sender, EventArgs e)
        {
            try
            {
                await SaveFileTokensAsync();
                _currentDocumentToken.Value = _currentDocument?.Token;
            }
            catch { }
        }

        private void Doc_OnRequestClose(object sender, EventArgs e)
        {
            CloseDocument(sender as BaseDocumentViewModel);
        }

        protected abstract Task<BaseDocumentViewModel> LoadFromFileAsync(StorageFile file, string token, PropertiesViewModel properties, bool isSaveable);

        private async Task LoadFilesAsync()
        {
            OpenDocuments.Clear();

            var tokens = await Settings.GetFileTokensAsync();
            var documents = new List<BaseDocumentViewModel>();
            foreach (string token in tokens)
            {
                try
                {
                    StorageFile file;
                    bool isSaveable = false;
                    if (token.StartsWith("SampleFile:"))
                    {
                        string fileName = token.Substring("SampleFile:".Length);
                        file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///{SamplesFolderName}/{fileName}"));
                    }
                    else
                    {
                        file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
                        isSaveable = true;
                    }

                    if (file != null)
                    {
                        documents.Add(await LoadFromFileAsync(file, token, Properties, isSaveable));
                    }
                }
                catch { }
            }

            try
            {
                var samplesFolder = await Package.Current.InstalledLocation.GetFolderAsync(SamplesFolderName);
                foreach (var file in await samplesFolder.GetFilesAsync())
                {
                    if (file.FileType.ToLower().Equals(".json"))
                    {
                        AddDocumentItems.Add(new AddDocumentListItem()
                        {
                            DisplayName = file.Name,
                            File = file
                        });
                    }
                }
            }
            catch { }

            foreach (var doc in documents)
            {
                AddDocument(doc);
            }

            var currDoc = documents.FirstOrDefault(i => i.Token == _currentDocumentToken.Value);
            if (currDoc == null)
            {
                currDoc = documents.FirstOrDefault();
            }

            CurrentDocument = currDoc;
        }

        protected abstract string SamplesFolderName { get; }

        public async void CloseDocument(BaseDocumentViewModel document)
        {
            // TODO: Check if not saved
            int index = OpenDocuments.IndexOf(document);
            if (index != -1)
            {
                OpenDocuments.RemoveAt(index);

                if (index < OpenDocuments.Count)
                {
                    CurrentDocument = OpenDocuments[index];
                }
                else
                {
                    CurrentDocument = OpenDocuments.LastOrDefault();
                }

                if (document.Token != null)
                {
                    await SaveFileTokensAsync();
                }
            }
        }

        private async Task SaveFileTokensAsync()
        {
            await Settings.SaveFileTokensAsync(OpenDocuments.Select(i => i.Token));
        }
    }
}
