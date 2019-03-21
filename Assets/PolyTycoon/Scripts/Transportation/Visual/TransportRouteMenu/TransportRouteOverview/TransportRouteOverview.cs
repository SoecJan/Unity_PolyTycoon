using Assets.PolyTycoon.Scripts.Transportation.Model.TransportRoute;
using Assets.PolyTycoon.Scripts.Transportation.Visual.TransportRouteMenu;
using Assets.PolyTycoon.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

public class TransportRouteOverview : AbstractUi
{
	[Header("Scroll View")]
	[SerializeField] private GameObject _overviewElementPrefab;
	[SerializeField] private ScrollViewHandle _routeOverviewScrollView;

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
		GameObject transportRouteOverviewView = _routeOverviewScrollView.AddObject((RectTransform)_overviewElementPrefab.transform);
		TransportRouteOverviewElement overviewElement = transportRouteOverviewView.GetComponent<TransportRouteOverviewElement>();
		overviewElement.TransportRoute = transportRoute;
		return overviewElement;
	}

	public bool Remove(TransportRoute transportRoute)
	{
		foreach (var routeOverview in _routeOverviewScrollView.ContentObjects)
		{
			TransportRouteOverviewElement element = routeOverview.gameObject.GetComponent<TransportRouteOverviewElement>();
			if (element.TransportRoute != transportRoute) continue;
			_routeOverviewScrollView.RemoveObject((RectTransform) element.transform);
			return true;
		}

		return false;
	}
}
