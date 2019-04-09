using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class TransportVehicleUi : AbstractUi
{
	private static TransportRouteCreateController _transportRouteCreateController;
	private TransportVehicle _displayedTransportVehicle;
	private Coroutine _coroutine;
	//[SerializeField] private LineRenderer _lineRenderer;
	[SerializeField] private Image _vehicleImage;
	[SerializeField] private Button _vehicleRouteButton;
	[SerializeField] private Button _exitButton;
	[SerializeField] private Text _initialCostText;
	[SerializeField] private Text _dailyCostText;
	[SerializeField] private Text _strengthText;
	[SerializeField] private Text _topSpeedText;
	[SerializeField] private Text _capacityText;
	[SerializeField] private Text _loadSpeedText;
	[SerializeField] private RectTransform _scrollView;
	[SerializeField] private NeededProductView _scrollViewElementPrefab;

	public TransportVehicle DisplayedTransportVehicle {
		get {
			return _displayedTransportVehicle;
		}

		set
		{
			if (_displayedTransportVehicle == value) return;
			_displayedTransportVehicle = value;
			if (_displayedTransportVehicle == null)
			{
				SetVisible(false);
			}
			else
			{
				foreach (ProductStorage productStorage in _displayedTransportVehicle.LoadedProducts.Values)
				{
					NeededProductView neededProductView = GameObject.Instantiate(_scrollViewElementPrefab, _scrollView);
					neededProductView.ProductData = productStorage.StoredProductData;
					neededProductView.NeededAmountText.text = productStorage.Amount + "/" + productStorage.MaxAmount;
				}

				_vehicleImage.sprite = _displayedTransportVehicle.Sprite;
				_initialCostText.text = "-";
				_dailyCostText.text = "-";
				_strengthText.text = "-";
				_topSpeedText.text = _displayedTransportVehicle.UnloadSpeed.ToString();
				_capacityText.text = _displayedTransportVehicle.TotalCapacity.ToString();
				_loadSpeedText.text = _displayedTransportVehicle.UnloadSpeed.ToString();
				if (_coroutine == null) _coroutine = StartCoroutine(UpdateUi());
				SetVisible(true);
			}
		}
	}

	void Start()
	{
		if (!_transportRouteCreateController) _transportRouteCreateController = FindObjectOfType<TransportRouteCreateController>();
		_vehicleRouteButton.onClick.AddListener(OnVehicleRouteButtonClick);
		_exitButton.onClick.AddListener(delegate { SetVisible(false); Reset(); });
	}

	void OnVehicleRouteButtonClick()
	{
		if (_displayedTransportVehicle != null)
		{
			_transportRouteCreateController
				.LoadTransportRoute(
					_displayedTransportVehicle
						.TransportRoute);
		}
	}

	private IEnumerator UpdateUi()
	{
		while (_displayedTransportVehicle != null)
		{
			for (int i = 0; i< _scrollView.childCount; i++)
			{
				NeededProductView productView = _scrollView.GetChild(i).gameObject.GetComponent<NeededProductView>();
				ProductStorage productStorage = _displayedTransportVehicle.LoadedProducts[productView.ProductData];
				productView.NeededAmountText.text = productStorage.Amount + "/" + productStorage.MaxAmount;
			}
			yield return new WaitForSeconds(1);
		}

		_coroutine = null;
	}

	public new void Reset()
	{ 
		_vehicleImage.sprite = null;
		_displayedTransportVehicle = null;
		for (int i = 0; i< _scrollView.childCount; i++)
		{
			GameObject.Destroy(_scrollView.GetChild(i).gameObject);
		}
		_initialCostText.text = "-";
		_dailyCostText.text = "-";
		_strengthText.text = "-";
		_topSpeedText.text = "-";
		_capacityText.text = "-";
		_loadSpeedText.text = "-";
	}
}
