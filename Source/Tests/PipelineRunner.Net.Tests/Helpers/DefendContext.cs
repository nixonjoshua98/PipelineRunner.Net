namespace PipelineRunner.Net.Tests.Helpers
{
    internal sealed class DefendContext : IPipelineContext
    {
        public int Defense { get; set; }
        public bool IsBlocked { get; set; }
    }
}
