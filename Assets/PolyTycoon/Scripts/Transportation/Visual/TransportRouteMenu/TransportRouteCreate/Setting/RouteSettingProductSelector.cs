using System;
using Assets.PolyTycoon.Resources.Data.ProductData;
using Assets.PolyTycoon.Scripts.Transportation.Model.Product;
using Assets.PolyTycoon.Scripts.Utility;
using UnityEngine;

namespace Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate.Setting
{
	public class RouteSettingProductSelector : MonoBehaviour
	{
		private System.Action<ProductData> onProductSelectAction;
		[Header("Navigation")]
		[SerializeField] private GameObject _visibleGameObject;
		[Header("ScrollView")]
		[SerializeField] private ScrollViewHandle _scrollViewHandle;
		[SerializeField] private GameObject _productUiSlotPrefab;

		public GameObject ProductUiSlotPrefab {
			get {
				return _productUiSlotPrefab;
			}

			set {
				_productUiSlotPrefab = value;
			}
		}

		public GameObject VisibleGameObject {
			get {
				return _visibleGameObject;
			}

			set {
				_visibleGameObject = value;
			}
		}

		public Action<ProductData> OnProductSelectAction
		{
			get { return onProductSelectAction; }
			set { onProductSelectAction = value; }
		}

		private void Start()
		{
			ProductManager productManager = FindObjectOfType<ProductManager>();
			foreach (ProductData product in productManager.Products)
			{
				GameObject productUiSlot = _scrollViewHandle.AddObject((RectTransform)ProductUiSlotPrefab.transform);
				ProductView productView = productUiSlot.GetComponent<ProductView>();
				productView.ProductData = product;
				productView.ProductButton.onClick.AddListener(delegate
				{
					OnProductSelectAction(productView.ProductData);
				});
			}
		}
	}
}
