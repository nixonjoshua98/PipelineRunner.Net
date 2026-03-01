using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipelineRunner.Net
{
    internal sealed partial class BuiltPipeline : IBuiltPipeline
    {
        private readonly FilterDescriptor[] _descriptors;
        private readonly Dictionary<Type, CachedPipeline> _pipelines = new Dictionary<Type, CachedPipeline>();

        public BuiltPipeline(List<FilterDescriptor> descriptors)
        {
            _descriptors = descriptors.ToArray();
        }

        public async Task ExecuteAsync<TContext>(TContext context) where TContext : class, IPipelineContext
        {
            var pipeline = GetOrCreatePipeline<TContext>();

            await pipeline(context);
        }

        private PipelineDelegate<TContext> GetOrCreatePipeline<TContext>() where TContext : class, IPipelineContext
        {
            var contextType = typeof(TContext);

            if (_pipelines.TryGetValue(contextType, out var cacheEntry))
            {
                return (PipelineDelegate<TContext>)cacheEntry.Delegate;
            }

            var pipeline = BuildPipeline<TContext>();

            _pipelines[contextType] = new CachedPipeline(pipeline);

            return pipeline;
        }

        private PipelineDelegate<TContext> BuildPipeline<TContext>() where TContext : class, IPipelineContext
        {
            PipelineDelegate<TContext> pipeline = _ => Task.CompletedTask;

            for (int i = _descriptors.Length - 1; i >= 0; i--)
            {
                if (_descriptors[i].TryGetFilter<TContext>(out var filter))
                {
                    var next = pipeline;
                    pipeline = context => filter.ExecuteAsync(context, next);
                }
            }

            return pipeline;
        }
    }
}