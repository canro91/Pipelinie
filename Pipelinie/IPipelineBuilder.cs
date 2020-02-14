namespace Pipelinie
{
    public interface IPipelineBuilder<T> where T : ICommand
    {
        IPipeline CreatePipeline(T command);
    }
}