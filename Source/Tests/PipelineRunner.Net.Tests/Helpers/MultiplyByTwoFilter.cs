namespace PipelineRunner.Net.Tests.Helpers
{
    internal sealed class MultiplyByTwoFilter : IFilter<NumericPipelineContext>
    {
        public static MultiplyByTwoFilter Instance = new();

        public async Task ExecuteAsync(NumericPipelineContext context, PipelineDelegate<NumericPipelineContext> next)
        {
            context.Value *= 2;

            await next(context);
        }
    }
}
