using System;
using System.Reflection;
using System.Threading.Tasks;

namespace ExecQueue
{
  class FuncTaskWrapper<TResult>
  {
    public Action Action { get; }
    public Task<TResult> Task { get; }

    public FuncTaskWrapper(Func<TResult> func)
    {
      var taskCompletionSource = new TaskCompletionSource<TResult>();
      Action = () =>
      {
        try
        {
          var result = func();
          taskCompletionSource.SetResult(result);
        }
        catch (TargetInvocationException exception)
        {
          taskCompletionSource.SetException(exception.InnerException);
        }
      };
      Task = taskCompletionSource.Task;
    }
  }
}