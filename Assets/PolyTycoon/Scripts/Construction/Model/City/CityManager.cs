using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// The city manager holds a reference to all cities.
/// </summary>
public interface ICityManager
{
    /// <summary>
    /// This function returns the <see cref="CityPlaceable"/> reference to a given city name string.
    /// </summary>
    /// <param name="cityName">The name of the <see cref="CityPlaceable"/> that needs to be returned.</param>
    /// <returns>The <see cref="CityPlaceable"/> instance associated with the given name</returns>
    CityPlaceable GetCity(string cityName);
}

public class CityManager : MonoBehaviour, ICityManager
{
    [SerializeField] private CityWorldToScreenUi _cityWorldToScreenUi;
    [SerializeField] private List<CityPlaceable> _possibleCityPlaceables;
    private List<CityPlaceable> _placedCities;
    private PlacementManager _placementManager;
    private WorldToScreenUiManager _worldToScreenUiManager;


    // Use this for initialization
    void Start()
    {
        _placedCities = new List<CityPlaceable>();
        _placementManager = FindObjectOfType<PlacementManager>();
        _worldToScreenUiManager = FindObjectOfType<WorldToScreenUiManager>();
        AddRandomCity();
    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.K))
//        {
//            AddRandomCity(); // Debugging
//        }
//    }

    private void AddRandomCity()
    {
        AddCity(_possibleCityPlaceables[Random.Range(0, _possibleCityPlaceables.Count)], new Vector3(0.5f, 0f, 0.5f));
        AddCity(_possibleCityPlaceables[Random.Range(0, _possibleCityPlaceables.Count)], new Vector3(45.5f, 0f, 0.5f));
        AddCity(_possibleCityPlaceables[Random.Range(0, _possibleCityPlaceables.Count)], new Vector3(-45.5f, 0f, 0.5f));
        AddCity(_possibleCityPlaceables[Random.Range(0, _possibleCityPlaceables.Count)], new Vector3(0.5f, 0f, 45.5f));
        AddCity(_possibleCityPlaceables[Random.Range(0, _possibleCityPlaceables.Count)], new Vector3(0.5f, 0f, -45.5f));
    }

    public CityPlaceable GetCity(string cityName)
    {
        foreach (CityPlaceable cityPlaceable in _placedCities)
        {
            if (cityPlaceable.transform.name.ToLower().Equals(cityName.ToLower()))
            {
                return cityPlaceable;
            }
        }

        return null;
    }

    private void AddCity(CityPlaceable cityPlaceable, Vector3 offset)
    {
        CityToPlace cityToPlace = new CityToPlace(cityPlaceable, offset);
        ThreadedDataRequester.RequestData(() => PlacePendingCity(cityToPlace), OnPlacementPositionFound);
    }

    private void OnPlacementPositionFound(object cityToPlaceObject)
    {
        CityToPlace cityToPlace = (CityToPlace) cityToPlaceObject;
        CityPlaceable city = Instantiate(cityToPlace.CityPlaceable, cityToPlace.Position, Quaternion.identity);
        if (!_placementManager.PlaceObject(city))
        {
            Destroy(city.gameObject);
            return;
        }
        
        _placedCities.Add(city);
            
        WorldToScreenUiManager.WorldUiElement uiGameObject = _worldToScreenUiManager.Add(
            _cityWorldToScreenUi.gameObject,
            city.gameObject.transform, new Vector3(0, 50f, 0));
        CityWorldToScreenUi worldToScreenUi = uiGameObject.UiTransform.gameObject.GetComponent<CityWorldToScreenUi>();
        worldToScreenUi.City = city;
    }

    CityToPlace PlacePendingCity(CityToPlace cityToPlace)
    {
        while (!_placementManager.TerrainGenerator.IsReady())
        {
            Thread.Sleep(500);
        }

        return MoveToPlaceablePosition(cityToPlace);
    }

    private CityToPlace MoveToPlaceablePosition(CityToPlace cityToPlace)
    {
        int stepSize = 2;
        int targetXMove = 0;
        int targetYMove = 0;
        int currentXMove = 0;
        int currentYMove = 0;
        int direction = -1;

        // Moves the cityPlaceable in a growing square around the starting position until a suitable location is found
        while (!_placementManager.IsPlaceable(cityToPlace.Position, cityToPlace.CityPlaceableNeededSpaces))
        {
            // Move one step at a time
            if (currentXMove > 0)
            {
                cityToPlace.Translate(direction * stepSize, 0, 0);
                currentXMove--;
                continue;
            }

            if (currentYMove > 0)
            {
                cityToPlace.Translate(0, 0, direction * stepSize);
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
        }

        return cityToPlace;
    }

    private struct CityToPlace
    {
        private Vector3 _position;
        private List<NeededSpace> _cityPlaceableNeededSpaces;

        public CityToPlace(CityPlaceable cityPlaceable, Vector3 offset)
        {
            CityPlaceable = cityPlaceable;
            _cityPlaceableNeededSpaces = new List<NeededSpace>();
            foreach (SimpleMapPlaceable childMapPlaceable in cityPlaceable.ChildMapPlaceables)
            {
                foreach (NeededSpace neededSpace in childMapPlaceable.UsedCoordinates)
                {
                    _cityPlaceableNeededSpaces.Add(new NeededSpace(neededSpace, Vector3Int.FloorToInt(childMapPlaceable.transform.localPosition)));
                }
            }
            _position = offset;
        }

        public List<NeededSpace> CityPlaceableNeededSpaces => _cityPlaceableNeededSpaces;

        public Vector3 Position => _position;

        public CityPlaceable CityPlaceable { get; private set; }

        public void Translate(float x, float y, float z)
        {
            _position += new Vector3(x, y, z);
        }
    }
}