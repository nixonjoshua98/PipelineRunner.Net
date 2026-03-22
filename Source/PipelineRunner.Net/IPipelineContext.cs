using System.Collections.Generic;

namespace PipelineRunner.Net
{
    public interface IPipelineContext
    {

    }
    
    public interface IRecursivePipelineContext : IPipelineContext
    {
        IEnumerable<IPipelineContext> Children { get; }

        void Enqueue(IPipelineContext context);
    }
}