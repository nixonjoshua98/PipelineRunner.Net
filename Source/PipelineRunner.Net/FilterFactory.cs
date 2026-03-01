using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PipelineRunner.Net
{

    internal static class FilterFactory<TContext> where TContext : class, IPipelineContext
    {
        private static readonly Dictionary<Type, Func<IFilter, IFilter<TContext>>> _cache = new Dictionary<Type, Func<IFilter, IFilter<TContext>>>();

        public static IFilter<TContext> CreateWrapper(IFilter instance, Type actualContextType)
        {
            if (_cache.TryGetValue(actualContextType, out var factory))
            {
                return factory(instance);
            }

            factory = CreateFactory(actualContextType);

            _cache[actualContextType] = factory;

            return factory(instance);
        }

        private static Func<IFilter, IFilter<TContext>> CreateFactory(Type actualContextType)
        {
            var wrapperType = typeof(FilterWrapper<,>).MakeGenericType(typeof(TContext), actualContextType);
            var parameter = Expression.Parameter(typeof(IFilter), "filter");
            var filterType = typeof(IFilter<>).MakeGenericType(actualContextType);
            var castedParam = Expression.Convert(parameter, filterType);
            var constructor = wrapperType.GetConstructors()[0];
            var newExpression = Expression.New(constructor, castedParam);

            return Expression.Lambda<Func<IFilter, IFilter<TContext>>>(newExpression, parameter).Compile();
        }
    }
}
