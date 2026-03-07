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

        public bool TryGetFilter<TContext>(Type runtimeContextType, [NotNullWhen(true)] out IFilter<TContext>? filter)
            where TContext : class, IPipelineContext
        {
            if (FilterInstance is IFilter<TContext> typedFilter)
            {
                filter = typedFilter;
                return true;
            }

            if (!IsCompatible(runtimeContextType))
            {
                filter = null;
                return false;
            }

            IFilter? instance = null;

            if (FilterInstance != null)
            {
                instance = FilterInstance;
            }

            else if (Factory != null && FilterType != null)
            {
                instance = Factory(FilterType);
            }

            if (instance is IFilter<TContext> typedInstance)
            {
                filter = typedInstance;
                return true;
            }

            else if (instance != null)
            {
                filter = FilterFactory<TContext>.CreateWrapper(instance, runtimeContextType);
                return true;
            }

            filter = null;
            return false;
        }

        private bool IsCompatible(Type contextType)
        {
            if (!_compatibilityCache.TryGetValue(contextType, out var isCompatible))
            {
                var filterInterfaceType = typeof(IFilter<>).MakeGenericType(contextType);
                isCompatible = filterInterfaceType.IsAssignableFrom(FilterType);
                _compatibilityCache[contextType] = isCompatible;
            }

            return isCompatible;
        }
    }
}
