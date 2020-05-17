using System.Collections.Generic;
using UnityEngine;

public class CityManager : ICityManager
{
    private List<CityPlaceable> _possibleCityPlaceables;
    private List<CityPlaceable> _placedCities;
    private IPlacementController _placementController;

    private CityWorldToScreenView cityWorldToScreenView;
    private IWorldToScreenManager _worldToScreenManager;

    private CityManager()
    {
        _possibleCityPlaceables = new List<CityPlaceable>
        {
            Resources.Load<CityPlaceable>(PathUtil.Get("City1-1_0")),
            Resources.Load<CityPlaceable>(PathUtil.Get("City1-1_1")),
            Resources.Load<CityPlaceable>(PathUtil.Get("City2-1"))
        };
        _placedCities = new List<CityPlaceable>();
        cityWorldToScreenView = Resources.Load<CityWorldToScreenView>(PathUtil.Get("CityWorldToScreenUi"));
    }

    public CityManager(IPlacementController placementController) : this()
    {
        _placementController = placementController;
        _worldToScreenManager = GameObject.FindObjectOfType<WorldToScreenManager>();
    }

    public CityManager(IPlacementController placementController, WorldToScreenManager worldToScreenManager) : this()
    {
        this._placementController = placementController;
        this._worldToScreenManager = worldToScreenManager;
    }

    public CityPlaceable GetRandomCityPrefab()
    {
        return _possibleCityPlaceables[Random.Range(0, _possibleCityPlaceables.Count)];
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