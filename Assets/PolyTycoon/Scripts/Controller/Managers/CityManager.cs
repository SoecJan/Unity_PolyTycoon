using System.Collections.Generic;
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

public class CityManager : ICityManager
{
    private CityWorldToScreenView cityWorldToScreenView;
    private List<CityPlaceable> _possibleCityPlaceables;
    private List<CityPlaceable> _placedCities;
    private PlacementController _placementController;
    private WorldToScreenManager _worldToScreenManager;
    
    public CityManager(PlacementController placementController)
    {
        _possibleCityPlaceables = new List<CityPlaceable>
        {
            Resources.Load<CityPlaceable>(PathUtil.Get("City1-1_0")),
            Resources.Load<CityPlaceable>(PathUtil.Get("City1-1_1")),
            Resources.Load<CityPlaceable>(PathUtil.Get("City2-1"))
        };
        _placedCities = new List<CityPlaceable>();
        _placementController = placementController;
        cityWorldToScreenView = Resources.Load<CityWorldToScreenView>(PathUtil.Get("CityWorldToScreenUi"));
        _worldToScreenManager = GameObject.FindObjectOfType<WorldToScreenManager>();
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
        CityPlaceable city = GameObject.Instantiate((CityPlaceable)cityToPlace.MapPlaceable, cityToPlace.Position, Quaternion.identity);
        if (!_placementController.PlaceObject(city))
        {
            GameObject.Destroy(city.gameObject);
            return;
        }
        
        _placedCities.Add(city);
            
        WorldToScreenElement uiGameObject = _worldToScreenManager.Add(
            cityWorldToScreenView.gameObject,
            city.gameObject.transform, new Vector3(0, 50f, 0));
        CityWorldToScreenView worldToScreenView = uiGameObject.UiTransform.gameObject.GetComponent<CityWorldToScreenView>();
        worldToScreenView.City = city;
    }
}