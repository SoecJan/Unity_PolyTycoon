using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransportRouteElementView : MonoBehaviour
{
	private TransportRouteElement _transportRouteElement;
	[SerializeField] private TextMeshProUGUI _fromText;
	[SerializeField] private Toggle _selectToggle;
	[SerializeField] private Button _deleteButton;

	public PathFindingNode FromNode
	{
		get => _transportRouteElement.FromNode;
		set
		{
			if (_transportRouteElement == null) _transportRouteElement = new TransportRouteElement();
			_transportRouteElement.FromNode = value;
			if (value is ICityBuilding cityBuilding)
			{
				_fromText.text = cityBuilding.CityPlaceable().name;
			}
			else
			{
				_fromText.text = value.name;
			}
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
			_fromText.text = value.FromNode.name;
			ToNode = value.ToNode;
		}
	}

	public Toggle SelectToggle => _selectToggle;

	public Button DeleteButton => _deleteButton;

	void Awake()
	{
		if (_transportRouteElement == null) _transportRouteElement = new TransportRouteElement();
		_transportRouteElement.RouteSettings = new List<TransportRouteSetting>();
		DeleteButton.onClick.AddListener(delegate
		{
			TransportRouteCreationView transportRouteCreationView = FindObjectOfType<TransportRouteCreationView>();
			transportRouteCreationView.StationManager.RemoveTransportRouteElement(this);
		});
	}
}
