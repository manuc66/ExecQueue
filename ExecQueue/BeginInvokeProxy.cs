using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace ExecQueue
{
  public class BeginInvokeProxy<T> : RealProxy where T : class
  {
    private readonly T _instance;
    private readonly IExecutionQueue _executionQueue;

    private BeginInvokeProxy(T instance, IExecutionQueue executionQueue)
      : base(typeof(T))
    {
      _instance = instance;
      _executionQueue = executionQueue;
    }

    public static T Create(T instance, IExecutionQueue executionQueue)
    {
      var actorProxy = new BeginInvokeProxy<T>(instance, executionQueue);
      return (T)actorProxy.GetTransparentProxy();
    }

    public override IMessage Invoke(IMessage msg)
    {
      var methodCall = (IMethodCallMessage)msg;

      IMessage message;
      if (MethodDoesNotHaveAnyOutput(methodCall))
      {
        message = BeginInvoke(methodCall);
      }
      else
      {
        message = Invoke(methodCall);
      }

      return message;
    }

    private static bool MethodDoesNotHaveAnyOutput(IMethodCallMessage methodCall)
    {
      return ((MethodInfo)methodCall.MethodBase).ReturnType == typeof(void) && methodCall.ArgCount == methodCall.InArgCount;
    }

    private IMessage BeginInvoke(IMethodCallMessage methodCall)
    {
      var parameters = methodCall.Args;
      _executionQueue.BeginInvoke(() =>
      {
        methodCall.MethodBase.Invoke(_instance, parameters);
      });
      return new ReturnMessage(null, null, 0, methodCall.LogicalCallContext, methodCall);
    }

    private IMessage Invoke(IMethodCallMessage methodCall)
    {
      var args = methodCall.Args;

      var result = _executionQueue.InvokeAsync(() => methodCall.MethodBase.Invoke(_instance, args)).Result;

      return new ReturnMessage(result, args, args.Length, methodCall.LogicalCallContext, methodCall);
    }
  }
}