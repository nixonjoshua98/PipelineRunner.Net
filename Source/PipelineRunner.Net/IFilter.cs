using System.Threading.Tasks;

namespace PipelineRunner.Net
{
    public delegate Task PipelineDelegate<TContext>(TContext context) where TContext : class, IPipelineContext;

    public interface IFilter
    {

    }

    public interface IFilter<TContext> : IFilter where TContext : class, IPipelineContext
    {
        Task ExecuteAsync(TContext context, PipelineDelegate<TContext> next);
    }
}
