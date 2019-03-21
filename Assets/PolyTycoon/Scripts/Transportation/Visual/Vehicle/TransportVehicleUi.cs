using System.Collections;
using Assets.PolyTycoon.Scripts.Transportation.Model.Product;
using Assets.PolyTycoon.Scripts.Transportation.Model.Transport;
using Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu.TransportRouteCreate;
using Assets.PolyTycoon.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class TransportVehicleUi : AbstractUi
{
	private static TransportRouteCreateController _transportRouteCreateController;
	private TransportVehicle _displayedTransportVehicle;
	private Coroutine _coroutine;
	[SerializeField] private LineRenderer _lineRenderer;
	[SerializeField] private Image _vehicleImage;
	[SerializeField] private Button _vehicleRouteButton;
	[SerializeField] private Button _exitButton;
	[SerializeField] private Text _topSpeedText;
	[SerializeField] private Text _capacityText;
	[SerializeField] private Text _currentSpeedText;
	[SerializeField] private ScrollViewHandle _scrollView;
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
					GameObject viewInstance = _scrollView.AddObject((RectTransform)_scrollViewElementPrefab.transform);
					NeededProductView productView = viewInstance.GetComponent<NeededProductView>();
					productView.ProductData = productStorage.StoredProductData;
					productView.NeededAmountText.text = productStorage.Amount + "/" + productStorage.MaxAmount;
				}

				_vehicleImage.sprite = _displayedTransportVehicle.Sprite;
				_topSpeedText.text = _displayedTransportVehicle.UnloadSpeed.ToString();
				_capacityText.text = _displayedTransportVehicle.TotalCapacity.ToString();
				_currentSpeedText.text = _displayedTransportVehicle.UnloadSpeed.ToString();
				if (_coroutine == null) _coroutine = StartCoroutine(UpdateUi());
				_lineRenderer.positionCount = _displayedTransportVehicle.TransportRoute
					.TransportRouteElements[_displayedTransportVehicle.RouteIndex].Path.WayPoints.Count;
				_lineRenderer.SetPositions(_displayedTransportVehicle.TransportRoute.TransportRouteElements[_displayedTransportVehicle.RouteIndex].Path.WayPoints.ToArray());
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
			foreach (RectTransform rectTransform in _scrollView.ContentObjects)
			{
				NeededProductView productView = rectTransform.gameObject.GetComponent<NeededProductView>();
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
		_scrollView.ClearObjects();
		_topSpeedText.text = "top";
		_currentSpeedText.text = "cur";
		_capacityText.text = "cap";
	}
}
