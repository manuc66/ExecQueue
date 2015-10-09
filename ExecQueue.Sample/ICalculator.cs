namespace ExecQueue.Sample
{
  public interface ICalculator
  {
    void SetValue(int val);
    void Inc();
    void Sub();
    int Result();
  }
}