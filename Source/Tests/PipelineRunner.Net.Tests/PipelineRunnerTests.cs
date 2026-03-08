using PipelineRunner.Net.Tests.Helpers;

namespace PipelineRunner.Net.Tests
{
    public class PipelineRunnerTests
    {
        [Fact]
        public async Task FiltersShouldAllExecute()
        {
            var context = new NumericPipelineContext();

            var runner = new PipelineBuilder()
                .AddFilter(MultiplyByTwoFilter.Instance)
                .AddFilter(new MultiplyByTwoFilter())
                .AddFilter<MultiplyByTwoFilter>(_ => new MultiplyByTwoFilter())
                .Build();

            await runner.ExecuteAsync(context);

            Assert.Equal(8, context.Value);
        }

        [Fact]
        public async Task FiltersShouldExecute()
        {
            var numericContext = new NumericPipelineContext();

            IPipelineContext context = numericContext;

            var runner = new PipelineBuilder()
                .AddFilter(MultiplyByTwoFilter.Instance)
                .AddFilter(new MultiplyByTwoFilter())
                .AddFilter<MultiplyByTwoFilter>()
                .Build();

            await runner.ExecuteAsync(context);

            Assert.Equal(8, numericContext.Value);
        }

        [Fact]
        public async Task FiltersShouldExecuteInOrder()
        {
            var context = new NumericPipelineContext();

            var runner = new PipelineBuilder()
                .AddFilter<MultiplyByTwoFilter>()
                .AddFilter(new AddValueFilter(1))
                .Build();

            await runner.ExecuteAsync(context);

            Assert.Equal(3, context.Value);
        }

        [Fact]
        public async Task BuildShouldReturnReusablePipeline()
        {
            var pipeline = new PipelineBuilder()
                .AddFilter<MultiplyByTwoFilter>()
                .AddFilter(new AddValueFilter(1))
                .Build();

            var context1 = new NumericPipelineContext();
            await pipeline.ExecuteAsync(context1);
            Assert.Equal(3, context1.Value);

            var context2 = new NumericPipelineContext { Value = 5 };
            await pipeline.ExecuteAsync(context2);
            Assert.Equal(11, context2.Value);
        }

        [Fact]
        public async Task BuildShouldSupportMultipleContextTypes()
        {
            var pipeline = new PipelineBuilder()
                .AddFilter<MultiplyByTwoFilter>()
                .AddFilter(new AddValueFilter(1))
                .AddFilter<UpperCaseFilter>()
                .Build();

            var numericContext = new NumericPipelineContext { Value = 10 };
            await pipeline.ExecuteAsync(numericContext);
            Assert.Equal(21, numericContext.Value);

            var stringContext = new StringPipelineContext { Value = "hello" };
            await pipeline.ExecuteAsync(stringContext);
            Assert.Equal("HELLO", stringContext.Value);

            var anotherNumeric = new NumericPipelineContext { Value = 5 };
            await pipeline.ExecuteAsync(anotherNumeric);
            Assert.Equal(11, anotherNumeric.Value);
        }

        [Fact]
        public async Task GamePipelineExample()
        {
            var gamePipeline = new PipelineBuilder()
                .AddFilter<CriticalHitFilter>()
                .AddFilter<BlockFilter>()
                .Build();

            var attack = new AttackContext { Damage = 100, IsCritical = true };
            await gamePipeline.ExecuteAsync(attack);
            Assert.Equal(200, attack.Damage);

            var defend = new DefendContext { Defense = 50, IsBlocked = true };
            await gamePipeline.ExecuteAsync(defend);
            Assert.Equal(100, defend.Defense);

            var normalAttack = new AttackContext { Damage = 100, IsCritical = false };
            await gamePipeline.ExecuteAsync(normalAttack);
            Assert.Equal(100, normalAttack.Damage);
        }
    }
}