using System;
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
      void AssignWithFirstVal<T>(out T assigned, ref bool invertMe, T input);
      void ThowOnVoid();
      bool ThowOnFunc();
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
        Called = true;
        return value;
      }

      public void Assign5(out int willBe5)
      {
        Called = true;
        willBe5 = 5;
      }

      public void AssignWithFirstVal<T>(out T assigned, ref bool invertMe, T input)
      {
        Called = true;
        assigned = input;
        invertMe = !invertMe;
        input = default(T);
      }

      public void ThowOnVoid()
      {
        Called = true;
        throw new System.NotImplementedException();
      }

      public bool ThowOnFunc()
      {
        Called = true;
        throw new System.NotImplementedException();
      }
    }
    [Test]
    public void InterceptVoidMethods()
    {
      var simpleCalls = new SimpleCalls();
      var proxiedSimpleCalls = BeginInvokeProxy<ISimpleCalls>.Create(simpleCalls, new NonRecursiveExecutor());

      proxiedSimpleCalls.Tell();

      Assert.IsTrue(simpleCalls.Called);
    }

    [Test]
    public void InterceptMethodsThatRetuns()
    {
      var simpleCalls = new SimpleCalls();
      var proxiedSimpleCalls = BeginInvokeProxy<ISimpleCalls>.Create(simpleCalls, new NonRecursiveExecutor());

      Assert.IsTrue(proxiedSimpleCalls.Ask());
      Assert.IsTrue(simpleCalls.Called);
    }

    [Test]
    public void InterceptGenericMethods()
    {
      var simpleCalls = new SimpleCalls();
      var proxiedSimpleCalls = BeginInvokeProxy<ISimpleCalls>.Create(simpleCalls, new NonRecursiveExecutor());

      Assert.AreEqual(5, proxiedSimpleCalls.Repeate(5));
    }

    [Test]
    public void OutParameterAreHandled()
    {
      var simpleCalls = new SimpleCalls();
      var proxiedSimpleCalls = BeginInvokeProxy<ISimpleCalls>.Create(simpleCalls, new NonRecursiveExecutor());

      int expect5;
      proxiedSimpleCalls.Assign5(out expect5);

      Assert.AreEqual(5, expect5);
    }

    [Test]
    public void OutParameterMixedWithOtherInGenericMethodAreHandled()
    {
      var simpleCalls = new SimpleCalls();
      var proxiedSimpleCalls = BeginInvokeProxy<ISimpleCalls>.Create(simpleCalls, new NonRecursiveExecutor());

      string assigned;
      bool toInvert = false;
      var firstVal = "55";
      proxiedSimpleCalls.AssignWithFirstVal(out assigned, ref toInvert, firstVal);

      Assert.AreEqual("55", assigned);
      Assert.AreEqual("55", firstVal);
      Assert.IsTrue(toInvert);
    }

    [Test]
    public void ExceptionOnVoidIsIgnored()
    {
      var simpleCalls = new SimpleCalls();
      var executionQueue = new QueuedDelayedExecutor();
      var proxiedSimpleCalls = BeginInvokeProxy<ISimpleCalls>.Create(simpleCalls, executionQueue);

      proxiedSimpleCalls.ThowOnVoid();

      try
      {
        executionQueue.ExecuteAll();
        Assert.Fail();
      }
      catch (Exception e)
      {
        Assert.IsNotNull(e);
      }

      Assert.IsTrue(simpleCalls.Called);
    }

    [Test]
    public void ExceptionOnNonVoidIsIgnored()
    {
      var simpleCalls = new SimpleCalls();
      var proxiedSimpleCalls = BeginInvokeProxy<ISimpleCalls>.Create(simpleCalls, new NonRecursiveExecutor());

      try
      {
        proxiedSimpleCalls.ThowOnFunc();
      }
      catch (AggregateException e)
      {
        Assert.IsNotNull(e);
        Assert.IsInstanceOf<NotImplementedException>(e.InnerExceptions[0]);
      }
      catch (Exception)
      {
        Assert.Fail();
      }

      Assert.IsTrue(simpleCalls.Called);
    }
  }
}
