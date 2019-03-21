using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeededProductView : ProductView
{

	[SerializeField] private Text _neededAmountText;

	public Text NeededAmountText {
		get {
			return _neededAmountText;
		}

		set {
			_neededAmountText = value;
		}
	}
}
