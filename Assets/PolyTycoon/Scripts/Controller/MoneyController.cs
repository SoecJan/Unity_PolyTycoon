public class MoneyController
{
    private long _moneyAmount;
    public System.Action<long, long> OnValueChange;

    public MoneyController()
    {
        this._moneyAmount = 100000;
    }

    public MoneyController(long amount)
    {
        this._moneyAmount = amount;
    }
    
    public long MoneyAmount
    {
        get => _moneyAmount;
        set
        {
            long oldValue = _moneyAmount;
            _moneyAmount = value;
            OnValueChange?.Invoke(oldValue, _moneyAmount);
        }
    }

    public string Money()
    {
        return MoneyAmount + " €";
    }
}