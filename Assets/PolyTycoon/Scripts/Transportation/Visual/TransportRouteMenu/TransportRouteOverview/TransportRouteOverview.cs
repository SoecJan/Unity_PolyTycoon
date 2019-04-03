using UnityEngine;
using UnityEngine.UI;

public class TransportRouteOverview : AbstractUi
{
	[Header("Scroll View")]
	[SerializeField] private TransportRouteOverviewElement _overviewElementPrefab;
	[SerializeField] private RectTransform _routeOverviewScrollView;

	[Header("Navigation")] 
	[SerializeField] private Button _showButton;
	[SerializeField] private Button _exitButton;

	private void Start()
	{
		_showButton.onClick.AddListener(delegate { SetVisible(!VisibleObject.activeSelf);});
		_exitButton.onClick.AddListener(OnExitClick);
	}

	private void OnExitClick()
	{
		SetVisible(false);
	}

	public TransportRouteOverviewElement Add(TransportRoute transportRoute)
	{
		TransportRouteOverviewElement transportRouteOverviewView = GameObject.Instantiate(_overviewElementPrefab, _routeOverviewScrollView);
		transportRouteOverviewView.TransportRoute = transportRoute;
		return transportRouteOverviewView;
	}

	public bool Remove(TransportRoute transportRoute)
	{
		Debug.Log("Remove Overview Element");
		//foreach (var routeOverview in _routeOverviewScrollView.ContentObjects)
		//{
		//	TransportRouteOverviewElement element = routeOverview.gameObject.GetComponent<TransportRouteOverviewElement>();
		//	if (element.TransportRoute != transportRoute) continue;
		//	_routeOverviewScrollView.RemoveObject((RectTransform) element.transform);
		//	return true;
		//}

		return false;
	}
}
