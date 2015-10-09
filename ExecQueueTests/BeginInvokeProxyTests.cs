using ExecQueue;
using NUnit.Framework;

namespace ExecQueueTests
{
  [TestFixture]
  public class BeginInvokeProxyTests
  {
    internal interface ISimpleCalls
    {
      void Tell();
      bool Ask();

      T Repeate<T>(T value);

      void Assign5(out int willBe5);
      void AssignWithFirstVal<T>(T firstVal, out T assigned, ref bool invertMe);
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

      public void Assign5(out int willBe5)
      {
        willBe5 = 5;
      }

      public void AssignWithFirstVal<T>(T firstVal, out T assigned, ref bool invertMe)
      {
        assigned = firstVal;
        invertMe = !invertMe;
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

    [Test]
    public void OutParameterAreHandled()
    {
      var simpleCalls = new SimpleCalls();
      var proxiedSimpleCalls = BeginInvokeProxy<ISimpleCalls>.Create(simpleCalls, new NonRecursiveExecutionQueue());

      int expect5;
      proxiedSimpleCalls.Assign5(out expect5);

      Assert.AreEqual(5, expect5);
    }

    [Test]
    public void OutParameterMixedWithOtherInGenericMethodAreHandled()
    {
      var simpleCalls = new SimpleCalls();
      var proxiedSimpleCalls = BeginInvokeProxy<ISimpleCalls>.Create(simpleCalls, new NonRecursiveExecutionQueue());

      string assigned;
      bool toInvert = false;
      proxiedSimpleCalls.AssignWithFirstVal("55", out assigned, ref toInvert);

      Assert.AreEqual("55", assigned);
      Assert.IsTrue(toInvert);
    }
  }
}
