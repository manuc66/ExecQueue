using System;
using System.Runtime.ExceptionServices;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace ExecQueue
{
  public class InvokeProxy<T> : RealProxy where T : class
  {
    protected readonly T Instance;
    protected readonly IExecutionDispatcher ExecutionDispatcher;

    protected InvokeProxy(T instance, IExecutionDispatcher executionDispatcher)
      : base(typeof(T))
    {
      Instance = instance;
      ExecutionDispatcher = executionDispatcher;
    }

    public static T Create(T instance, IExecutionDispatcher executionDispatcher)
    {
      var actorProxy = new InvokeProxy<T>(instance, executionDispatcher);
      return (T)actorProxy.GetTransparentProxy();
    }

    public override IMessage Invoke(IMessage msg)
    {
      return Invoke((IMethodCallMessage)msg);
    }

    private IMessage Invoke(IMethodCallMessage methodCall)
    {
      var args = methodCall.Args;

      var invokeAsync = ExecutionDispatcher.InvokeAsync(() => methodCall.MethodBase.Invoke(Instance, args));

      try
      {
        return new ReturnMessage(invokeAsync.Result, args, args.Length, methodCall.LogicalCallContext, methodCall);
      }
      catch (Exception ex)
      {
        ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
        throw;
      }
    }

  }
}