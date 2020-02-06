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
    [SerializeField] private MoneyAnimationBehaviour _cashFlowAnimationObject;

    private void Start()
    {
        _moneyController = new MoneyController();
        PlacementManager._onObjectPlacement += buildingData => { if (buildingData) SpendMoney(buildingData.BuildingPrice); };
    }

    public bool SpendMoney(long amount)
    {
        if (amount == 0) return true;
        if (amount > _moneyController.MoneyAmount) return false;
        _moneyController.MoneyAmount -= amount;
        _moneyText.text = _moneyController.Money();
        MoneyAnimationBehaviour cashflowAnimation = Instantiate(_cashFlowAnimationObject, transform);
        cashflowAnimation.Text.text = "- " + amount + "€";
        Animator cashflowAnimator = cashflowAnimation.GetComponent<Animator>();
        cashflowAnimator.SetTrigger("NegativeCashflow");
        return true;
    }

    public void AddMoney(long amount)
    {
        if (amount == 0) return;
        _moneyController.MoneyAmount += amount;
        _moneyText.text = _moneyController.Money();
        MoneyAnimationBehaviour cashflowAnimation = Instantiate(_cashFlowAnimationObject, transform);
        cashflowAnimation.Text.text = "+ " + amount + "€";
        Animator cashflowAnimator = cashflowAnimation.GetComponent<Animator>();
        cashflowAnimator.SetTrigger("PositiveCashflow");
    }
}
