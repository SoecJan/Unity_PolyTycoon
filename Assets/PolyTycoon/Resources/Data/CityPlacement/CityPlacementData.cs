using UnityEngine;

/// <summary>
/// Defines all needed references to Objects needed by the CityGenerator
/// </summary>
[CreateAssetMenu(fileName = "CityPlacementData", menuName = "PolyTycoon/CityPlacement", order = 1)]
public class CityPlacementData : ScriptableObject
{
	#region Attributes
	[SerializeField] private Vector2[] _cityVec2;
	[SerializeField] private GameObject _streetPrefab;
	[SerializeField] private GameObject _mainBuildingPrefab;
	[SerializeField] private GameObject _citizenBuidlingPrefab;
	[SerializeField] private GameObject _businessBuildingPrefab;
	#endregion

	#region Getter & Setter
	public Vector2[] CityVec2 {
		get {
			return _cityVec2;
		}

		set {
			_cityVec2 = value;
		}
	}

	public GameObject StreetPrefab {
		get {
			return _streetPrefab;
		}

		set {
			_streetPrefab = value;
		}
	}

	public GameObject MainBuildingPrefab {
		get {
			return _mainBuildingPrefab;
		}

		set {
			_mainBuildingPrefab = value;
		}
	}

	public GameObject CitizenBuidlingPrefab {
		get {
			return _citizenBuidlingPrefab;
		}

		set {
			_citizenBuidlingPrefab = value;
		}
	}

	public GameObject BusinessBuildingPrefab {
		get {
			return _businessBuildingPrefab;
		}

		set {
			_businessBuildingPrefab = value;
		}
	}
	#endregion
}
