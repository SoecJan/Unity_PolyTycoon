using System.Collections;
using System.Linq;
using Assets.PolyTycoon.Resources.Data.ProductData;
using Assets.PolyTycoon.Scripts.Construction.Model.City;
using Assets.PolyTycoon.Scripts.Transportation.Model.Product;
using Assets.PolyTycoon.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PolyTycoon.Scripts.Construction.Visual.City
{
	public class CityView : AbstractUi
	{
		#region Attributes
		private CityBuilding _cityBuilding;
		[Header("Navigation")]
		[SerializeField] private Button _exitButton;
		[Header("UI")]
		[SerializeField] private ScrollViewHandle _neededProductScrollView;
		[SerializeField] private GameObject _productUiPrefab;
		[SerializeField] private Text _cityGeneralText;
		#endregion

		#region Getter & Setter
		public CityBuilding CityBuilding {
			get {
				return _cityBuilding;
			}

			set {
				if (_cityBuilding == value) return;
				_cityBuilding = value;
				Reset();
				if (!_cityBuilding)
				{
					SetVisible(false);
					return;
				}
				Debug.Log("New City selected");
				foreach (ProductStorage productStorage in ((IConsumer)CityBuilding.CityPlaceable).NeededProducts().Values)
				{
					GameObject productSlot = _neededProductScrollView.AddObject((RectTransform)_productUiPrefab.transform);
					NeededProductView productUiSlot = productSlot.GetComponent<NeededProductView>();
					productUiSlot.ProductData = productStorage.StoredProductData;
					productUiSlot.NeededAmountText.text = productStorage.Amount + "/" + productStorage.MaxAmount;
				}
				StartCoroutine(UpdateUi());
				SetVisible(true);
			}
		}
		#endregion

		#region Methods
		private void Start()
		{
			_exitButton.onClick.AddListener(delegate { SetVisible(false); Reset();;
			});
		}

		/// <summary>
		/// Coroutine that updates the CityPlaceable UI
		/// </summary>
		/// <returns></returns>
		private IEnumerator UpdateUi()
		{
			while (_cityBuilding != null)
			{
				ProductStorage cityProduct = ((IConsumer) CityBuilding.CityPlaceable).NeededProducts().Values.ElementAt(0);
				_cityGeneralText.text = "Size: " + CityBuilding.CityPlaceable.ChildMapPlaceables.Count.ToString() + "\nLocation: " + CityBuilding.CityPlaceable.CenterPosition.ToString() + "\nPeople: " + CityBuilding.CityPlaceable.CurrentInhabitantCount() + "\nProducts: " + cityProduct.StoredProductData.ProductName;
				foreach (RectTransform rectTransform in _neededProductScrollView.ContentObjects)
				{
					NeededProductView productView = rectTransform.gameObject.GetComponent<NeededProductView>();
					ProductStorage productStorage = ((IConsumer) _cityBuilding.CityPlaceable).NeededProducts()[productView.ProductData];
					productView.NeededAmountText.text = productStorage.Amount + "/" + productStorage.MaxAmount;
				}
				yield return new WaitForSeconds(1);
			}
		}

		public new void Reset()
		{
			_neededProductScrollView.ClearObjects();
		}
		#endregion
	}
}
