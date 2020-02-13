using NUnit.Framework;
using System.Threading.Tasks;

namespace Pipelinie.Tests
{
    public class PipelineTests
    {
        [Test]
        public async Task ExecutePipeline_SingleStepInPipeline_CallsStep()
        {
            var command = new BuyItemCommand();
            var updateStockStep = new UpdateStockStep();
            var builder = new BuyItemPipelineBuilder(command, updateStockStep);
            var pipeline = builder.CreatePipeline();

            await pipeline.ExecuteAsync();

            Assert.IsTrue(updateStockStep.WasCalled);
        }

        [Test]
        public async Task ExecutePipeline_MultipleStepsInPipeline_CallsEveryStep()
        {
            var command = new BuyItemCommand();
            var updateStockStep = new UpdateStockStep();
            var chargeCreditCardStep = new ChargeCreditCard();
            var steps = new IStep<BuyItemCommand>[] { updateStockStep, chargeCreditCardStep };
            var builder = new BuyItemPipelineBuilder(command, steps);
            var pipeline = builder.CreatePipeline();

            await pipeline.ExecuteAsync();

            Assert.IsTrue(updateStockStep.WasCalled);
            Assert.IsTrue(chargeCreditCardStep.WasCalled);
        }
    }

    public class UpdateStockStep : IStep<BuyItemCommand>
    {
        public bool WasCalled;

        public Task ExecuteAsync(BuyItemCommand command)
        {
            WasCalled = true;
            return Task.CompletedTask;
        }
    }

    public class ChargeCreditCard : IStep<BuyItemCommand>
    {
        public bool WasCalled;

        public Task ExecuteAsync(BuyItemCommand command)
        {
            WasCalled = true;
            return Task.CompletedTask;
        }
    }

    internal class BuyItemPipelineBuilder : IPipelineBuilder
    {
        private readonly BuyItemCommand Command;
        private readonly IStep<BuyItemCommand>[] Steps;

        public BuyItemPipelineBuilder(BuyItemCommand command, params IStep<BuyItemCommand>[] steps)
        {
            Command = command;
            Steps = steps;
        }

        public IPipeline CreatePipeline()
        {
            return new BuyItemPipeline(Command, Steps);
        }
    }

    internal class BuyItemPipeline : IPipeline
    {
        private readonly BuyItemCommand Command;
        private readonly IStep<BuyItemCommand>[] Steps;

        public BuyItemPipeline(BuyItemCommand command, IStep<BuyItemCommand>[] steps)
        {
            Command = command;
            Steps = steps;
        }

        public async Task ExecuteAsync()
        {
            foreach (var step in Steps)
            {
                await step.ExecuteAsync(Command);
            }
        }
    }

    public class BuyItemCommand : ICommand
    {
        public BuyItemCommand()
        {
        }
    }
}