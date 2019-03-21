using System.Collections.Generic;
using UnityEngine;

namespace Assets.PolyTycoon.Scripts.Construction.Model.Placement
{
	/// <summary>
	/// This class handles MapPlaceables that contain more than one <see cref="SimpleMapPlaceable"/>.
	/// </summary>
	public abstract class ComplexMapPlaceable : MonoBehaviour {

		#region Attributes
		[SerializeField]
		private List<SimpleMapPlaceable> _childMapPlaceables;
		#endregion

		#region Getter & Setter
		public List<SimpleMapPlaceable> ChildMapPlaceables {
			get {
				return _childMapPlaceables;
			}
		}
		#endregion

		#region Default Methods
		// Use this for initialization
		void Awake () {
			if (_childMapPlaceables == null)
				_childMapPlaceables = new List<SimpleMapPlaceable>();
			Initialize();
		}

		protected virtual void Initialize()
		{}

		#endregion
	}
}
