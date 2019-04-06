using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransportRouteElementView : MonoBehaviour
{
	private TransportRouteElement _transportRouteElement;

	[SerializeField] private Text _fromText;
	[SerializeField] private Button _selectButton;


	public PathFindingNode FromNode
	{
		get { return _transportRouteElement.FromNode; }
		set
		{
			if (_transportRouteElement == null) _transportRouteElement = new TransportRouteElement();
			_transportRouteElement.FromNode = value;
			_fromText.text = value.BuildingName;
		}
	}

	public PathFindingNode ToNode
	{
		get { return _transportRouteElement.ToNode; }
		set
		{
			_transportRouteElement.ToNode = value;
		}
	}

	public TransportRouteElement RouteElement
	{
		get { return _transportRouteElement; }
		set
		{
			_transportRouteElement = value;
			_fromText.text = value.FromNode.BuildingName;
			ToNode = value.ToNode;
		}
	}

	public Button SelectButton
	{
		get { return _selectButton; }
		set { _selectButton = value; }
	}

	void Awake()
	{
		if (_transportRouteElement == null) _transportRouteElement = new TransportRouteElement();
		_transportRouteElement.RouteSettings = new List<TransportRouteSetting>();
	}
}
