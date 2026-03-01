namespace PipelineRunner.Net.Tests.Helpers
{
    internal sealed class AttackContext : IPipelineContext
    {
        public int Damage { get; set; }
        public bool IsCritical { get; set; }
    }
}
