using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pipelinie
{
    public abstract class PipelineBuilderBase<T> : IPipelineBuilder where T : ICommand
    {
        private readonly IServiceProvider ServiceProvider;

        protected PipelineBuilderBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IPipeline CreatePipeline()
        {
            var steps = StepsTypes.Select(s =>
            {
                return (IStep<T>)ServiceProvider.GetRequiredService(s);
            });

            return Factory(steps);
        }

        protected abstract Type[] StepsTypes { get; }

        protected abstract IPipeline Factory(IEnumerable<IStep<T>> steps);
    }
}