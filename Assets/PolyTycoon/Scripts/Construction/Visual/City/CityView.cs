using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class CityView : AbstractUi
{
	#region Attributes
	private CityBuilding _cityBuilding;
	[Header("Navigation")]
	[SerializeField] private Button _exitButton;
	[Header("UI")]
	[SerializeField] private RectTransform _neededProductScrollView;
	[SerializeField] private RectTransform _producedProductScrollView;
	[SerializeField] private NeededProductView _productUiPrefab;
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
			foreach (ProductStorage neededProductStorage in ((IConsumer)CityBuilding.CityPlaceable).NeededProducts().Values)
			{
				NeededProductView neededProductView = GameObject.Instantiate(_productUiPrefab, _neededProductScrollView);
				neededProductView.ProductData = neededProductStorage.StoredProductData;
				neededProductView.NeededAmountText.text = neededProductStorage.Amount + "/" + neededProductStorage.MaxAmount;
			}

			ProductStorage producedProductStorage = ((IProducer) CityBuilding.CityPlaceable).ProducedProductStorage();
			NeededProductView producedProductView = GameObject.Instantiate(_productUiPrefab, _producedProductScrollView);
			producedProductView.ProductData = producedProductStorage.StoredProductData;
			producedProductView.NeededAmountText.text = producedProductStorage.Amount + "/" + producedProductStorage.MaxAmount;
			
			StartCoroutine(UpdateUi());
			SetVisible(true);
		}
	}
	#endregion

	#region Methods
	private void Start()
	{
		_exitButton.onClick.AddListener(delegate { CityBuilding = null; });
	}

	/// <summary>
	/// Coroutine that updates the CityPlaceable UI
	/// </summary>
	/// <returns></returns>
	private IEnumerator UpdateUi()
	{
		while (_cityBuilding != null)
		{
			_cityGeneralText.text = "Name: " + CityBuilding.CityPlaceable.CityName + 
			                        "\nPeople: " + CityBuilding.CityPlaceable.CurrentInhabitantCount();
			for (int i = 0; i < _neededProductScrollView.childCount; i++)
			{
				NeededProductView productView = _neededProductScrollView.transform.GetChild(i).GetComponent<NeededProductView>();
				ProductStorage productStorage = ((IConsumer)_cityBuilding.CityPlaceable).NeededProducts()[productView.ProductData];
				productView.NeededAmountText.text = productStorage.Amount + "/" + productStorage.MaxAmount;
			}
			for (int i = 0; i < _producedProductScrollView.childCount; i++)
			{
				NeededProductView productView = _producedProductScrollView.transform.GetChild(i).GetComponent<NeededProductView>();
				ProductStorage productStorage = ((IProducer)_cityBuilding.CityPlaceable).ProducedProductStorage();
				productView.NeededAmountText.text = productStorage.Amount + "/" + productStorage.MaxAmount;
			}
			yield return new WaitForSeconds(1);
		}
	}

	public override void Reset()
	{
		for (int i = 0; i < _neededProductScrollView.childCount; i++)
		{
			Destroy(_neededProductScrollView.transform.GetChild(i).gameObject);
		}

		for (int i = 0; i < _producedProductScrollView.childCount; i++)
		{
			Destroy(_producedProductScrollView.transform.GetChild(i).gameObject);
		}
	}
	#endregion
}
