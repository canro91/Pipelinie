using NUnit.Framework;
using System.Threading.Tasks;

namespace Pipelinie.Tests
{
    public class PipelineTestDefaultPipeline
    {
        [Test]
        public async Task ExecutePipeline_SingleStepInPipelineAndBuilderCreatesDefaultPipeline_CallsStep()
        {
            var command = new BuyItemCommand();
            var updateStockStep = new UpdateStockStep();
            var builder = new BuyItemDefaultPipelineBuilder(updateStockStep);
            var pipeline = builder.CreatePipeline(command);

            await pipeline.ExecuteAsync();

            Assert.IsTrue(updateStockStep.WasCalled);
        }
    }

    internal class BuyItemDefaultPipelineBuilder : IPipelineBuilder<BuyItemCommand>
    {
        private readonly UpdateStockStep UpdateStockStep;

        public BuyItemDefaultPipelineBuilder(UpdateStockStep updateStockStep)
        {
            UpdateStockStep = updateStockStep;
        }

        public IPipeline CreatePipeline(BuyItemCommand command)
        {
            var pipeline = new Pipeline<BuyItemCommand>(command);
            pipeline.AddSteps(new[] { UpdateStockStep });
            return pipeline;
        }
    }
}