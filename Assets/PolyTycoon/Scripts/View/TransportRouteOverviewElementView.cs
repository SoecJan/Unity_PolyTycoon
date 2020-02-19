
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TransportRouteOverviewElementView : MonoBehaviour
{
	private static TransportRouteCreationView _transportRouteCreationView;
	private static ITransportRouteManager _transportRouteManager;
	private TransportRoute _transportRoute;

	[Header("Navigation")]
	[SerializeField] private Button _editButton;
	[SerializeField] private Button _removeButton;

	[Header("Information")]
	[SerializeField] private TextMeshProUGUI _routeNameText;

	public TransportRoute TransportRoute {
		get => _transportRoute;

		set {
			_transportRoute = value;
			_routeNameText.text = _transportRoute.RouteName;
		}
	}

	private void Start()
	{
		_editButton.onClick.AddListener(OnEditClick);
		_removeButton.onClick.AddListener(OnRemoveClick);
	}

	private void OnEditClick()
	{
		if (!_transportRouteCreationView) _transportRouteCreationView = Object.FindObjectOfType<TransportRouteCreationView>();
		_transportRouteCreationView.LoadRoute(TransportRoute);
		Object.FindObjectOfType<CameraBehaviour>().SetTarget(TransportRoute.TransportVehicle.transform);
	}

	private void OnRemoveClick()
	{
		if (_transportRouteManager == null) _transportRouteManager = Object.FindObjectOfType<GameHandler>().TransportRouteManager;
		_transportRouteManager.RemoveTransportRoute(TransportRoute);
	}

	private void Reset()
	{
		_routeNameText.text = "";
		_transportRoute = null;
	}
}
