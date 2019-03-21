using Assets.PolyTycoon.Resources.Data.ProductData;
using Assets.PolyTycoon.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class communicates between Factory and FactoryUI if the player wants to change the produced productData of a factory.
/// </summary>
public class ProductView : MonoBehaviour
{
	#region Attributes
	private ProductData _productData; // The productData this UI Slot represents

	[SerializeField]
	private Button _productButton;

	//[SerializeField]
	//private static FactoryView factoryUI; // The FactoryUI this UI Slot is presented by

	[SerializeField]
	private Image _image; // The Image that is going to show the productData sprite to the player
	#endregion

	#region Getter & Setter
	public ProductData ProductData {
		get {
			return _productData;
		}

		set {
			_productData = value;
			_image.sprite = _productData.ProductSprite;
		}
	}

	public Button ProductButton {
		get {
			return _productButton;
		}

		set {
			_productButton = value;
		}
	}

	//public static FactoryView FactoryUI {
	//	get {
	//		return factoryUI;
	//	}

	//	set {
	//		factoryUI = value;
	//	}
	//}
	#endregion

	#region Default Methods
	void Start()
	{
		//if (!FactoryUI)
		//{
		//	FactoryUI = GameObject.FindObjectOfType<FactoryView>(); // Get the static reference 
		//}
	}
	#endregion

	#region UI Input
	//public void OnButtonClick()
	//{
	//	if (FactoryUI.Factory)
	//		FactoryUI.Factory.ProductData = ProductData; // If this UI Slot is clicked -> Factory ProductData is set
	//}
	#endregion
}
