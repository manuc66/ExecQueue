namespace ExecQueue.Sample
{
  class CalculatorExecQueueDelegator : ICalculator
  {
    private readonly IExecutionQueue _serviceExecutor;
    private readonly ICalculator _calculator;

    public CalculatorExecQueueDelegator(IExecutionQueue serviceExecutor, ICalculator calculator)
    {
      _serviceExecutor = serviceExecutor;
      _calculator = calculator;
    }

    public void SetValue(int val)
    {
      _serviceExecutor.BeginInvoke(() => _calculator.SetValue(val));
    }

    public void Inc()
    {
      _serviceExecutor.BeginInvoke(() => _calculator.Inc());
    }

    public void Sub()
    {
      _serviceExecutor.BeginInvoke(() => _calculator.Sub());
    }

    public int Result()
    {
      return _serviceExecutor.InvokeAsync(() => _calculator.Result()).Result;
    }
  }
}