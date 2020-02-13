using System.Threading.Tasks;

namespace Pipelinie
{
    public interface IStep<T> where T : ICommand
    {
        Task ExecuteAsync(T command);
    }
}