using UnityEngine;
using UnityEngine.UI;

public class TransportRouteProductView : PoolableObject
{
	private ProductData productData;
	private TransportRouteSetting _setting;
	private static Sprite _defaultSprite;
	[SerializeField] private Image _productImage;
	[SerializeField] private Toggle _selectionButton;

	public ProductData Product
	{
		get { return productData; }
		set
		{
			productData = value;
			_productImage.sprite = value ? value.ProductSprite : _defaultSprite;
			if (_setting == null) _setting = new TransportRouteSetting();
			_setting.ProductData = value;
		}
	}

	public TransportRouteSetting Setting
	{
		get { return _setting ?? (_setting = new TransportRouteSetting()); }
		set
		{
			_setting = value;
			Product = value.ProductData;
		}
	}

	public Toggle SelectionButton
	{
		get { return _selectionButton; }
		set { _selectionButton = value; }
	}

	void Start()
    {
		if (!_defaultSprite)
		{
			_defaultSprite = _productImage.sprite;
		}
    }

	public override void Hide()
	{
		base.Hide();
		_productImage.sprite = _defaultSprite;
		productData = null;
	}

	public override void Show()
	{
		base.Show();
	}
}
