using UnityEngine;
using UnityEngine.UI;

public class NeededProductView : ProductView
{
	[SerializeField] private Text _neededAmountText;

	public void Text(ProductStorage productStorage)
	{
		_neededAmountText.text = productStorage.Amount + "/" + productStorage.MaxAmount;
	}
}
