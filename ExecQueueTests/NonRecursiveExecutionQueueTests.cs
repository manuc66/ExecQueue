using ExecQueue;
using NUnit.Framework;

namespace ExecQueueTests
{
  [TestFixture]
  public class NonRecursiveExecutionQueueTests
  {
    [Test]
    public void MethodAreCalled()
    {
      int i = 0;
      new NonRecursiveExecutionQueue().BeginInvoke(() => { i = 5; });
      Assert.That(i == 5);
    }
  }
}
