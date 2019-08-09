using AdaptiveBlocks.Visualizer.Uwp.Helpers;
using AdaptiveBlocks.Visualizer.Uwp.Renderer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Popups;

namespace AdaptiveBlocks.Visualizer.Uwp.ViewModels.Documents
{
    public abstract class BaseDocumentViewModel : BindableBase
    {
        public event EventHandler OnRequestClose;
        public event EventHandler OnRequestSaveFileTokens;

        public string Token { get; private set; }
        public StorageFile File { get; private set; }

        private string _name = "Untitled";
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public PropertiesViewModel Properties { get; private set; }

        public BaseDocumentViewModel(PropertiesViewModel properties)
        {
            Properties = properties;
        }

        private bool _isPropertiesOutdated;
        private bool _isBuildNumberOutdated;
        public void FlagPropertiesChanged()
        {
            _isPropertiesOutdated = true;
        }

        public void FlagBuildNumberChanged()
        {
            _isBuildNumberOutdated = true;
        }

        protected abstract void ApplyBuildNumber();

        protected abstract void ApplyProperties();

        public bool IsOutdated { get; set; } = true;

        private bool _isUnsaved = true;
        public bool IsUnsaved
        {
            get { return _isUnsaved; }
            set { SetProperty(ref _isUnsaved, value); }
        }

        public bool IsSaveable { get; private set; }

        private bool _firstLoad = true;

        protected string _payload = "";
        public string Payload
        {
            get { return _payload; }
            set { SetPayload(value); IsUnsaved = true; }
        }

        private void SetPayload(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (object.Equals(_payload, value))
            {
                return;
            }

            SetProperty(ref _payload, value, nameof(Payload));

            Reload();
        }

        public async void Reload()
        {
            IsOutdated = true;

            if (!IsLoading)
            {
                if (_firstLoad)
                {
                    _firstLoad = false;
                }
                else
                {
                    IsLoading = true;

                    await Task.Delay(1000);

                    IsLoading = false;
                }

                NotifyPropertyChanged(DelayedUpdatePayload);
                await LoadAsync();
            }
        }

        public virtual void ReloadIfNeeded()
        {
            bool needsReload = false;

            if (_isBuildNumberOutdated)
            {
                ApplyBuildNumber();
                _isBuildNumberOutdated = false;
                needsReload = true;
            }

            if (_isPropertiesOutdated)
            {
                ApplyProperties();
                _isPropertiesOutdated = false;
            }

            needsReload = needsReload || IsOutdated;

            if (needsReload)
            {
                Reload();
            }
        }

        private bool _isLoading;
        /// <summary>
        /// Represents whether the system is delaying loading. Views should indicate this.
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            private set { SetProperty(ref _isLoading, value); }
        }

        /// <summary>
        /// This property will be notified of changes on a delayed schedule, so that it's not
        /// changing every single time a character is typed. Views presenting 
        /// </summary>
        public string DelayedUpdatePayload
        {
            get { return Payload; }
        }

        public ObservableCollection<ParseError> Errors { get; private set; } = new ObservableCollection<ParseError>();

        public static async Task<T> CreateFromFileAsync<T>(StorageFile file, string token, PropertiesViewModel properties, bool isSaveable)
            where T : BaseDocumentViewModel
        {
            T answer = (T)Activator.CreateInstance(typeof(T), properties);
            answer.IsSaveable = isSaveable;

            await answer.LoadFromFileAsync(file, token, true);

            return answer;
        }

        protected async Task LoadFromFileAsync(StorageFile file, string token, bool assignPayloadWithoutLoading)
        {
            this.Name = file.Name;
            this.File = file;
            this.Token = token;
            this.IsUnsaved = false;

            string payloadText = await ModifyFilePayloadTextAsync(await FileIO.ReadTextAsync(file));
            if (assignPayloadWithoutLoading)
            {
                _payload = payloadText;
            }
            else
            {
                SetPayload(payloadText);
            }
        }

        protected virtual Task<string> ModifyFilePayloadTextAsync(string payloadText)
        {
            return Task.FromResult(payloadText);
        }

        public async Task SaveAsync()
        {
            try
            {
                if (!IsSaveable)
                {
                    await SaveAsAsync();
                    return;
                }

                await FileIO.WriteTextAsync(File, Payload);

                IsUnsaved = false;
            }
            catch (Exception ex)
            {
                //Microsoft.HockeyApp.HockeyClient.Current.TrackException(ex);
                var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        public async Task SaveAsAsync()
        {
            try
            {
                FileSavePicker savePicker = new FileSavePicker()
                {
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                    SuggestedFileName = Name
                };

                if (IsSaveable)
                {
                    savePicker.SuggestedSaveFile = File;
                }

#if CARDS
                if (IsXml(Payload))
                {
                    savePicker.FileTypeChoices.Add("XML", new string[] { ".xml" });
                }
                else
                {
                    savePicker.FileTypeChoices.Add("JSON", new string[] { ".json" });
                }
#else
                savePicker.FileTypeChoices.Add("XML", new string[] { ".xml" });
#endif

                StorageFile file = await savePicker.PickSaveFileAsync();

                if (file != null)
                {
                    File = file;
                    Name = File.Name;
                    Token = StorageApplicationPermissions.FutureAccessList.Add(file);
                    IsSaveable = true;

                    await SaveAsync();

                    // Save file tokens since the file changed, so next time user opens app it has the correct file
                    OnRequestSaveFileTokens(this, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                //Microsoft.HockeyApp.HockeyClient.Current.TrackException(ex);
                var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        public static bool IsXml(string text)
        {
#if CARDS
            return text.TrimStart().StartsWith("<");
#else
            return true;
#endif
        }

        /// <summary>
        /// Closes the document
        /// </summary>
        public void Close()
        {
            OnRequestClose?.Invoke(this, new EventArgs());
        }

        private async Task LoadAsync()
        {
            if (string.IsNullOrWhiteSpace(Payload))
            {
                SetSingleError(new ParseError(ParseErrorType.Error, "Invalid payload"));
                IsOutdated = false;
                return;
            }

            string payload = Payload;

            try
            {
                await LoadPayloadAsync(payload);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                MakeErrorsLike(new List<ParseError>()
                {
                    new ParseError(ParseErrorType.Error, "Loading failed")
                });
            }

            IsOutdated = false;
        }

        protected abstract Task LoadPayloadAsync(string payload);

        protected void SetSingleError(ParseError error)
        {
            MakeErrorsLike(new List<ParseError>() { error });
        }

        protected void MakeErrorsLike(List<ParseError> errors)
        {
            errors.Sort(new ParseErrorComparer());
            Errors.MakeListLike(errors);
        }

        private class ParseErrorComparer : IComparer<ParseError>
        {
            public int Compare(ParseError x, ParseError y)
            {
                int answer = ((int)x.Type).CompareTo((int)y.Type);
                if (answer != 0)
                {
                    return answer;
                }

                if (x.Position == null)
                {
                    if (y.Position == null)
                    {
                        return 0;
                    }

                    return -1;
                }
                if (y.Position == null)
                {
                    return 1;
                }

                return x.Position.LineNumber.CompareTo(y.Position.LineNumber);
            }
        }
    }
}
