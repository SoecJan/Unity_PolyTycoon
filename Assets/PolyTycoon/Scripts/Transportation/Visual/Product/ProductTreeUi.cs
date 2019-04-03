using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductTreeUi : AbstractUi
{
	[SerializeField] private Button _showButton;
	[SerializeField] private NeededProductView _neededProductViewPrefab;
	[SerializeField] private RectTransform _parentTransform;
	[SerializeField] private GameObject _uiConnectorGameObject;
	private static ProductManager _productManager;
	private List<NeededProductView> _displayedViews;

	// Use this for initialization
	void Start()
	{
		_showButton.onClick.AddListener(delegate { SetVisible(!VisibleObject.activeSelf); });
		_displayedViews = new List<NeededProductView>();
		_productManager = FindObjectOfType<ProductManager>();
		for (int i = 0; i < _productManager.Products.Count; i++)
		{
			ProductData productData = _productManager.Products[i];
			GameObject productGameObject = Instantiate(_neededProductViewPrefab.gameObject, _parentTransform);
			RectTransform productRectTransform = ((RectTransform)productGameObject.transform);
			productRectTransform.anchoredPosition = new Vector2(i * 60f, -120f);
			NeededProductView productView = productGameObject.GetComponent<NeededProductView>();
			_displayedViews.Add(productView);
			productView.ProductData = productData;
			productView.NeededAmountText.text = "";

			if (productData.NeededProduct.Product == null) continue;
			NeededProduct neededProduct = productData.NeededProduct;
			GameObject neededProductGameObject = Instantiate(_neededProductViewPrefab.gameObject, _parentTransform);
			RectTransform neededProductRectTransform = ((RectTransform)neededProductGameObject.transform);
			neededProductRectTransform.anchoredPosition = productRectTransform.anchoredPosition + new Vector2(0f, 120f);
			NeededProductView neededProductView = neededProductGameObject.GetComponent<NeededProductView>();
			_displayedViews.Add(neededProductView);
			neededProductView.ProductData = neededProduct.Product;
			neededProductView.NeededAmountText.text = neededProduct.Amount.ToString();

			GameObject connectorGameObject = Instantiate(_uiConnectorGameObject, _parentTransform);
			RectTransform connectorRectTransform = ((RectTransform)connectorGameObject.transform);
			connectorRectTransform.sizeDelta = new Vector2(10f, neededProductRectTransform.anchoredPosition.y - productRectTransform.anchoredPosition.y - productRectTransform.sizeDelta.y);
			connectorRectTransform.anchoredPosition = productRectTransform.anchoredPosition + new Vector2((productRectTransform.sizeDelta.x / 2f) - (connectorRectTransform.sizeDelta.x / 2f), productRectTransform.sizeDelta.y);

		}

		_parentTransform.sizeDelta = new Vector2(_productManager.Products.Count * 60f, 60f);
	}

	public override void OnShortCut()
	{
		base.OnShortCut();
		SetVisible(!VisibleObject.activeSelf);
	}
}
