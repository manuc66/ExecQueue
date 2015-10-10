namespace ExecQueue.Sample
{
  class CalculatorExecQueueDelegator : ICalculator
  {
    private readonly IExecutionDispatcher _executionDispatcher;
    private readonly ICalculator _calculator;

    public CalculatorExecQueueDelegator(IExecutionDispatcher executionDispatcher, ICalculator calculator)
    {
      _executionDispatcher = executionDispatcher;
      _calculator = calculator;
    }

    public void SetValue(int val)
    {
      _executionDispatcher.BeginInvoke(() => _calculator.SetValue(val));
    }

    public void Inc()
    {
      _executionDispatcher.BeginInvoke(() => _calculator.Inc());
    }

    public void Sub()
    {
      _executionDispatcher.BeginInvoke(() => _calculator.Sub());
    }

    public int Result()
    {
      return _executionDispatcher.InvokeAsync(() => _calculator.Result()).Result;
    }
  }
}