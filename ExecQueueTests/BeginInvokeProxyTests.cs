using ExecQueue;
using NUnit.Framework;

namespace ExecQueueTests
{
  [TestFixture]
  class BeginInvokeProxyTests
  {
    internal interface ISimpleCalls
    {
      void Tell();
      bool Ask();

      T Repeate<T>(T value);
    }
    class SimpleCalls : ISimpleCalls
    {
      public bool Called;
      public void Tell()
      {
        Called = true;
      }
      public bool Ask()
      {
        return Called = true;
      }

      public T Repeate<T>(T value)
      {
        return value;
      }
    }
    [Test]
    public void InterceptVoidMethods()
    {
      var simpleCalls = new SimpleCalls();
      var proxiedSimpleCalls = BeginInvokeProxy<ISimpleCalls>.Create(simpleCalls, new NonRecursiveExecutionQueue());

      proxiedSimpleCalls.Tell();

      Assert.IsTrue(simpleCalls.Called);
    }

    [Test]
    public void InterceptMethodsThatRetuns()
    {
      var simpleCalls = new SimpleCalls();
      var proxiedSimpleCalls = BeginInvokeProxy<ISimpleCalls>.Create(simpleCalls, new NonRecursiveExecutionQueue());

      Assert.IsTrue(proxiedSimpleCalls.Ask());

      Assert.IsTrue(simpleCalls.Called);
    }

    [Test]
    public void InterceptGenericMethods()
    {
      var simpleCalls = new SimpleCalls();
      var proxiedSimpleCalls = BeginInvokeProxy<ISimpleCalls>.Create(simpleCalls, new NonRecursiveExecutionQueue());

      Assert.AreEqual(5, proxiedSimpleCalls.Repeate(5));
    }
  }
}
