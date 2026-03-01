using System.Threading.Tasks;

namespace PipelineRunner.Net
{
    public interface IBuiltPipeline
    {
        Task ExecuteAsync<TContext>(TContext context) where TContext : class, IPipelineContext;
    }
}
