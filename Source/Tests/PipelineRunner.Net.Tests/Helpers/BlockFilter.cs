namespace PipelineRunner.Net.Tests.Helpers
{
    internal sealed class BlockFilter : IFilter<DefendContext>
    {
        public async Task ExecuteAsync(DefendContext context, PipelineDelegate<DefendContext> next)
        {
            if (context.IsBlocked)
            {
                context.Defense += 50;
            }

            await next(context);
        }
    }
}
