using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CityPlaceable))]
public class CityGrowthBehaviour : MonoBehaviour
{
    [SerializeField] private int seed = 0;
    [SerializeField] private int maxCityProgress = 100;
    [SerializeField] private Vector2Int _streetLengthMinMax = new Vector2Int(2, 6);
    [SerializeField] private float buildingProbability = 0.6f;
    
    private CityPlaceable _cityPlaceable;
    private PlacementController _placementController;
    private BuildingManager _buildingManager;
    private BuildingData _streetData;
    private Coroutine generationRoutine;
    
    // Start is called before the first frame update
    void Start()
    {
        _cityPlaceable = GetComponent<CityPlaceable>();
        
        GameHandler gameHandler = FindObjectOfType<GameHandler>();
        _placementController = (PlacementController) gameHandler.PlacementController;
        _buildingManager = (BuildingManager) gameHandler.BuildingManager;
        
        _streetData = Resources.Load<BuildingData>(Util.PathTo("Street"));
        generationRoutine = StartCoroutine(GenerateCity(seed, maxCityProgress, _streetLengthMinMax, buildingProbability));
    }

    private IEnumerator GenerateCity(int seed, int maxCityProgress, Vector2Int streetLengthMinMax, float buildingProbability)
    {
        List<SimpleMapPlaceable> endpoints = new List<SimpleMapPlaceable>();
        SimpleMapPlaceable mapPlaceable = GetComponentInChildren<SimpleMapPlaceable>();
        endpoints.Add(mapPlaceable);
        System.Random random = new System.Random(seed);

        float waitTime = 0.0f;
        int progress = 0;
        while (progress < maxCityProgress)
        {
            progress++;
            if (endpoints.Count == 0) break;
            Street street = (Street) endpoints[0];
            endpoints.RemoveAt(0);
            for (int i = 0; i < street.NeighborNodes.Length; i++)
            {
                if (street.NeighborNodes[i]) continue;
                
                Vector3 directionVec = Util.DirectionIntToVector(i);
                Vector3 directionClockwiseVec =
                    Util.DirectionIntToVector(Util.Mod(i + 1,
                        PathFindingNode.TotalNodeCount));
                Vector3 directionCounterClockwiseVec = Util.DirectionIntToVector(
                    Util.Mod(i-1, PathFindingNode.TotalNodeCount));
                
                int amount = random.Next(streetLengthMinMax.x, streetLengthMinMax.y);
                for (int j = 1; j < amount+1; j++)
                {
                    Vector3 placedPosition = street.transform.position + (directionVec * j);
                    
                    if (IsRasterizing(placedPosition) || !_placementController.IsPlaceable(placedPosition, street.UsedCoordinates)) break;
                    SimpleMapPlaceable placeable = PlaceStreetAt(placedPosition);
                    yield return new WaitForSeconds(waitTime);

                    if (j < amount)
                    {
                        if (random.NextDouble() > buildingProbability) continue;
                        
                        List<NeededSpace> neededSpaces = new List<NeededSpace>()
                        {
                            NeededSpace.Zero(TerrainGenerator.TerrainType.Flatland)
                        };
                        
                        bool isClockwisePlaceable = _placementController.IsPlaceable(placedPosition + directionClockwiseVec, neededSpaces);
                        bool isCounterClockwisePlaceable = _placementController.IsPlaceable(placedPosition + directionCounterClockwiseVec, neededSpaces);

                        Vector3 buildingStartPosition;
                        Vector3 streetDirection;
                        
                        if (isClockwisePlaceable)
                        {
                            buildingStartPosition = placedPosition + directionClockwiseVec;
                            streetDirection = directionCounterClockwiseVec;
                        } else if (isCounterClockwisePlaceable)
                        {
                            buildingStartPosition = placedPosition + directionCounterClockwiseVec;
                            streetDirection = directionClockwiseVec;
                        }
                        else
                        {
                            continue;
                        }
                        GameObject building = new GameObject("Procedural Building");
                        building.transform.position = buildingStartPosition;
                        SimpleMapPlaceable simpleMapPlaceable;
                        
                        if (!_cityPlaceable.MainBuilding)
                        {
                            CityMainBuilding cityMainBuilding = building.AddComponent<CityMainBuilding>();
                            cityMainBuilding.CityPlaceable = _cityPlaceable;
                            cityMainBuilding.UsedCoordinates = new List<NeededSpace>() {NeededSpace.Zero(TerrainGenerator.TerrainType.Flatland)};
                            _cityPlaceable.MainBuilding = cityMainBuilding;
                            simpleMapPlaceable = cityMainBuilding;
                        }
                        else
                        {
                            ProceduralCityBuilding cityBuilding = building.AddComponent<ProceduralCityBuilding>();
                            cityBuilding.CityPlaceable = _cityPlaceable;
                            cityBuilding.Height = 1f;
                            cityBuilding.Generate(random.Next(1, amount - j), directionVec, streetDirection, _placementController);
                            simpleMapPlaceable = cityBuilding;
                        }

                        _placementController.PlaceObject(simpleMapPlaceable);
                        _cityPlaceable.ChildMapPlaceables.Add(simpleMapPlaceable);
                        building.transform.SetParent(transform);
                    }
                    
                    if (j != amount) continue;
                    endpoints.Add(placeable);
                    break;
                }
                yield return new WaitForSeconds(waitTime);
            }
        }
    }

    private bool IsRasterizing(Vector3 position, int? kernelSize = 3, int? threshold = 3)
    {
        int count = 0;
        int kernelCenter = Mathf.FloorToInt((float) (kernelSize / 2f));
        for (int i = 0; i < kernelSize; i++)
        {
            for (int j = 0; j < kernelSize; j++)
            {
                int x = i - kernelCenter;
                int z = j - kernelCenter;
                
                if (x == 0 && z == 0) continue;
                
                Vector3 checkedPosition = new Vector3(position.x + x, position.y, position.z + z);
                if (!(_buildingManager.GetNode(checkedPosition) is Street)) continue;
                count++;
                if (count > threshold) return true;
            }
        }

        return false;
    }

    // private int CountNeighbors(Street street)
    // {
    //     int count = 0;
    //     foreach (PathFindingNode neighborNode in street.NeighborNodes)
    //     {
    //         if (!neighborNode) continue;
    //         count++;
    //     }
    //
    //     return count;
    // }
    
    private SimpleMapPlaceable PlaceStreetAt(Vector3 position)
    {
        GameObject gameObject = Instantiate(_streetData.Prefab);
        SimpleMapPlaceable simpleMapPlaceable = gameObject.GetComponent<SimpleMapPlaceable>();
        gameObject.transform.position = position;
        simpleMapPlaceable.OnPlacement();
        _placementController.PlaceObject(simpleMapPlaceable);
        _cityPlaceable.ChildMapPlaceables.Add(simpleMapPlaceable);
        gameObject.transform.SetParent(transform);
        return simpleMapPlaceable;
    }
}