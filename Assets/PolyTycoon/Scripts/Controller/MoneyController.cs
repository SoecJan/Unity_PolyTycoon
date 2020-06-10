using System;

public class MoneyController
{
    private long _moneyAmount;
    private string _currency = "€";

    private string[] _abbreviations =
    {
        "",
        "K",
        "M",
        "B",
        "T",
        "Q",
        "S",
        "D"
    };

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

    public string Currency
    {
        get => _currency;
        set => _currency = value;
    }

    public string ToCurrencyString()
    {
        long temp = _moneyAmount;
        int abbreviationIndex = 0;
        while (temp > 1000)
        {
            temp /= 1000;
            abbreviationIndex += 1;
        }
        return Currency + " " + temp + _abbreviations[abbreviationIndex];
    }
}