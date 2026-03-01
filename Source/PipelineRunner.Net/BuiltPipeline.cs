using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineRunner.Net
{
    public sealed partial class PipelineBuilder
    {
        private sealed class BuiltPipeline : IBuiltPipeline
        {
            private readonly List<FilterDescriptor> _descriptors;
            private readonly Dictionary<Type, object> _pipelines = new Dictionary<Type, object>();

            public BuiltPipeline(List<FilterDescriptor> descriptors)
            {
                _descriptors = descriptors;
            }

            public Task ExecuteAsync<TContext>(TContext context) where TContext : class, IPipelineContext
            {
                var contextType = typeof(TContext);

                if (!_pipelines.TryGetValue(contextType, out var cachedPipeline))
                {
                    cachedPipeline = BuildPipeline<TContext>();
                    _pipelines[contextType] = cachedPipeline;
                }

                var pipeline = (PipelineDelegate<TContext>)cachedPipeline;

                return pipeline(context);
            }

            private PipelineDelegate<TContext> BuildPipeline<TContext>() where TContext : class, IPipelineContext
            {
                PipelineDelegate<TContext> pipeline = _ => Task.CompletedTask;

                foreach (var descriptor in _descriptors.AsEnumerable().Reverse())
                {
                    if (descriptor.TryGetFilter<TContext>(out var filter))
                    {
                        var next = pipeline;
                        pipeline = context => filter.ExecuteAsync(context, next);
                    }
                }

                return pipeline;
            }
        }
    }
}