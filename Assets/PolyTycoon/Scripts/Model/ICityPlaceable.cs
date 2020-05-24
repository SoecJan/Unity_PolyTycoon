using UnityEngine;

/// <summary>
/// This interface describes all functionality a city has. 
/// </summary>
public interface ICityPlaceable
{
    /// <summary>
    /// The main buidling of this city. It is the target of pathfinding for this city. 
    /// </summary>
    CityMainBuilding MainBuilding { get; set; }

    /// <summary>
    /// Count of people living in this cityPlaceable.
    /// </summary>
    /// <returns>Number of inhabitants</returns>
    int CurrentInhabitantCount();

    /// <summary>
    /// Rotates the city. Useful for bringing variation into city generation.
    /// </summary>
    /// <param name="axis">The axis of rotation. Vector3.up for terrain aligned rotation.</param>
    /// <param name="rotationAmount">The amount of rotation. Input of 90f = 90° of rotation.</param>
    void Rotate(Vector3 axis, float rotationAmount);
}