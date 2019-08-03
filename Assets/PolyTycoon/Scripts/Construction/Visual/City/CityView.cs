using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class CityView : AbstractUi
{
	#region Attributes
	private ICityBuilding _cityBuilding;
	[Header("Navigation")]
	[SerializeField] private Button _exitButton;
	[Header("UI")]
	[SerializeField] private RectTransform _neededProductScrollView;
	[SerializeField] private RectTransform _producedProductScrollView;
	[SerializeField] private NeededProductView _productUiPrefab;
	[SerializeField] private Text _cityGeneralText;
	#endregion

	#region Getter & Setter
	public ICityBuilding CityBuilding {
		private get => _cityBuilding;

		set {
			if (_cityBuilding == value) return;
			_cityBuilding = value;
			Reset();
			if (_cityBuilding == null)
			{
				SetVisible(false);
				return;
			}
			Debug.Log("New City selected");
			IProductReceiver cityPlaceable = ((IProductReceiver) CityBuilding.CityPlaceable());
			foreach (ProductData neededProduct in cityPlaceable.ReceivedProductList())
			{
				NeededProductView neededProductView = GameObject.Instantiate(_productUiPrefab, _neededProductScrollView);
				neededProductView.ProductData = neededProduct;
				neededProductView.Text(cityPlaceable.ReceiverStorage(neededProduct));
			}

			ProductStorage producedProductStorage = ((IProductEmitter) CityBuilding.CityPlaceable()).EmitterStorage();
			NeededProductView producedProductView = GameObject.Instantiate(_productUiPrefab, _producedProductScrollView);
			producedProductView.ProductData = producedProductStorage.StoredProductData;
			producedProductView.Text(producedProductStorage);
			
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
			_cityGeneralText.text = "Name: " + CityBuilding.CityPlaceable().transform.name + 
			                        "\nPeople: " + CityBuilding.CityPlaceable().CurrentInhabitantCount();
			for (int i = 0; i < _neededProductScrollView.childCount; i++)
			{
				NeededProductView productView = _neededProductScrollView.transform.GetChild(i).GetComponent<NeededProductView>();
				ProductStorage productStorage = ((IProductReceiver)_cityBuilding.CityPlaceable()).ReceiverStorage(productView.ProductData);
				productView.Text(productStorage);
			}
			for (int i = 0; i < _producedProductScrollView.childCount; i++)
			{
				NeededProductView productView = _producedProductScrollView.transform.GetChild(i).GetComponent<NeededProductView>();
				ProductStorage productStorage = ((IProductEmitter)_cityBuilding.CityPlaceable()).EmitterStorage();
				productView.Text(productStorage);
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
