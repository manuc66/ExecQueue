using System.Threading.Tasks;

namespace ExecQueue.Sample
{
  class AsyncCalculator
  {
    private readonly IExecutionQueue _serviceExecutor;
    private readonly ICalculator _calculator;

    public AsyncCalculator(IExecutionQueue serviceExecutor, ICalculator calculator)
    {
      _serviceExecutor = serviceExecutor;
      _calculator = calculator;
    }

    public async void SetValue(int val)
    {
      await _serviceExecutor.InvokeAsync(() => _calculator.SetValue(val));
    }

    public async void Inc()
    {
      await _serviceExecutor.InvokeAsync(() => _calculator.Inc());
    }

    public async void Sub()
    {
      await _serviceExecutor.InvokeAsync(() => _calculator.Sub());
    }

    public async Task<int> Result()
    {
      return await _serviceExecutor.InvokeAsync(() => _calculator.Result());
    }
  }
}