using UnityEngine;
using UnityEngine.UI;

public class NeededProductStorageView : NeededProductView
{
	[SerializeField] private Text _storedProductAmountText;

	public Text StoredProductAmountText => _storedProductAmountText;
}
