using UnityEngine;
using UnityEngine.UI;

public class TransportRouteProductView : PoolableObject
{
	private ProductData productData;
	private TransportRouteSetting _setting;
	private static Sprite _defaultSprite;
	[SerializeField] private Image _productImage;
	[SerializeField] private Toggle _selectionToggle;

	public ProductData Product
	{
		get => productData;
		set
		{
			productData = value;
			if (!_defaultSprite)
			{
				_defaultSprite = _productImage.sprite;
			}
			_productImage.sprite = value ? value.ProductSprite : _defaultSprite;
			if (_setting == null) _setting = new TransportRouteSetting();
			_setting.ProductData = value;
		}
	}

	public TransportRouteSetting Setting
	{
		get => _setting ?? (_setting = new TransportRouteSetting());
		set
		{
			_setting = value;
			Product = value.ProductData;
		}
	}

	public Toggle SelectionToggle => _selectionToggle;

	public override void Hide()
	{
		base.Hide();
		_productImage.sprite = _defaultSprite;
		productData = null;
	}
}
