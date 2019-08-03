using UnityEngine;
using UnityEngine.UI;

public interface ITransportRouteOverview
{
	/// <summary>
	/// Adds a new entry to the overview
	/// </summary>
	/// <param name="transportRoute"></param>
	/// <returns></returns>
	TransportRouteOverviewElement Add(TransportRoute transportRoute);

	/// <summary>
	/// Removes an old entry from the overview
	/// </summary>
	/// <param name="transportRoute">the transport route to be removed</param>
	/// <returns>true if the element was found. </returns>
	bool Remove(TransportRoute transportRoute);
}

/// <summary>
/// Overview of transportroutes that were created by the player using <see cref="TransportRouteManager"/>
/// </summary>
public class TransportRouteOverview : AbstractUi, ITransportRouteOverview
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

	/// <summary>
	/// Adds a new entry to the overview
	/// </summary>
	/// <param name="transportRoute"></param>
	/// <returns></returns>
	public TransportRouteOverviewElement Add(TransportRoute transportRoute)
	{
		TransportRouteOverviewElement transportRouteOverviewView = GameObject.Instantiate(_overviewElementPrefab, _routeOverviewScrollView);
		transportRouteOverviewView.TransportRoute = transportRoute;
		return transportRouteOverviewView;
	}

	/// <summary>
	/// Removes an old entry from the overview
	/// </summary>
	/// <param name="transportRoute">the transport route to be removed</param>
	/// <returns>true if the element was found. </returns>
	public bool Remove(TransportRoute transportRoute)
	{
		Debug.Log("Remove Overview Element");
		for (int i = 0; i < _routeOverviewScrollView.childCount; i++)
		{
			TransportRouteOverviewElement element = _routeOverviewScrollView.GetChild(i).gameObject.GetComponent<TransportRouteOverviewElement>();
			if (element.TransportRoute != transportRoute) continue;
			Destroy(_routeOverviewScrollView.GetChild(i).gameObject);
			return true;
		}
		return false;
	}
}
