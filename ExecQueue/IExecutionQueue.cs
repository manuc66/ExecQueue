using System;
using System.Threading.Tasks;

namespace ExecQueue
{
  public interface IExecutionQueue
  {
    void BeginInvoke(Action action);
    Task InvokeAsync(Action action);
    Task<TResult> InvokeAsync<TResult>(Func<TResult> action);
  }
}