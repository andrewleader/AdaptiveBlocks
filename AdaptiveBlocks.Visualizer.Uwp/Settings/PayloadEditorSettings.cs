using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace AdaptiveBlocks.Visualizer.Uwp.Settings
{
    public abstract class PayloadEditorSettings
    {
        public async Task<string[]> GetFileTokensAsync()
        {
            try
            {
                var file = await ApplicationData.Current.LocalCacheFolder.GetFileAsync(OpenTokensFileName);
                return (await FileIO.ReadLinesAsync(file)).ToArray();
            }
            catch { return new string[0]; }
        }

        public async Task SaveFileTokensAsync(IEnumerable<string> tokens)
        {
            try
            {
                tokens = tokens.Where(i => i != null).ToArray();
                var file = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(OpenTokensFileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteLinesAsync(file, tokens);
            }
            catch { }
        }

        protected abstract string OpenTokensFileName { get; }

        protected static void SetFolder(string token, StorageFolder folder)
        {
            if (folder == null)
            {
                StorageApplicationPermissions.FutureAccessList.Remove(token);
                return;
            }

            StorageApplicationPermissions.FutureAccessList.AddOrReplace(token, folder);
        }

        protected static void SetFile(string token, StorageFile file)
        {
            if (file == null)
            {
                StorageApplicationPermissions.FutureAccessList.Remove(token);
                return;
            }

            StorageApplicationPermissions.FutureAccessList.AddOrReplace(token, file);
        }
    }

    public class BlocksPayloadEditorSettings : PayloadEditorSettings
    {
        protected override string OpenTokensFileName => "BlocksFiles.dat";
    }
}
