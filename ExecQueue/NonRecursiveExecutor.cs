using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExecQueue
{
  public class NonRecursiveExecutor : QueuedDelayedExecutor
  {
    private readonly Queue<Action> _toExecute = new Queue<Action>();
    private bool _running;

    public override void BeginInvoke(Action action)
    {
      base.BeginInvoke(action);
      ExecuteAll();
    }

    public override Task InvokeAsync(Action action)
    {
      var invokeAsync = base.InvokeAsync(action);
      ExecuteAll();
      return invokeAsync;
    }

    public override Task<TResult> InvokeAsync<TResult>(Func<TResult> action)
    {
      var invokeAsync = base.InvokeAsync(action);
      ExecuteAll();
      return invokeAsync;
    }
    private void EnqueueAndExecute(Action action)
    {
      _toExecute.Enqueue(action);
      ExecuteAll();
    }
    public override void ExecuteAll()
    {
      if (_running)
        return;

      _running = true;
      base.ExecuteAll();
      _running = false;
    }
  }
}