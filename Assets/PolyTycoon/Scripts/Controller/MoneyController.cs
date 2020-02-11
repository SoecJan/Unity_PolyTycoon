public class MoneyController
{
    private long _moneyAmount = 100000;

    public long MoneyAmount
    {
        get => _moneyAmount;
        set => _moneyAmount = value;
    }

    public string Money()
    {
        long tempMoney = MoneyAmount;
        return tempMoney + " €";
    }
}