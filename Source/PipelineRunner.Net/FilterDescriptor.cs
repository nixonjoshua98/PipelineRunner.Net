using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PipelineRunner.Net
{
    internal sealed class FilterDescriptor
    {
        public readonly Type? FilterType;
        public readonly IFilter? FilterInstance;
        public readonly Func<Type, IFilter>? Factory;
        private readonly Dictionary<Type, bool> _compatibilityCache = new Dictionary<Type, bool>();

        public FilterDescriptor(Type filerType, Func<Type, IFilter> factory)
        {
            FilterInstance = null;
            FilterType = filerType;
            Factory = factory;
        }

        public FilterDescriptor(IFilter instance)
        {
            FilterInstance = instance;
            FilterType = instance.GetType();
            Factory = null;
        }

        public bool TryGetFilter<TContext>([NotNullWhen(true)] out IFilter<TContext>? filter) where TContext : class, IPipelineContext
        {
            if (FilterInstance is IFilter<TContext> typedFilter)
            {
                filter = typedFilter;
                return true;
            }

            if (Factory != null && FilterType != null && IsCompatible<TContext>())
            {
                var basicFilter = Factory(FilterType);

                if (basicFilter is IFilter<TContext> typed)
                {
                    filter = typed;
                    return true;
                }
            }

            filter = null;
            return false;
        }

        private bool IsCompatible<TContext>() where TContext : class, IPipelineContext
        {
            var contextType = typeof(TContext);

            if (!_compatibilityCache.TryGetValue(contextType, out var isCompatible))
            {
                var filterInterfaceType = typeof(IFilter<TContext>);
                isCompatible = filterInterfaceType.IsAssignableFrom(FilterType);
                _compatibilityCache[contextType] = isCompatible;
            }

            return isCompatible;
        }
    }
}
