using System.Threading.Tasks;

namespace PipelineRunner.Net.Tests.Helpers
{
    internal sealed class CriticalHitFilter : IFilter<AttackContext>
    {
        public async Task ExecuteAsync(AttackContext context, PipelineDelegate<AttackContext> next)
        {
            if (context.IsCritical)
            {
                context.Damage *= 2;
            }

            await next(context);
        }
    }
}
