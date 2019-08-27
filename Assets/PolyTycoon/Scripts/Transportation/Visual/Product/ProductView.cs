using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class communicates between Factory and FactoryUI if the player wants to change the produced productData of a factory.
/// </summary>
public class ProductView : MonoBehaviour
{
	#region Attributes
	private ProductData _productData; // The productData this UI Slot represents

	[SerializeField] private Button _productButton;

	[SerializeField] private Image _image; // The Image that is going to show the productData sprite to the player
	#endregion

	#region Getter & Setter
	public ProductData ProductData {
		get => _productData;

		set {
			_productData = value;
			_image.sprite = _productData.ProductSprite;
		}
	}

	public Button ProductButton => _productButton;

	public System.Action<ProductData> ClickCallBack
	{
		private get; set;
	}
	#endregion

	#region Default Methods
	void Start()
	{
		_productButton.onClick.AddListener(delegate
		{
			ClickCallBack?.Invoke(_productData);
		});
	}
	#endregion
}
