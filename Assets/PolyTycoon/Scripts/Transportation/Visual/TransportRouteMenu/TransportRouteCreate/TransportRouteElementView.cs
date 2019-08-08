using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransportRouteElementView : MonoBehaviour
{
	private TransportRouteElement _transportRouteElement;
	[SerializeField] private TextMeshProUGUI _fromText;
	[SerializeField] private Button _selectButton;
	[SerializeField] private Button _deleteButton;

	public PathFindingNode FromNode
	{
		get => _transportRouteElement.FromNode;
		set
		{
			if (_transportRouteElement == null) _transportRouteElement = new TransportRouteElement();
			_transportRouteElement.FromNode = value;
			_fromText.text = value.BuildingName;
		}
	}

	public PathFindingNode ToNode
	{
		get => _transportRouteElement.ToNode;
		set => _transportRouteElement.ToNode = value;
	}

	public TransportRouteElement RouteElement
	{
		get => _transportRouteElement;
		set
		{
			_transportRouteElement = value;
			_fromText.text = value.FromNode.BuildingName;
			ToNode = value.ToNode;
		}
	}

	public Button SelectButton => _selectButton;

	void Awake()
	{
		if (_transportRouteElement == null) _transportRouteElement = new TransportRouteElement();
		_transportRouteElement.RouteSettings = new List<TransportRouteSetting>();
		_deleteButton.onClick.AddListener(delegate
		{
			TransportRouteCreateController transportRouteCreateController = FindObjectOfType<TransportRouteCreateController>();
			transportRouteCreateController.StationManager.RemoveTransportRouteElement(this);
		});
	}
}
