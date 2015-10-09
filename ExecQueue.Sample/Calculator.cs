namespace ExecQueue.Sample
{
  class Calculator : ICalculator
  {
    private int _val;
    public void SetValue(int val)
    {
      _val = val;
    }

    public void Inc()
    {
      _val++;
    }

    public void Sub()
    {
      _val--;
    }

    public int Result()
    {
      return _val;
    }
  }
}