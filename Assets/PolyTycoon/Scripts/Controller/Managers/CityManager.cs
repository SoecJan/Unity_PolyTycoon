using System.Collections.Generic;
using UnityEngine;

public class CityManager : ICityManager
{
    private System.Random _random;
    private List<CityPlaceable> _possibleCityPlaceables;
    private List<CityPlaceable> _placedCities;
    private IPlacementController _placementController;

    private CityWorldToScreenView cityWorldToScreenView;
    private IWorldToScreenManager _worldToScreenManager;

    private CityManager(int seed)
    {
        this._random = new System.Random(seed);
        _possibleCityPlaceables = new List<CityPlaceable>
        {
            Resources.Load<CityPlaceable>(Util.PathTo("ProceduralCity")),
        };
        _placedCities = new List<CityPlaceable>();
        cityWorldToScreenView = Resources.Load<CityWorldToScreenView>(Util.PathTo("CityWorldToScreenUi"));
    }

    public CityManager(IPlacementController placementController, int seed) : this(seed)
    {
        _placementController = placementController;
        _worldToScreenManager = GameObject.FindObjectOfType<WorldToScreenManager>();
    }

    public CityManager(IPlacementController placementController, WorldToScreenManager worldToScreenManager, int seed) : this(seed)
    {
        this._placementController = placementController;
        this._worldToScreenManager = worldToScreenManager;
    }

    public CityPlaceable GetRandomCityPrefab()
    {
        return _possibleCityPlaceables[_random.Next(0, _possibleCityPlaceables.Count)];
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

    public void OnPlacementPositionFound(object cityToPlaceObject)
    {
        ThreadsafePlaceable cityToPlace = (ThreadsafePlaceable) cityToPlaceObject;
        CityPlaceable city = GameObject.Instantiate((CityPlaceable) cityToPlace.MapPlaceable, cityToPlace.Position,
            Quaternion.identity);
        Debug.Log("City placed at: " + cityToPlace.Position);
        if (!_placementController.PlaceObject(city))
        {
            GameObject.Destroy(city.gameObject);
            return;
        }

        _placedCities.Add(city);

        WorldToScreenElement uiGameObject = _worldToScreenManager.Add(
            cityWorldToScreenView.gameObject,
            city.gameObject.transform, new Vector3(0, 50f, 0));
        CityWorldToScreenView worldToScreenView =
            uiGameObject.UiTransform.gameObject.GetComponent<CityWorldToScreenView>();
        worldToScreenView.City = city;
    }
}