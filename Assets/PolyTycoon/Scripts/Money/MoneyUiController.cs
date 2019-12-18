using TMPro;
using UnityEngine;

class MoneyController
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

public class MoneyUiController : MonoBehaviour
{
    private MoneyController _moneyController;
    [SerializeField] private TMP_Text _moneyText;

    private void Start()
    {
        _moneyController = new MoneyController();
    }

    public bool SpendMoney(long amount)
    {
        if (amount > _moneyController.MoneyAmount) return false;
        _moneyController.MoneyAmount -= amount;
        _moneyText.text = _moneyController.Money();
        return true;
    }

    public void AddMoney(long amount)
    {
        _moneyController.MoneyAmount += amount;
        _moneyText.text = _moneyController.Money();
    }
}
