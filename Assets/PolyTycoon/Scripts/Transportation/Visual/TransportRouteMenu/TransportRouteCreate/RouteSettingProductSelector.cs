using System;
using UnityEngine;

public class RouteSettingProductSelector : MonoBehaviour
{
	private System.Action<ProductData> onProductSelectAction;
	[Header("Navigation")]
	[SerializeField] private GameObject _visibleGameObject;
	[Header("ScrollView")]
	[SerializeField] private Transform _scrollView;
	[SerializeField] private TransportProductView _productUiSlotPrefab;

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

	private void Start()
	{
		ProductManager productManager = FindObjectOfType<ProductManager>();
		foreach (ProductData product in productManager.Products)
		{
			TransportProductView productUiSlot = GameObject.Instantiate(_productUiSlotPrefab, _scrollView);
			productUiSlot.Product = product;
			productUiSlot.SelectionButton.onValueChanged.AddListener(delegate
			{
				OnProductSelectAction(productUiSlot.Product);
			});
		}
	}
}
