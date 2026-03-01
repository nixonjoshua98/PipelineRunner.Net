namespace PipelineRunner.Net
{
    internal sealed class CachedPipeline
    {
        public readonly object Delegate;

        public CachedPipeline(object @delegate)
        {
            Delegate = @delegate;
        }
    }
}