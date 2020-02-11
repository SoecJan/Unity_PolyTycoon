using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NeededProductView : AmountProductView
{
	[FormerlySerializedAs("_storedProductAmountText")] [SerializeField] private TextMeshProUGUI _neededProductText;

	public string StoredProductAmountText {
		get { return _neededProductText.text; }
		set { _neededProductText.text = value; }
	} 
}
