using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExecQueue
{
  public class QueuedExecutionDispatcher : IExecutionDispatcher
  {
    private readonly Queue<Action> _toExecute = new Queue<Action>();
    private bool _running;
    private static readonly object Void = new object();

    public virtual void BeginInvoke(Action action)
    {
      _toExecute.Enqueue(action);
    }

    public virtual void Invoke(Action action)
    {
      InvokeAsync(action).Wait();
    }

    public virtual TResult Invoke<TResult>(Func<TResult> action)
    {
      return InvokeAsync(action).Result;
    }

    public virtual Task InvokeAsync(Action action)
    {
      return InvokeAsync(() => { action(); return Void; });
    }

    public virtual Task<TResult> InvokeAsync<TResult>(Func<TResult> action)
    {
      var funcTaskWrapper = new FuncTaskWrapper<TResult>(action);
      _toExecute.Enqueue(funcTaskWrapper.Action);
      return funcTaskWrapper.Task;
    }

    public virtual void ExecuteAll()
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