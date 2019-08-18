using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AmountProductView : ProductView
{
	[FormerlySerializedAs("_neededAmountText")] [SerializeField] private TextMeshProUGUI _amountText;

	public void Text(ProductStorage productStorage)
	{
		if (productStorage != null)
			_amountText.text = productStorage.Amount + "/" + productStorage.MaxAmount;
	}
}
