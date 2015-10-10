using System;
using System.Threading;

namespace ExecQueue.Sample
{

  class Program
  {
    static void Main(string[] args)
    {
      var actionQueue = new ConcurrentExecutionDispatcher(e => { Console.Error.WriteLine(e.ToString()); });
      var calculator = BeginInvokeProxy<ICalculator>.Create(new Calculator(), actionQueue);

      var cancellationTokenSource = ExecuteActionsAsync(actionQueue);

      calculator.SetValue(5);
      for (int i = 0; i < 100; i++)
      {
        calculator.Inc();
        calculator.Sub();
      }
      calculator.Inc();
      calculator.Sub();


      Console.WriteLine(calculator.Result());

      cancellationTokenSource.Cancel();


      Console.ReadKey();
    }

    public static CancellationTokenSource ExecuteActionsAsync(ConcurrentExecutionDispatcher actions)
    {
      var cancellationTokenSource = new CancellationTokenSource();

      ThreadPool.QueueUserWorkItem(x =>
      {
        actions.Run(cancellationTokenSource.Token);
      });
      return cancellationTokenSource;
    }
  }
}
