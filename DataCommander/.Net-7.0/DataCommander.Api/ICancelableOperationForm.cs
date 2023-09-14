using System;
using System.Threading.Tasks;

namespace DataCommander.Api;

public interface ICancelableOperationForm
{
    void Start(Task task, TimeSpan showDialogDelay);
}