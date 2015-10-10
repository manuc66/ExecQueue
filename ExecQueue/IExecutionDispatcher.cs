using System;
using System.Threading.Tasks;

namespace ExecQueue
{
  public interface IExecutionDispatcher
  {
    void BeginInvoke(Action action);
    void Invoke(Action action);
    TResult Invoke<TResult>(Func<TResult> action);
    Task InvokeAsync(Action action);
    Task<TResult> InvokeAsync<TResult>(Func<TResult> action);
  }
}