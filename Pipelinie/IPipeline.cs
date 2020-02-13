using System.Threading.Tasks;

namespace Pipelinie
{
    public interface IPipeline
    {
        Task ExecuteAsync();
    }
}