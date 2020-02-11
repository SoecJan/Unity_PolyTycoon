using System;
using System.Collections.Generic;
using UnityEngine;

public class ProductSelectionView : MonoBehaviour
{
	[Header("Navigation")]
	[SerializeField] private GameObject _visibleGameObject;
	[Header("ScrollView")]
	[SerializeField] private Transform _scrollView;
	[SerializeField] private ProductView _productUiSlotPrefab;

	public GameObject VisibleGameObject => _visibleGameObject;

	public void ShowProducts(List<ProductData> _shownProducts)
	{
		for (int i = 0; i < _scrollView.childCount; i++)
		{
			GameObject childGameObject = _scrollView.GetChild(i).gameObject;
			ProductView productView = childGameObject.GetComponent<ProductView>();
			childGameObject.SetActive(_shownProducts.Contains(productView.ProductData));
		}
	}
	
	public Action<ProductData> OnProductSelectAction { private get; set; }

	private void OnProductSelect(ProductData productData)
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
			productUiSlot.ClickCallBack = OnProductSelect;
		}
	}
}
