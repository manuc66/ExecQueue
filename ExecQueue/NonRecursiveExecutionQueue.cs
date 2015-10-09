using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExecQueue
{
  public class NonRecursiveExecutionQueue : IExecutionQueue
  {
    private readonly Queue<Action> _toExecute = new Queue<Action>();
    private bool _running;
    private static readonly object Void = new object();

    public void BeginInvoke(Action action)
    {
      EnqueueAndExecute(action);
    }

    public Task InvokeAsync(Action action)
    {
      return InvokeAsync(() => { action(); return Void; });
    }

    public Task<TResult> InvokeAsync<TResult>(Func<TResult> action)
    {
      var taskCompletionSource = new TaskCompletionSource<TResult>();
      EnqueueAndExecute(() =>
      {
        var result = action();
        taskCompletionSource.SetResult(result);
      });
      return taskCompletionSource.Task;
    }
    private void EnqueueAndExecute(Action action)
    {
      _toExecute.Enqueue(action);
      ExecuteAll();
    }
    private void ExecuteAll()
    {
      if (_running)
        return;

      _running = true;
      foreach (var action in _toExecute)
      {
        action();
      }
      _running = false;
    }
  }
}