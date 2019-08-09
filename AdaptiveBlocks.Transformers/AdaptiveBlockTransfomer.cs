using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Transformers
{
    public abstract class AdaptiveBlockTransformer<T>
    {
        public async Task<AdaptiveBlockTransformResult<T>> TransformAsync(AdaptiveBlock block)
        {
            var result = new AdaptiveBlockTransformResult<T>();
            await TransformAsync(block, result);
            return result;
        }

        protected abstract Task TransformAsync(AdaptiveBlock block, AdaptiveBlockTransformResult<T> result);
    }

    public class AdaptiveBlockTransformResult<T>
    {
        public T Result { get; set; }

        public List<string> Warnings { get; } = new List<string>();

        public List<string> Errors { get; } = new List<string>();
    }
}
