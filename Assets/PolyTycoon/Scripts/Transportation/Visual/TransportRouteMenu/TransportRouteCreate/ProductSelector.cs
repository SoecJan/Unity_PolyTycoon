using System;
using UnityEngine;

public class ProductSelector : MonoBehaviour
{
	private System.Action<ProductData> onProductSelectAction;
	[Header("Navigation")]
	[SerializeField] private GameObject _visibleGameObject;
	[Header("ScrollView")]
	[SerializeField] private Transform _scrollView;
	[SerializeField] private ProductView _productUiSlotPrefab;

	public GameObject VisibleGameObject {
		get {
			return _visibleGameObject;
		}

		set {
			_visibleGameObject = value;
		}
	}

	public Action<ProductData> OnProductSelectAction {
		get { return onProductSelectAction; }
		set { onProductSelectAction = value; }
	}

	private void PrintClick(ProductData productData)
	{
		OnProductSelectAction(productData);
	}

	private void Start()
	{
		ProductManager productManager = FindObjectOfType<ProductManager>();
		foreach (ProductData product in productManager.Products)
		{
			ProductView productUiSlot = GameObject.Instantiate(_productUiSlotPrefab, _scrollView);
			productUiSlot.ProductData = product;
			productUiSlot.ClickCallBack = PrintClick;
		}
	}
}
