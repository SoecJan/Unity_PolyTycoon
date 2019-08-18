using System.Collections;
using RTS_Cam;
using TMPro;
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
    private TransportVehicle _transportVehicle;
    private Coroutine _coroutine;
    private static RTS_Camera _rtsCamera; // For vehicle following

    // Ui navigation
    [SerializeField] private Button _exitButton;

    // Vehicle display
    [SerializeField] private Image _vehicleImage;
    [SerializeField] private Button _vehicleFollowButton;

    // Vehicle information display
    [SerializeField] private TextMeshProUGUI _initialCostText;
    [SerializeField] private TextMeshProUGUI _dailyCostText;
    [SerializeField] private TextMeshProUGUI _strengthText;
    [SerializeField] private TextMeshProUGUI _topSpeedText;
    [SerializeField] private TextMeshProUGUI _capacityText;
    [SerializeField] private TextMeshProUGUI _transferTimeText;

    // Loaded product display
    [SerializeField] private RectTransform _scrollView;
    [SerializeField] private AmountProductView _scrollViewElementPrefab;

    /// <summary>
    /// Null value: Removes displayed outline, resets the ui and makes it invisible
    ///
    /// Value: Sets displayed outline, loads vehicle & product information and makes the ui visible
    /// </summary>
    public TransportVehicle DisplayedTransportVehicle
    {
        set
        {
            if (_transportVehicle == value) return; // Don't update the selected object
            if (_transportVehicle) _transportVehicle.Outline.enabled = false; // remove outline
            ShowVehicleInformation(null);
            _transportVehicle = value; 
            if (_transportVehicle) _transportVehicle.Outline.enabled = true; // add outline

            ShowVehicleInformation(_transportVehicle); // Update information display
            SetVisible(_transportVehicle != null); // Show Ui, if a vehicle is selected
            
            if (_coroutine == null) _coroutine = StartCoroutine(UpdateUi());
        }
    }

    void Start()
    {
        _exitButton.onClick.AddListener(delegate
        {
            DisplayedTransportVehicle = null;
            Reset();
        });
        _vehicleFollowButton.onClick.AddListener(delegate
        {
            if (!_rtsCamera) _rtsCamera = FindObjectOfType<RTS_Camera>();
            _rtsCamera.SetTarget(_transportVehicle ? _transportVehicle.transform : null);
        });
    }

    private void ShowVehicleInformation(TransportVehicle transportVehicle)
    {
        // Set text displays
        _vehicleImage.sprite = transportVehicle != null ? transportVehicle.Sprite : null;
        _initialCostText.text = transportVehicle != null ? "-" : "/";
        _dailyCostText.text = transportVehicle != null ? "-" : "/";
        _strengthText.text = transportVehicle != null ? "-" : "/";
        _topSpeedText.text = transportVehicle != null ? transportVehicle.MaxSpeed.ToString() : "-";
        _capacityText.text = transportVehicle != null ? transportVehicle.MaxCapacity.ToString() : "-";
        _transferTimeText.text = transportVehicle != null ? transportVehicle.TransferTime.ToString() : "-";
        
        // Set loaded products view
        if (transportVehicle != null)
        {
            foreach (ProductData productData in transportVehicle.LoadedProducts)
            {
                AmountProductView amountProductView = Object.Instantiate(_scrollViewElementPrefab, _scrollView);
                amountProductView.ProductData = productData;
                amountProductView.Text(transportVehicle.TransportStorage(productData));
            }
        }
        else
        {
            // remove loaded products
            for (int i = 0; i < _scrollView.childCount; i++)
            {
                Object.Destroy(_scrollView.GetChild(i).gameObject);
            }
        }
    }

    private IEnumerator UpdateUi()
    {
        while (_transportVehicle != null)
        {
            for (int i = 0; i < _scrollView.childCount; i++)
            {
                AmountProductView productView = _scrollView.GetChild(i).gameObject.GetComponent<AmountProductView>();
                ProductStorage productStorage = _transportVehicle.TransportStorage(productView.ProductData);
                productView.Text(productStorage);
            }
            yield return new WaitForSeconds(1);
        }
        _coroutine = null;
    }
}