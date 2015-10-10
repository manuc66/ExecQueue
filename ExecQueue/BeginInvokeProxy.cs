using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace ExecQueue
{
  public class BeginInvokeProxy<T> : InvokeProxy<T> where T : class
  {
    protected BeginInvokeProxy(T instance, IExecutionDispatcher executionDispatcher) : base(instance, executionDispatcher) { }

    public new static T Create(T instance, IExecutionDispatcher executionDispatcher)
    {
      var actorProxy = new BeginInvokeProxy<T>(instance, executionDispatcher);
      return (T)actorProxy.GetTransparentProxy();
    }

    public override IMessage Invoke(IMessage msg)
    {
      var methodCallMessage = (IMethodCallMessage)msg;

      if (MethodHaveOutput(methodCallMessage))
        return base.Invoke(methodCallMessage);

      return BeginInvoke(methodCallMessage);
    }

    private IMessage BeginInvoke(IMethodCallMessage methodCall)
    {
      var parameters = methodCall.Args;
      ExecutionDispatcher.BeginInvoke(() =>
      {
        methodCall.MethodBase.Invoke(Instance, parameters);
      });
      return new ReturnMessage(null, null, 0, methodCall.LogicalCallContext, methodCall);
    }

    private static bool MethodHaveOutput(IMethodCallMessage methodCall)
    {
      return ((MethodInfo)methodCall.MethodBase).ReturnType != typeof(void) || methodCall.ArgCount != methodCall.InArgCount;
    }
  }
}