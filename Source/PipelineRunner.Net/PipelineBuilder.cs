using System;
using System.Collections.Generic;

namespace PipelineRunner.Net
{
    public sealed class PipelineBuilder
    {
        private readonly List<FilterDescriptor> _descriptors = new List<FilterDescriptor>();

        public PipelineBuilder AddFilter<TFilter>(TFilter filter) where TFilter : IFilter
        {
            _descriptors.Add(new FilterDescriptor(filter));
            return this;
        }

        public PipelineBuilder AddFilter<TFilter>() where TFilter : IFilter, new()
        {
            _descriptors.Add(new FilterDescriptor(typeof(TFilter), _ => new TFilter()));
            return this;
        }

        public PipelineBuilder AddFilter<TFilter>(Func<Type, IFilter> factory) where TFilter : IFilter
        {
            _descriptors.Add(new FilterDescriptor(typeof(TFilter), factory));
            return this;
        }

        public IBuiltPipeline Build()
        {
            return new BuiltPipeline(_descriptors);
        }
    }
}