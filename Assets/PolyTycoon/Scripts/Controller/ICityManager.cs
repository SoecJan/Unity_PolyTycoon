/// <summary>
/// The city manager holds a reference to all cities.
/// </summary>
public interface ICityManager
{
    void OnPlacementPositionFound(object threadsafePlaceable);
    CityPlaceable GetRandomCityPrefab();
    /// <summary>
    /// This function returns the <see cref="CityPlaceable"/> reference to a given city name string.
    /// </summary>
    /// <param name="cityName">The name of the <see cref="CityPlaceable"/> that needs to be returned.</param>
    /// <returns>The <see cref="CityPlaceable"/> instance associated with the given name</returns>
    CityPlaceable GetCity(string cityName);
}