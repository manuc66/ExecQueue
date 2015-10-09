using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace ExecQueue
{
  public class BeginInvokeProxy<T> : RealProxy
  {
    private readonly T _instance;
    private readonly IExecutionQueue _executionQueue;

    private BeginInvokeProxy(T instance, IExecutionQueue concurrentExecutionQueue)
      : base(typeof(T))
    {
      _instance = instance;
      _executionQueue = concurrentExecutionQueue;
    }

    public static T Create(T instance, IExecutionQueue concurrentExecutionQueue)
    {
      var actorProxy = new BeginInvokeProxy<T>(instance, concurrentExecutionQueue);
      return (T)actorProxy.GetTransparentProxy();
    }

    public override IMessage Invoke(IMessage msg)
    {
      var methodCall = (IMethodCallMessage)msg;
      var method = (MethodInfo)methodCall.MethodBase;

      IMessage message;
      if (MethodDoesNotHaveAnyOutput(method, methodCall))
      {
        message = BeginInvoke(methodCall, methodCall.Args, (MethodInfo)methodCall.MethodBase);
      }
      else
      {
        message = Invoke(methodCall, methodCall.Args, (MethodInfo)methodCall.MethodBase);
      }

      return message;
    }

    private static bool MethodDoesNotHaveAnyOutput(MethodInfo method, IMethodCallMessage methodCall)
    {
      return method.ReturnType == typeof(void) && methodCall.ArgCount == methodCall.InArgCount;
    }

    private IMessage BeginInvoke(IMethodCallMessage methodCall, object[] args, MethodInfo methodBase)
    {
      try
      {
        _executionQueue.BeginInvoke(() => methodBase.Invoke(_instance, args));
        return new ReturnMessage(null, null, 0, methodCall.LogicalCallContext, methodCall);
      }
      catch (Exception e)
      {
        if (e is TargetInvocationException && e.InnerException != null)
        {
          return new ReturnMessage(e.InnerException, methodCall);
        }

        return new ReturnMessage(e, methodCall);
      }
    }

    private IMessage Invoke(IMethodCallMessage methodCall, object[] args, MethodInfo methodBase)
    {
      var result = _executionQueue.InvokeAsync(() => methodBase.Invoke(_instance, args)).Result;

      return new ReturnMessage(result, args, args.Length, methodCall.LogicalCallContext, methodCall);
    }
  }
}