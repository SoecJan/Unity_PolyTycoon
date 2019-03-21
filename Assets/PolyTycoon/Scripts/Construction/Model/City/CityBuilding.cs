using System.Collections.Generic;
using Assets.PolyTycoon.Resources.Data.ProductData;
using Assets.PolyTycoon.Scripts.Construction.Model.Placement;
using UnityEngine;

namespace Assets.PolyTycoon.Scripts.Construction.Model.City
{
	/// <summary>
	/// Class of any Building that can be registered at a <see cref="CityPlaceable"/> object.
	/// </summary>
	public class CityBuilding : PathFindingNode
	{
		#region Attributes
		private int _buildingLevel = 1;
		private int _currentResidentCount = 3;
		private int _maxResidentCount = 5;
		[SerializeField] private List<NeededProduct> _consumedProducts;

		[SerializeField]
		private CityPlaceable _cityPlaceable;
		private static BuildingManager _buildingManager; // Used to search for connected streets to this object
		private Street.Street _connectedStreet;
		#endregion

		#region Getter & Setter
		public Street.Street ConnectedStreet {
			get {
				return _connectedStreet;
			}

			set {
				_connectedStreet = value;
			}
		}

		public int BuildingLevel {
			get {
				return _buildingLevel;
			}

			set {
				_buildingLevel = value;
			}
		}

		public int CurrentResidentCount {
			get {
				return _currentResidentCount;
			}

			set {
				_currentResidentCount = value;
			}
		}

		public int MaxResidentCount {
			get {
				return _maxResidentCount;
			}

			set {
				_maxResidentCount = value;
			}
		}

		public CityPlaceable CityPlaceable {
			get {
				return _cityPlaceable;
			}

			set {
				_cityPlaceable = value;
			}
		}

		public List<NeededProduct> ConsumedProducts {
			get {
				return _consumedProducts;
			}

			set {
				_consumedProducts = value;
			}
		}
		#endregion

		#region Methods

		public override bool IsTraversable()
		{
			return false;
		}

		public override bool IsNode()
		{
			return true;
		}

		protected override void Initialize()
		{
			IsClickable = true;
			if (BuildingManager == null) BuildingManager = FindObjectOfType<GroundPlacementController>().BuildingManager;
			if (!CityPlaceable) CityPlaceable = transform.parent.gameObject.GetComponent<CityPlaceable>();
		}

		public override void OnPlacement()
		{
			base.OnPlacement();
			FindConnectedStreet();
		}

		/// <summary>
		/// Checks for any connected street on this object.
		/// </summary>
		void FindConnectedStreet()
		{
			// Find all neighbor streets
			SimpleMapPlaceable mapPlaceableTop = BuildingManager.GetMapPlaceable(gameObject.transform.position + Vector3.forward);
			if (mapPlaceableTop is Street.Street)
			{
				ConnectedStreet = (Street.Street)mapPlaceableTop;
				return;
			}
			SimpleMapPlaceable mapPlaceableRight = BuildingManager.GetMapPlaceable(gameObject.transform.position + Vector3.right);
			if (mapPlaceableRight is Street.Street)
			{
				ConnectedStreet = (Street.Street)mapPlaceableRight;
				return;
			}
			SimpleMapPlaceable mapPlaceableBottom = BuildingManager.GetMapPlaceable(gameObject.transform.position + Vector3.back);
			if (mapPlaceableBottom is Street.Street)
			{
				ConnectedStreet = (Street.Street)mapPlaceableBottom;
				return;
			}
			SimpleMapPlaceable mapPlaceableLeft = BuildingManager.GetMapPlaceable(gameObject.transform.position + Vector3.left);
			if (mapPlaceableLeft is Street.Street)
			{
				ConnectedStreet = (Street.Street)mapPlaceableLeft;
				return;
			}
		}
		#endregion
	}
}