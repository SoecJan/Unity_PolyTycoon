using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(CityPlaceable))]
public class CityGrowthBehaviour : MonoBehaviour
{
    [SerializeField] private int seed = 0;
    [SerializeField] private int maxCityProgress = 20;
    [SerializeField] private int currentCityProgress = 0;
    [SerializeField] private Vector2Int _streetLengthMinMax = new Vector2Int(2, 6);
    [SerializeField] private float buildingProbability = 0.6f;
    private int startCitySize;
    
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
        List<Street> endpoints = new List<Street>();
        Street startStreet = PlaceStreetAt(transform.position);
        endpoints.Add(startStreet);
        System.Random random = new System.Random(seed);
        startCitySize = random.Next(2, 4);

        while (currentCityProgress < maxCityProgress)
        {
            yield return new WaitUntil(IsGrowing);
            if (endpoints.Count == 0) break;
            Street street = (Street) endpoints[0];
            SimpleMapPlaceable streetPlaceable = street.GetComponent<SimpleMapPlaceable>();
            endpoints.RemoveAt(0);
            for (int i = 0; i < street.NeighborNodes.Length; i++)
            {
                if (street.NeighborNodes[i]) continue;
                
                Vector3 directionVec = Util.DirectionIntToVector(i);
                Vector3 directionClockwiseVec = Util.DirectionIntToVector((i + 1) % 4);
                Vector3 directionCounterClockwiseVec = Util.DirectionIntToVector((i + 3) % 4);
                
                int maxStreetLengthRand = random.Next(streetLengthMinMax.x, streetLengthMinMax.y);
                for (int currStreetLength = 1; currStreetLength < maxStreetLengthRand+1; currStreetLength++)
                {
                    Vector3 placedPosition = street.transform.position + (directionVec * currStreetLength);
                    
                    if (IsRasterizing(placedPosition) || !_placementController.IsPlaceable(placedPosition, streetPlaceable.UsedCoordinates)) break;
                    Street streetAtCurrPos = PlaceStreetAt(placedPosition);

                    if (currStreetLength < maxStreetLengthRand)
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
                            simpleMapPlaceable = building.AddComponent<SimpleMapPlaceable>();
                            simpleMapPlaceable.UsedCoordinates = new List<NeededSpace>() {NeededSpace.Zero(TerrainGenerator.TerrainType.Flatland)};
                            
                            CityMainBuilding cityMainBuilding = building.AddComponent<CityMainBuilding>();
                            cityMainBuilding.CityPlaceable = _cityPlaceable;
                            _cityPlaceable.MainBuilding = cityMainBuilding;
                        }
                        else
                        {
                            ProceduralCityBuilding cityBuilding = building.AddComponent<ProceduralCityBuilding>();
                            cityBuilding.CityPlaceable = _cityPlaceable;
                            cityBuilding.Height = 1f;
                            cityBuilding.Generate(random.Next(1, maxStreetLengthRand - currStreetLength), directionVec, streetDirection, _placementController);
                            simpleMapPlaceable = cityBuilding;
                        }

                        if (!_placementController.PlaceObject(simpleMapPlaceable))
                        {
                            Destroy(simpleMapPlaceable.gameObject);
                            continue;
                        };
                        currentCityProgress++;
                        _cityPlaceable.ChildMapPlaceables.Add(simpleMapPlaceable);
                        building.transform.SetParent(transform);
                    }
                    
                    if (currStreetLength != maxStreetLengthRand) continue;
                    endpoints.Add(streetAtCurrPos);
                    break;
                }
            }
        }
    }

    private bool IsGrowing()
    {
        return currentCityProgress <= startCitySize + (Mathf.Pow(_cityPlaceable.Level, 2));
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
    
    private Street PlaceStreetAt(Vector3 position)
    {
        GameObject streetObject = Instantiate(_streetData.Prefab);
        Street street = streetObject.GetComponent<Street>();
        SimpleMapPlaceable simpleMapPlaceable = streetObject.GetComponent<SimpleMapPlaceable>();
        streetObject.transform.position = position + Vector3.up;
        // simpleMapPlaceable.OnPlacement();
        _placementController.PlaceObject(simpleMapPlaceable);
        _cityPlaceable.ChildMapPlaceables.Add(simpleMapPlaceable);
        streetObject.transform.SetParent(transform);
        return street;
    }
}