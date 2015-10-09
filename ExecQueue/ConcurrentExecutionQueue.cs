using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExecQueue
{
  public class ConcurrentExecutionQueue : IExecutionQueue
  {
    private readonly Action<Exception> _handleActionException;
    private readonly BlockingCollection<Action> _toExecute = new BlockingCollection<Action>();
    private static readonly object Void = new object();

    public ConcurrentExecutionQueue(Action<Exception> handleActionException)
    {
      _handleActionException = handleActionException;
    }

    public void BeginInvoke(Action action)
    {
      _toExecute.Add(action);
    }

    public Task InvokeAsync(Action action)
    {
      return InvokeAsync(() => { action(); return Void; });
    }

    public Task<TResult> InvokeAsync<TResult>(Func<TResult> action)
    {
      var funcTaskWrapper = new FuncTaskWrapper<TResult>(action);
      _toExecute.Add(funcTaskWrapper.Action);
      return funcTaskWrapper.Task;
    }

    public void Run(CancellationToken cancellationToken)
    {
      try
      {
        var actions = _toExecute.GetConsumingEnumerable(cancellationToken);

        Execute(actions);
      }
      catch (OperationCanceledException) { }
      catch (ObjectDisposedException) { }
    }

    private void Execute(IEnumerable<Action> actions)
    {
      foreach (var action in actions)
      {
        try
        {
          action.Invoke();
        }
        catch (Exception exception)
        {
          _handleActionException(exception);
        }
      }
    }
  }
}