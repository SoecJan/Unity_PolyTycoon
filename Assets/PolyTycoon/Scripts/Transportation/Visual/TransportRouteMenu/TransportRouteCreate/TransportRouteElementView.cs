using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransportRouteElementView : MonoBehaviour
{
	private TransportRouteElement _transportRouteElement;

	[SerializeField] private Text _fromText;
	[SerializeField] private Text _toText;
	[SerializeField] private Button _selectButton;


	public PathFindingNode FromNode
	{
		get { return _transportRouteElement.FromNode; }
		set
		{
			_transportRouteElement.FromNode = value;
			_fromText.text = "From " + value.BuildingName;
		}
	}

	public PathFindingNode ToNode
	{
		get { return _transportRouteElement.ToNode; }
		set
		{
			_transportRouteElement.ToNode = value;
			_toText.text = "To " + value.BuildingName;
		}
	}

	public TransportRouteElement RouteElement
	{
		get { return _transportRouteElement; }
		set { _transportRouteElement = value; }
	}

	public Button SelectButton
	{
		get { return _selectButton; }
		set { _selectButton = value; }
	}

	void Awake()
	{
		_transportRouteElement = new TransportRouteElement();
		_transportRouteElement.RouteSettings = new List<TransportRouteSetting>();
	}
}
