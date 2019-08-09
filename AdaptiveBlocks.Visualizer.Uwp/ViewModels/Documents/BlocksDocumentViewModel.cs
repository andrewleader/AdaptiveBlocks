
using AdaptiveBlocks.Visualizer.Uwp.Previews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Visualizer.Uwp.ViewModels.Documents
{
    public class BlocksDocumentViewModel : BaseDocumentViewModel
    {
        public PreviewSimpleBlockHost PreviewSimpleBlockHost { get; private set; } = new PreviewSimpleBlockHost();

        public PreviewRowsBlockHost PreviewRowsBlockHost { get; private set; } = new PreviewRowsBlockHost();

        public PreviewFullBlockHost PreviewFullBlockHost { get; private set; } = new PreviewFullBlockHost();

        public PreviewRichWatch PreviewRichWatchHost { get; private set; } = new PreviewRichWatch();

        public PreviewAudioHost PreviewAudioHost { get; private set; } = new PreviewAudioHost();

        public PreviewTimelineWebFullSize PreviewTimelineWebFullSize { get; private set; } = new PreviewTimelineWebFullSize();

        public PreviewTimelineWebHighDensity PreviewTimelineWebHighDensity { get; private set; } = new PreviewTimelineWebHighDensity();

        public PreviewTimelineAndroid PreviewTimelineAndroid { get; private set; } = new PreviewTimelineAndroid();

        public PreviewAndroidNotification PreviewAndroidNotification { get; private set; } = new PreviewAndroidNotification();

        public PreviewToastHost PreviewToastHost { get; private set; } = new PreviewToastHost();

        public PreviewToastCardHost PreviewToastCardHost { get; private set; } = new PreviewToastCardHost();

        public PreviewJumplist PreviewJumplist { get; private set; } = new PreviewJumplist();

        public PreviewOutlookActionableMessage PreviewOutlookActionableMessage { get; private set; } = new PreviewOutlookActionableMessage();

        public PreviewGitHubPullRequest PreviewGitHubPullRequest { get; private set; } = new PreviewGitHubPullRequest();

        public PreviewRecentActivities PreviewRecentActivities { get; private set; } = new PreviewRecentActivities();

        public PreviewEchoSpot PreviewEchoSpot { get; private set; } = new PreviewEchoSpot();

        private bool _usesDataBinding;
        public bool UsesDataBinding
        {
            get { return _usesDataBinding; }
            set { SetProperty(ref _usesDataBinding, value); }
        }

        public BlocksDocumentViewModel(PropertiesViewModel properties) : base(properties)
        {
            ApplyBuildNumber();
            ApplyProperties();
        }

        public IEnumerable<IPreviewBlockHost> GetPreviews()
        {
            return new IPreviewBlockHost[]
            {
                PreviewSimpleBlockHost,
                PreviewRowsBlockHost,
                PreviewFullBlockHost,
                PreviewRichWatchHost,
                PreviewAudioHost,
                PreviewJumplist,
                PreviewTimelineWebHighDensity,
                PreviewTimelineWebFullSize,
                PreviewTimelineAndroid,
                PreviewToastHost,
                PreviewAndroidNotification,
                PreviewOutlookActionableMessage,
                PreviewGitHubPullRequest,
                PreviewRecentActivities,
                PreviewToastCardHost,
                PreviewEchoSpot
            };
        }

        private AdaptiveBlockParseResult _lastParseResult;
        protected override async Task LoadPayloadAsync(string payload)
        {
            _lastParseResult = AdaptiveBlock.Parse(payload);
            CurrentBlock = _lastParseResult?.Block;

            if (_lastParseResult.Block != null)
            {
                await _lastParseResult.Block.ResolveAsync();
            }

            PreviewBlockHostViewModel updateArgs = new PreviewBlockHostViewModel()
            {
                BlockContent = _lastParseResult.Block?.View?.Content
            };

            switch (base.Name)
            {
                case "OfficeFile.json":
                    updateArgs.AssignApp("PowerPoint", "PowerPoint");
                    break;

                case "SkypeCallHistory.json":
                    updateArgs.AssignApp("Skype", "Skype");
                    break;

                case "SummerRetreats.json":
                case "GitHubUserActivity.json":
                    updateArgs.AssignApp("Edge", "Edge");
                    break;

                case "GoogleDocsSlide.json":
                    updateArgs.AssignApp("Google Slides", "GoogleSlides");
                    break;
            }

            foreach (var preview in GetPreviews())
            {
                try
                {
                    preview.Update(_lastParseResult.Block?.View?.Content, _lastParseResult.Block, updateArgs);
                }
                catch { }
            }

            RichRendererPreviewPage.RenderNewBlock(_lastParseResult.Block);
            TransformedCardPreviewPage.RenderNewBlock(_lastParseResult.Block);

            if (_lastParseResult != null)
            {
                this.MakeErrorsLike(_lastParseResult.Errors.ToList());
            }
        }

        private AdaptiveBlock m_currentBlock;
        public AdaptiveBlock CurrentBlock
        {
            get { return m_currentBlock; }
            private set { SetProperty(ref m_currentBlock, value); }
        }

        public AdaptiveBlockContent LastBlock => _lastParseResult?.Block?.View?.Content;
        public AdaptiveBlock LastBlockSource => _lastParseResult?.Block;

        public static BlocksDocumentViewModel CreateNew(PropertiesViewModel properties)
        {
            return new BlocksDocumentViewModel(properties)
            {
                _payload = @"{\n  ""title"": ""My title"",\n  ""subtitle"": ""My subtitle""\n}"
            };
        }

        protected override void ApplyBuildNumber()
        {
        }

        protected override void ApplyProperties()
        {
        }

        public void RoundTripSerialize()
        {
            Payload = _lastParseResult?.Block?.ToJson() ?? "";
        }

        public void RemoveBlock(AdaptiveBlock block)
        {
            try
            {
                if (CurrentBlock != null)
                {
                    if (CurrentBlock == block)
                    {
                        Payload = "";
                    }
                    else if (CurrentBlock.RemoveBlock(block))
                    {
                        Payload = CurrentBlock.ToJson();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
