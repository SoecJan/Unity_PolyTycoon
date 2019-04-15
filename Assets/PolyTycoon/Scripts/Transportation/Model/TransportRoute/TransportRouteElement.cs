using System.Collections.Generic;

public class TransportRouteElement
{
	#region Attributes
	private List<TransportRouteSetting> _routeSettings;
	private Path _path;
	private PathFindingNode _fromNode;
	private PathFindingNode _toNode;
	#endregion

	#region Constructor
	public TransportRouteElement()
	{
		_routeSettings = new List<TransportRouteSetting>();
	}

	#endregion

	#region Getter & Setter

	public Path Path {
		get {
			return _path;
		}

		set {
			_path = value;
		}
	}

	public PathFindingNode FromNode {
		get {
			return _fromNode;
		}

		set {
			_fromNode = value;
		}
	}

	public PathFindingNode ToNode {
		get {
			return _toNode;
		}

		set {
			_toNode = value;
		}
	}

	public List<TransportRouteSetting> RouteSettings {
		get {
			return _routeSettings;
		}

		set {
			_routeSettings = value;
		}
	}
	#endregion
}
