using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pipelinie.Tests
{
    public class PipelineTestsPipelineBuilder
    {
        [Test]
        public async Task CreatePipeline_StepWithSingleDependency_ResolvesStepWithServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
                                    .AddTransient<ICustomService, CustomService>()
                                    .AddTransient<StepWithSingleDependency>()
                                    .BuildServiceProvider();

            var command = new BuyItemCommand();
            var builder = new BuyItemSingleStepPipelineBuilder(serviceProvider, command);
            var pipeline = builder.CreatePipeline();

            await pipeline.ExecuteAsync();
        }

        [Test]
        public async Task CreatePipeline_StepWithMultipleDependencies_ResolveStepWithServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
                                    .AddTransient<ICustomService, CustomService>()
                                    .AddTransient<ICustomServiceB, CustomServiceB>()
                                    .AddTransient<StepWithMultipleDependencies>()
                                    .BuildServiceProvider();

            var command = new BuyItemCommand();
            var builder = new BuyItemMultipleStepsPipelineBuilder(serviceProvider, command);
            var pipeline = builder.CreatePipeline();

            await pipeline.ExecuteAsync();
        }

        [Test]
        public void CreatePipeline_StepWithSingleDependencyNotRegistered_ThrowException()
        {
            var serviceProvider = new ServiceCollection()
                                    // CustomService dependency missing
                                    .AddTransient<StepWithSingleDependency>()
                                    .BuildServiceProvider();

            var command = new BuyItemCommand();
            var builder = new BuyItemSingleStepPipelineBuilder(serviceProvider, command);

            Assert.Throws<InvalidOperationException>(() => builder.CreatePipeline());
        }
    }

    internal class BuyItemSingleStepPipelineBuilder : PipelineBuilderBase<BuyItemCommand>
    {
        private readonly BuyItemCommand Command;
        private readonly IServiceProvider Provider;

        public BuyItemSingleStepPipelineBuilder(IServiceProvider provider, BuyItemCommand command)
            : base(provider)
        {
            Provider = provider;
            Command = command;
        }

        protected override Type[] StepsTypes
            => new[]
            {
                typeof(StepWithSingleDependency),
            };

        protected override IPipeline Factory(IEnumerable<IStep<BuyItemCommand>> steps)
        {
            return new BuyItemPipeline(Command, steps.ToArray());
        }
    }

    internal class BuyItemMultipleStepsPipelineBuilder : PipelineBuilderBase<BuyItemCommand>
    {
        private readonly BuyItemCommand Command;
        private readonly IServiceProvider Provider;

        public BuyItemMultipleStepsPipelineBuilder(IServiceProvider provider, BuyItemCommand command)
            : base(provider)
        {
            Provider = provider;
            Command = command;
        }

        protected override Type[] StepsTypes
            => new[]
            {
                typeof(StepWithSingleDependency),
                typeof(StepWithMultipleDependencies)
            };

        protected override IPipeline Factory(IEnumerable<IStep<BuyItemCommand>> steps)
        {
            return new BuyItemPipeline(Command, steps.ToArray());
        }
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