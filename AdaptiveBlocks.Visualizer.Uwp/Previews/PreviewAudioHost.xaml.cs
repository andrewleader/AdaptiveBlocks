using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;

using AdaptiveBlocks.Speech;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AdaptiveBlocks.Visualizer.Uwp.Previews
{
    public sealed partial class PreviewAudioHost : UserControl, IPreviewBlockHost
    {
        private List<AdaptiveSpeechPrompt> m_prompts;
        private MediaPlayerHelper m_mediaPlayerHelper;

        public PreviewAudioHost()
        {
            this.InitializeComponent();

            m_mediaPlayerHelper = new MediaPlayerHelper(MediaElementAudio);
        }

        public void Update(AdaptiveBlockContent block, AdaptiveBlock sourceBlock, PreviewBlockHostViewModel args)
        {
            try
            {
                m_prompts = block.GetSpeechPrompts();
                ItemsControlPrompts.ItemsSource = m_prompts;
            }
            catch { }
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            TogglePlay();
        }

        private async void Play()
        {
            if (m_prompts == null || m_prompts.Count == 0)
            {
                return;
            }

            try
            {
                await SayPromptsAsync(m_prompts);
            }
            catch { }
        }

        private void TogglePlay()
        {
            if (m_mediaPlayerHelper.IsPlaying)
            {
                Stop();
            }
            else
            {
                Play();
            }
        }

        private void Stop()
        {
            m_mediaPlayerHelper.CancelExisting();
        }

        private async Task SayPromptsAsync(IEnumerable<AdaptiveSpeechPrompt> prompts)
        {
            foreach (var p in prompts.ToArray())
            {
                await SayPromptAsync(p);
            }
        }

        private async Task SayPromptAsync(AdaptiveSpeechPrompt prompt)
        {
            await SayTextAsync(prompt.GetTextToSay());

            if (prompt.Choices.Any())
            {
                var choices = prompt.GetSpeechChoices();
                var speechRecognizer = new SpeechRecognizer();
                speechRecognizer.Constraints.Add(new SpeechRecognitionListConstraint(choices));
                await speechRecognizer.CompileConstraintsAsync();
                var results = await speechRecognizer.RecognizeWithUIAsync();
                var selectedChoice = results.Text;

                int index = choices.ToList().IndexOf(selectedChoice);
                await Task.Delay(500); // Delay some time so the speaking doesn't get cut off
                if (index == -1)
                {
                    await SayTextAsync("I'm sorry, I didn't understand that.");
                    return;
                }
                if (index >= prompt.Choices.Count)
                {
                    // "I'm done"
                    return;
                }
                var choice = prompt.Choices[index];
                if (choice.FollowUpPrompts.Any())
                {
                    await SayPromptsAsync(choice.FollowUpPrompts);
                }
                else
                {
                    await SayTextAsync("Excellent, we're done!");
                }
            }
        }

        private async Task SayTextAsync(string text)
        {
            await m_mediaPlayerHelper.SayTextAsync(text);
        }

        private class MediaPlayerHelper
        {
            private MediaElement m_element;
            private TaskCompletionSource<bool> m_taskCompletionSource;

            public MediaPlayerHelper(MediaElement element)
            {
                m_element = element;
                m_element.MediaEnded += M_element_MediaEnded;
            }

            public async Task SayTextAsync(string text)
            {
                CancelExisting();

                m_taskCompletionSource = new TaskCompletionSource<bool>();
                var ourTask = m_taskCompletionSource.Task;

                var synth = new SpeechSynthesizer();
                synth.Options.SpeakingRate = 1.4;
                var stream = await synth.SynthesizeTextToStreamAsync(text);
                if (ourTask.IsCanceled)
                {
                    return;
                }

                m_element.SetSource(stream, stream.ContentType);
                m_element.Play();

                await ourTask;
                m_taskCompletionSource = null;
            }

            public void CancelExisting()
            {
                m_element.Stop();
                m_taskCompletionSource?.SetCanceled();
                m_taskCompletionSource = null;
            }

            private void M_element_MediaEnded(object sender, RoutedEventArgs e)
            {
                m_taskCompletionSource?.SetResult(true);
            }

            public bool IsPlaying
            {
                get { return m_taskCompletionSource != null && !m_taskCompletionSource.Task.IsCompleted; }
            }
        }
    }
}
