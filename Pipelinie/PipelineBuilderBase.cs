using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Pipelinie
{
    public abstract class PipelineBuilderBase<T> : IPipelineBuilder<T> where T : ICommand
    {
        private readonly IServiceProvider ServiceProvider;

        protected PipelineBuilderBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IPipeline CreatePipeline(T command)
        {
            var steps = StepsTypes.Select(s =>
            {
                return (IStep<T>)ServiceProvider.GetRequiredService(s);
            });

            var pipeline = new Pipeline<T>(command);
            pipeline.AddSteps(steps);
            return pipeline;
        }

        protected abstract Type[] StepsTypes { get; }
    }
}