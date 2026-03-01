using System;
using System.Diagnostics.CodeAnalysis;

namespace PipelineRunner.Net
{
    internal sealed class FilterDescriptor
    {
        public readonly Type? FilterType;
        public readonly IFilter? FilterInstance;
        public readonly Func<Type, IFilter>? Factory;

        public FilterDescriptor(Type filerType, Func<Type, IFilter> factory)
        {
            FilterInstance = null;
            FilterType = filerType;
            Factory = factory;
        }

        public FilterDescriptor(IFilter instance)
        {
            FilterInstance = instance;
            FilterType = null;
            Factory = null;
        }

        public bool TryGetFilter<TContext>([NotNullWhen(true)] out IFilter<TContext>? filter) where TContext : class, IPipelineContext
        {
            if (FilterInstance is IFilter<TContext> typedFilter)
            {
                filter = typedFilter;

                return true;
            }

            if (Factory != null && FilterType != null)
            {
                var filterInterfaceType = typeof(IFilter<TContext>);

                if (filterInterfaceType.IsAssignableFrom(FilterType))
                {
                    var basicFilter = Factory(FilterType);

                    if (basicFilter is IFilter<TContext> typed)
                    {
                        filter = typed;

                        return true;
                    }
                }
            }

            filter = null;

            return false;
        }
    }
}
