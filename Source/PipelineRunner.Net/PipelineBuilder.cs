using System;
using System.Collections.Generic;
using System.Linq;

namespace PipelineRunner.Net
{
    public sealed class PipelineBuilder
    {
        private readonly List<FilterDescriptor> _descriptors = new List<FilterDescriptor>();

        public void Clear()
        {
            _descriptors.Clear();
        }

        public PipelineBuilder AddFilter<TFilter>(TFilter filter) where TFilter : IFilter
        {
            var descriptor = new FilterDescriptor(filter);

            _descriptors.Add(descriptor);

            return this;
        }

        public PipelineBuilder AddFilter<TFilter>() where TFilter : IFilter, new()
        {
            var descriptor = new FilterDescriptor(typeof(TFilter), _ => new TFilter());

            _descriptors.Add(descriptor);

            return this;
        }

        public PipelineBuilder AddFilter<TFilter>(Func<Type, IFilter> factory) where TFilter : IFilter, new()
        {
            var descriptor = new FilterDescriptor(typeof(TFilter), factory);

            _descriptors.Add(descriptor);

            return this;
        }

        public IBuiltPipeline Build()
        {
            return new BuiltPipeline(_descriptors.ToList());
        }
    }
}