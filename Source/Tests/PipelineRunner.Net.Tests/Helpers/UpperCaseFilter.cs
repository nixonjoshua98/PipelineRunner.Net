namespace PipelineRunner.Net.Tests.Helpers
{
    internal sealed class UpperCaseFilter : IFilter<StringPipelineContext>
    {
        public static UpperCaseFilter Instance = new();

        public async Task ExecuteAsync(StringPipelineContext context, PipelineDelegate<StringPipelineContext> next)
        {
            context.Value = context.Value.ToUpperInvariant();

            await next(context);
        }
    }
}
