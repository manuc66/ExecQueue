using System;
using System.Runtime.ExceptionServices;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace ExecQueue
{
  public class InvokeProxy<T> : RealProxy where T : class
  {
    protected readonly T Instance;
    protected readonly IExecutionQueue ExecutionQueue;

    protected InvokeProxy(T instance, IExecutionQueue executionQueue)
      : base(typeof(T))
    {
      Instance = instance;
      ExecutionQueue = executionQueue;
    }

    public static T Create(T instance, IExecutionQueue executionQueue)
    {
      var actorProxy = new InvokeProxy<T>(instance, executionQueue);
      return (T)actorProxy.GetTransparentProxy();
    }

    public override IMessage Invoke(IMessage msg)
    {
      return Invoke((IMethodCallMessage)msg);
    }

    private IMessage Invoke(IMethodCallMessage methodCall)
    {
      var args = methodCall.Args;

      var invokeAsync = ExecutionQueue.InvokeAsync(() => methodCall.MethodBase.Invoke(Instance, args));

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