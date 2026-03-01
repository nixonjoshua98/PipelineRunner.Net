namespace PipelineRunner.Net.Tests.Helpers
{
    internal sealed class AddValueFilter : IFilter<NumericPipelineContext>
    {
        private readonly int Value;

        public AddValueFilter(int value)
        {
            Value = value;
        }

        public async Task ExecuteAsync(NumericPipelineContext context, PipelineDelegate<NumericPipelineContext> next)
        {
            context.Value += Value;

            await next(context);
        }
    }
}
