using System;
using TMPro;
using UnityEngine;

public class MoneyUiView : MonoBehaviour
{
    private MoneyController _moneyMoneyController;
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private MoneyAnimationBehaviour _cashFlowAnimationObject;

    public MoneyController MoneyController
    {
        get => _moneyMoneyController;
        set => _moneyMoneyController = value;
    }

    private void Awake()
    {
        MoneyController = new MoneyController();
    }

    private void Start()
    {
        MoneyController.OnValueChange += delegate(long oldValue, long newValue)
        {
            long difference = newValue - oldValue;
            _moneyText.text = MoneyController.Money();

            // Animation
            MoneyAnimationBehaviour cashflowAnimation = Instantiate(_cashFlowAnimationObject, transform);
            cashflowAnimation.Text.text = (difference < 0 ? "- " : "+ ") + difference + "€";
            Animator cashflowAnimator = cashflowAnimation.GetComponent<Animator>();
            cashflowAnimator.SetTrigger(difference < 0 ? "NegativeCashflow" : "PositiveCashflow");
        };
        PlacementController._onObjectPlacement += delegate(BuildingData buildingData)
        {
            Debug.Log("Object Placement Cost: " + (buildingData ? -buildingData.BuildingPrice : 0));
            if (buildingData) ChangeValueBy(-buildingData.BuildingPrice);
        };
    }

    public bool ChangeValueBy(long amount)
    {
        if (amount == 0) return true; // Needed for cities that are free for the player
        if (MoneyController.MoneyAmount - amount < 0) return false;
        MoneyController.MoneyAmount += amount;
        return true;
    }
}