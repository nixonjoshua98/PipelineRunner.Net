using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PipelineRunner.Net
{
    internal sealed class FilterWrapper<TContext, TActual> : IFilter<TContext>
        where TContext : class, IPipelineContext
        where TActual : class, TContext
    {
        private readonly IFilter<TActual> _inner;

        public FilterWrapper(IFilter<TActual> inner) => _inner = inner;

        public Task ExecuteAsync(TContext context, PipelineDelegate<TContext> next)
        {
            var typedContext = Unsafe.As<TActual>(context);
            return _inner.ExecuteAsync(typedContext, WrapNext(next));
        }

        private static PipelineDelegate<TActual> WrapNext(PipelineDelegate<TContext> next)
        {
            return actualContext => next(actualContext);
        }
    }
}
