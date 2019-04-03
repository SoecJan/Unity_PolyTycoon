using System.Collections.Generic;


	public class TransportRoute
	{
		#region Attributes
		private string _routeName;
		private List<TransportRouteElement> _transportRouteElements;
		private TransportVehicle _vehicle;
		#endregion

		#region Constructor
		public TransportRoute()
		{
			_transportRouteElements = new List<TransportRouteElement>();
		}
		#endregion

		#region Getter & Setter

		public TransportVehicle Vehicle {
			get {
				return _vehicle;
			}

			set {
				_vehicle = value;
			}
		}

		public List<TransportRouteElement> TransportRouteElements {
			get {
				return _transportRouteElements;
			}

			set {
				_transportRouteElements = value;
			}
		}

		public string RouteName {
			get {
				return _routeName;
			}

			set {
				_routeName = value;
			}
		}

		public int Distance()
		{
			int sum = 0;
			foreach (TransportRouteElement element in TransportRouteElements)
			{
				sum += element.Path.WayPoints.Count;
			}
			return sum;
		}
		#endregion
	}