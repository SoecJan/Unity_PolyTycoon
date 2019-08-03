using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

/// <summary>
/// Ui that displays information on a selected transport vehicle.
/// By setting the DisplayedTransportVehicle to a value or null.
/// </summary>
public class TransportVehicleUi : AbstractUi
{
    // dependencies
    private TransportRouteCreateController _transportRouteCreateController;
    private TransportVehicle _displayedTransportVehicle;
    private Coroutine _coroutine;

    // Ui navigation
    [SerializeField] private Button _vehicleRouteButton;
    [SerializeField] private Button _exitButton;

    // Vehicle display
    [SerializeField] private Image _vehicleImage;

    // Vehicle information display
    [SerializeField] private Text _initialCostText;
    [SerializeField] private Text _dailyCostText;
    [SerializeField] private Text _strengthText;
    [SerializeField] private Text _topSpeedText;
    [SerializeField] private Text _capacityText;
    [SerializeField] private Text _loadSpeedText;

    // Loaded product display
    [SerializeField] private RectTransform _scrollView;
    [SerializeField] private NeededProductView _scrollViewElementPrefab;

    /// <summary>
    /// Null value: Removes displayed outline, resets the ui and makes it invisible
    ///
    /// Value: Sets displayed outline, loads vehicle & product information and makes the ui visible
    /// </summary>
    public TransportVehicle DisplayedTransportVehicle
    {
        set
        {
            if (_displayedTransportVehicle == value) return; // Don't update the selected object
            if (_displayedTransportVehicle) _displayedTransportVehicle.Outline.enabled = false; // remove outline
            _displayedTransportVehicle = value; 
            if (_displayedTransportVehicle) _displayedTransportVehicle.Outline.enabled = true; // add outline

            ShowVehicleInformation(_displayedTransportVehicle); // Update information display
            SetVisible(_displayedTransportVehicle != null); // Show Ui, if a vehicle is selected
            
            if (_coroutine == null) _coroutine = StartCoroutine(UpdateUi());
        }
    }

    void Start()
    {
        if (!_transportRouteCreateController)
            _transportRouteCreateController = FindObjectOfType<TransportRouteCreateController>();
        _vehicleRouteButton.onClick.AddListener(OnVehicleRouteButtonClick);
        _exitButton.onClick.AddListener(delegate
        {
            DisplayedTransportVehicle = null;
            Reset();
        });
    }

    private void OnVehicleRouteButtonClick()
    {
        if (_displayedTransportVehicle != null)
        {
            _transportRouteCreateController.LoadRoute(_displayedTransportVehicle.TransportRoute);
        }
    }

    private void ShowVehicleInformation(TransportVehicle transportVehicle)
    {
        // Set text displays
        _vehicleImage.sprite = transportVehicle != null ? transportVehicle.Sprite : null;
        _initialCostText.text = transportVehicle != null ? "-" : "/";
        _dailyCostText.text = transportVehicle != null ? "-" : "/";
        _strengthText.text = transportVehicle != null ? "-" : "/";
        _topSpeedText.text = transportVehicle != null ? transportVehicle.TransferTime.ToString() : "-";
        _capacityText.text = transportVehicle != null ? transportVehicle.TotalCapacity.ToString() : "-";
        _loadSpeedText.text = transportVehicle != null ? transportVehicle.TransferTime.ToString() : "-";
        
        // Set loaded products view
        if (transportVehicle != null)
        {
            foreach (ProductData productData in transportVehicle.LoadedProducts)
            {
                NeededProductView neededProductView = Object.Instantiate(_scrollViewElementPrefab, _scrollView);
                neededProductView.ProductData = productData;
                neededProductView.Text(transportVehicle.TransportStorage(productData));
            }
        }
        else
        {
            for (int i = 0; i < _scrollView.childCount; i++)
            {
                Object.Destroy(_scrollView.GetChild(i).gameObject);
            }
        }
    }

    private IEnumerator UpdateUi()
    {
        while (_displayedTransportVehicle != null)
        {
            for (int i = 0; i < _scrollView.childCount; i++)
            {
                NeededProductView productView = _scrollView.GetChild(i).gameObject.GetComponent<NeededProductView>();
                ProductStorage productStorage = _displayedTransportVehicle.TransportStorage(productView.ProductData);
                productView.Text(productStorage);
            }
            yield return new WaitForSeconds(1);
        }
        _coroutine = null;
    }
}