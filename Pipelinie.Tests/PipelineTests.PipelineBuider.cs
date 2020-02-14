using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Pipelinie.Tests
{
    public class PipelineTestsPipelineBuilder
    {
        [Test]
        public void CreatePipeline_StepWithSingleDependency_ResolvesStepWithServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
                                    .AddTransient<ICustomService, CustomService>()
                                    .AddTransient<StepWithSingleDependency>()
                                    .BuildServiceProvider();

            var command = new BuyItemCommand();
            var builder = new BuyItemSingleStepPipelineBuilder(serviceProvider);
            var pipeline = builder.CreatePipeline(command);

            Assert.IsNotNull(pipeline);
        }

        [Test]
        public void CreatePipeline_StepWithMultipleDependencies_ResolveStepWithServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
                                    .AddTransient<ICustomService, CustomService>()
                                    .AddTransient<ICustomServiceB, CustomServiceB>()
                                    .AddTransient<StepWithSingleDependency>()
                                    .AddTransient<StepWithMultipleDependencies>()
                                    .BuildServiceProvider();

            var command = new BuyItemCommand();
            var builder = new BuyItemMultipleStepsPipelineBuilder(serviceProvider);
            var pipeline = builder.CreatePipeline(command);

            Assert.IsNotNull(pipeline);
        }

        [Test]
        public void CreatePipeline_StepWithSingleDependencyNotRegistered_ThrowException()
        {
            var serviceProvider = new ServiceCollection()
                                    // CustomService dependency missing
                                    .AddTransient<StepWithSingleDependency>()
                                    .BuildServiceProvider();

            var command = new BuyItemCommand();
            var builder = new BuyItemSingleStepPipelineBuilder(serviceProvider);

            Assert.Throws<InvalidOperationException>(() => builder.CreatePipeline(command));
        }
    }

    internal class BuyItemSingleStepPipelineBuilder : PipelineBuilderBase<BuyItemCommand>
    {
        public BuyItemSingleStepPipelineBuilder(IServiceProvider provider)
            : base(provider)
        {
        }

        protected override Type[] StepsTypes
            => new[]
            {
                typeof(StepWithSingleDependency),
            };
    }

    internal class BuyItemMultipleStepsPipelineBuilder : PipelineBuilderBase<BuyItemCommand>
    {
        public BuyItemMultipleStepsPipelineBuilder(IServiceProvider provider)
            : base(provider)
        {
        }

        protected override Type[] StepsTypes
            => new[]
            {
                typeof(StepWithSingleDependency),
                typeof(StepWithMultipleDependencies)
            };
    }

    internal class StepWithSingleDependency : IStep<BuyItemCommand>
    {
        private readonly ICustomService CustomService;

        public StepWithSingleDependency(ICustomService customService)
        {
            CustomService = customService;
        }

        public Task ExecuteAsync(BuyItemCommand command)
        {
            return Task.CompletedTask;
        }
    }

    internal class StepWithMultipleDependencies : IStep<BuyItemCommand>
    {
        private readonly ICustomService CustomService;
        private readonly ICustomServiceB CustomServiceB;

        public StepWithMultipleDependencies(ICustomService customService, ICustomServiceB customServiceB)
        {
            CustomService = customService;
            CustomServiceB = customServiceB;
        }

        public Task ExecuteAsync(BuyItemCommand command)
        {
            return Task.CompletedTask;
        }
    }

    internal class CustomServiceB : ICustomServiceB
    {
    }

    internal class CustomService : ICustomService
    {
    }

    internal interface ICustomService
    {
    }

    internal interface ICustomServiceB
    {
    }
}