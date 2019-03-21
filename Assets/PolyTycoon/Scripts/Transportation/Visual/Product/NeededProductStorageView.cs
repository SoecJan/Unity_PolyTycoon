using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeededProductStorageView : NeededProductView
{

	[SerializeField] private Text _storedProductAmountText;

	public Text StoredProductAmountText {
		get {
			return _storedProductAmountText;
		}

		set {
			_storedProductAmountText = value;
		}
	}
}
