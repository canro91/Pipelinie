using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pipelinie
{
    public class Pipeline<T> : IPipeline where T : ICommand
    {
        private readonly List<IStep<T>> PipelineSteps = new List<IStep<T>>();
        private readonly T Command;

        public Pipeline(T command)
        {
            Command = command;
        }

        public void AddSteps(IEnumerable<IStep<T>> steps)
        {
            PipelineSteps.AddRange(steps);
        }

        public async Task ExecuteAsync()
        {
            foreach (var step in PipelineSteps)
            {
                await step.ExecuteAsync(Command);
            }
        }
    }
}