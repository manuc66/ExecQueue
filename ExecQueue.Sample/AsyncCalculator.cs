using System.Threading.Tasks;

namespace ExecQueue.Sample
{
  class AsyncCalculator
  {
    private readonly IExecutionDispatcher _executionDispatcher;
    private readonly ICalculator _calculator;

    public AsyncCalculator(IExecutionDispatcher executionDispatcher, ICalculator calculator)
    {
      _executionDispatcher = executionDispatcher;
      _calculator = calculator;
    }

    public async void SetValue(int val)
    {
      await _executionDispatcher.InvokeAsync(() => _calculator.SetValue(val));
    }

    public async void Inc()
    {
      await _executionDispatcher.InvokeAsync(() => _calculator.Inc());
    }

    public async void Sub()
    {
      await _executionDispatcher.InvokeAsync(() => _calculator.Sub());
    }

    public async Task<int> Result()
    {
      return await _executionDispatcher.InvokeAsync(() => _calculator.Result());
    }
  }
}