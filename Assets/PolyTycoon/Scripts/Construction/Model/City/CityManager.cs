using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityManager : MonoBehaviour
{

	[SerializeField] private CityWorldToScreenUi _cityWorldToScreenUi;
	[SerializeField] private List<CityPlaceable> _possibleCityPlaceables;
	private List<CityPlaceable> _placedCities;
	private List<CityToPlace> _citiesToPlace;
	private GroundPlacementController _groundPlacementController;
	private WorldToScreenUiManager _worldToScreenUiManager;
	

	// Use this for initialization
	void Start ()
	{
		_placedCities = new List<CityPlaceable>();
		_groundPlacementController = FindObjectOfType<GroundPlacementController>();
		_worldToScreenUiManager = FindObjectOfType<WorldToScreenUiManager>();
		_citiesToPlace = new List<CityToPlace>();
		StartCoroutine(PlacePendingCity());
		AddRandomCity();
	}

	private void AddRandomCity()
	{
		AddCity(_possibleCityPlaceables[Random.Range(0, _possibleCityPlaceables.Count)], Vector2Int.zero);
	}

	public CityPlaceable GetCity(string cityName)
	{
		foreach (CityPlaceable cityPlaceable in _placedCities)
		{
			if (cityPlaceable.BuildingName.ToLower().Equals(cityName.ToLower()))
			{
				return cityPlaceable;
			}
		}
		return null;
	}

	private void AddCity(CityPlaceable cityPlaceable, Vector2Int offset)
	{
		_citiesToPlace.Add(new CityToPlace(cityPlaceable, offset));
	}

	private IEnumerator PlacePendingCity()
	{
		while (!_groundPlacementController.TerrainGenerator.IsReady())
		{ yield return new WaitForSeconds(0.1f); }

		while (true)
		{
			if (_citiesToPlace.Count > 0)
			{
				CityToPlace cityToPlace = _citiesToPlace[0];
				_citiesToPlace.Remove(cityToPlace);

				CityPlaceable cityPlaceable = cityToPlace.CityPlaceable;
				GameObject cityInstance = Instantiate(cityPlaceable.gameObject);

				CityPlaceable city = cityInstance.GetComponent<CityPlaceable>();
				city.Translate(cityToPlace.Offset.x, 0, cityToPlace.Offset.y);
				if (_worldToScreenUiManager)
				{
					GameObject uiGameObject = _worldToScreenUiManager.Add(_cityWorldToScreenUi.gameObject, city.gameObject.transform, new Vector3(0,50f,0));
					CityWorldToScreenUi worldToScreenUi = uiGameObject.GetComponent<CityWorldToScreenUi>();
					worldToScreenUi.Text.text = "".Equals(city.BuildingName) || city.BuildingName == null ? "Default Name" : city.BuildingName;
				}
					

				yield return Move(city);
			}
			yield return new WaitForSeconds(1);
		}
	}

	private IEnumerator Move(CityPlaceable cityPlaceable)
	{
		int x = 0;
		int z = 0;
		
		int targetXMove = 0;
		int targetYMove = 0;
		int currentXMove = 0;
		int currentYMove = 0;
		int direction = -1;

		// Moves the cityPlaceable in a growing square around the starting position until a suitable location is found
		while (!_groundPlacementController.PlaceObject(cityPlaceable))
		{
			// Move one step at a time
			if (currentXMove > 0)
			{
				cityPlaceable.Translate(direction, 0, 0);
				x += direction;
				currentXMove--;
				continue;
			}
			if (currentYMove > 0)
			{
				cityPlaceable.Translate(0, 0, direction);
				z += direction;
				currentYMove--;
				continue;
			}

			// Control step Amount
			if (targetXMove == targetYMove)
			{
				direction = -direction;
				targetXMove++;
				currentXMove = targetXMove;
			}
			else
			{
				targetYMove = targetXMove;
				currentYMove = targetYMove;
			}

			yield return null;
		}
	}

	private struct CityToPlace
	{
		private CityPlaceable _cityPlaceable;
		private Vector2Int _offset;

		public CityToPlace(CityPlaceable cityPlaceable, Vector2Int offset)
		{
			_cityPlaceable = cityPlaceable;
			_offset = offset;
		}

		public CityPlaceable CityPlaceable {
			get {
				return _cityPlaceable;
			}
		}

		public Vector2Int Offset {
			get {
				return _offset;
			}
		}
	}
}
