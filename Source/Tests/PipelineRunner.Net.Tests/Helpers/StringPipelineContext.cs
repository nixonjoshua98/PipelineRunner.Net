namespace PipelineRunner.Net.Tests.Helpers
{
    internal sealed class StringPipelineContext : IPipelineContext
    {
        public string Value { get; set; } = string.Empty;
    }
}
