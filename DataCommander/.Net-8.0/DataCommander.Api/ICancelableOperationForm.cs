using System.Threading.Tasks;

namespace DataCommander.Api;

public interface ICancelableOperationForm
{
    void Execute(Task cancelableOperation);
    T Execute<T>(Task<T> cancelableOperation);
}