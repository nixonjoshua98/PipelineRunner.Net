using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipelineRunner.Net
{
    internal sealed partial class BuiltPipeline : IBuiltPipeline
    {
        private readonly FilterDescriptor[] _descriptors;
        private readonly ConcurrentDictionary<Type, CachedPipeline> _pipelines = new ConcurrentDictionary<Type, CachedPipeline>();

        public BuiltPipeline(List<FilterDescriptor> descriptors)
        {
            _descriptors = descriptors.ToArray(); // Copy
        }

        public async Task ExecuteAsync<TContext>(TContext context) where TContext : class, IPipelineContext
        {
            var pipeline = GetOrCreatePipeline(context);

            await pipeline(context);
        }

        private PipelineDelegate<TContext> GetOrCreatePipeline<TContext>(TContext context) where TContext : class, IPipelineContext
        {
            var contextType = context.GetType();

            var value = _pipelines.GetOrAdd(
                contextType,
                _ =>
                {
                    var pipeline = BuildPipeline(context);

                    return new CachedPipeline(pipeline);
                });

            return (PipelineDelegate<TContext>)value.Delegate;
        }

        private PipelineDelegate<TContext> BuildPipeline<TContext>(TContext context) where TContext : class, IPipelineContext
        {
            var contextType = context.GetType();

            PipelineDelegate<TContext> pipeline = _ => Task.CompletedTask;

            for (int i = _descriptors.Length - 1; i >= 0; i--)
            {
                if (_descriptors[i].TryGetFilter<TContext>(contextType, out var filter))
                {
                    var next = pipeline;
                    pipeline = context => filter.ExecuteAsync(context, next);
                }
            }

            return pipeline;
        }
    }
}