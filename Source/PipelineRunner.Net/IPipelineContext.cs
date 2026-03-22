using System.Collections.Generic;

namespace PipelineRunner.Net
{
    public interface IPipelineContext
    {

    }
    
    public interface IRecursivePipelineContext : IPipelineContext
    {
        IEnumerable<IPipelineContext> Children { get; }
        
        void EnqueueContext<TContext>(TContext context) where TContext : IPipelineContext;
    }
}