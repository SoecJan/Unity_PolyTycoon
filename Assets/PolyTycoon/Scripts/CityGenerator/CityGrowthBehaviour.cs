using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityGrowthBehaviour : MonoBehaviour
{
    [SerializeField] private Vector3 offset = (Vector3.forward + Vector3.right)/2f;
    [SerializeField] private AnimationCurve _animationCurve;
    [SerializeField] private Vector2Int _size;
    [SerializeField] private Image _image;
    [SerializeField] private NoiseSettings _noiseSettings;
    [SerializeField] private Vector2Int _buildingBlockSize = Vector2Int.up;
    [SerializeField] private float _buildingScale = 1f;

    private BuildingManager _buildingManager;
    private BuildingData _streetData;
    private List<GameObject> cubes;
    private Coroutine generationRoutine;
    
    // Start is called before the first frame update
    void Start()
    {
        GameHandler gameHandler = GetComponent<GameHandler>();
        _buildingManager = (BuildingManager) (gameHandler ? gameHandler.BuildingManager : new BuildingManager());
        PathFindingNode.BuildingManager = _buildingManager;
        cubes = new List<GameObject>();
        _streetData = Resources.Load<BuildingData>(PathUtil.Get("Street"));
        generationRoutine = StartCoroutine(GenerateCity());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.K))
        {
            StopCoroutine(generationRoutine);
            Reset();
            generationRoutine = StartCoroutine(GenerateCity());
        }
    }

    private IEnumerator GenerateCity()
    {
        Vector2 position = Vector2.zero;
        List<SimpleMapPlaceable> endpoints = new List<SimpleMapPlaceable>();
        SimpleMapPlaceable mapPlaceable = PlaceStreetAt(new Vector3(position.x + offset.x, 0f, position.y + offset.z));
        endpoints.Add(mapPlaceable);
        System.Random random = new System.Random(_noiseSettings.seed);
        float[,] noise = Noise.GenerateNoiseMap(48, 48, _noiseSettings, position);
        this._image.sprite = GenerateImage(noise);

        float waitTime = 0.01f;
        int progress = 0;
        while (progress < 40)
        {
            progress++;
            noise = Noise.GenerateNoiseMap(1, 1, _noiseSettings, position);
            Street street = (Street) endpoints[0];
            endpoints.RemoveAt(0);
            for (int i = 0; i < street.NeighborNodes.Length; i++)
            {
                if (street.NeighborNodes[i]) continue;
                
                Vector3 directionVec = AbstractPathFindingAlgorithm.DirectionIntToVector(i);
                Vector3 directionClockwiseVec =
                    AbstractPathFindingAlgorithm.DirectionIntToVector(PathFindingNode.Mod(i + 1,
                        PathFindingNode.TotalNodeCount));
                Vector3 directionCounterClockwiseVec = AbstractPathFindingAlgorithm.DirectionIntToVector(
                    PathFindingNode.Mod(i-1, PathFindingNode.TotalNodeCount));
                
                int amount = random.Next(2, 6);
                for (int j = 1; j < amount+1; j++)
                {
                    Vector3 placedPosition = street.ThreadsafePosition + (directionVec * j);
                    
                    if (IsRasterizing(placedPosition) || !_buildingManager.IsPlaceable(placedPosition, street.UsedCoordinates)) break;
                    SimpleMapPlaceable placeable = PlaceStreetAt(placedPosition);
                    yield return new WaitForSeconds(waitTime);

                    if (j < amount)
                    {
                        if (random.NextDouble() > 0.6f) continue;
                        
                        List<NeededSpace> neededSpaces = new List<NeededSpace>()
                        {
                            NeededSpace.Zero(TerrainGenerator.TerrainType.Flatland)
                        };
                        
                        bool isClockwisePlaceable = _buildingManager.IsPlaceable(placedPosition + directionClockwiseVec, neededSpaces);
                        bool isCounterClockwisePlaceable = _buildingManager.IsPlaceable(placedPosition + directionCounterClockwiseVec, neededSpaces);

                        Vector3 buildingStartPosition;
                        
                        if (isClockwisePlaceable)
                        {
                            buildingStartPosition = placedPosition + directionClockwiseVec;
                        } else if (isCounterClockwisePlaceable)
                        {
                            buildingStartPosition = placedPosition + directionCounterClockwiseVec;
                        }
                        else
                        {
                            continue;
                        }
                        GameObject building = new GameObject("Procedural Building");
                        building.transform.position = buildingStartPosition;
                        ProceduralCityBuilding cityBuilding = building.AddComponent<ProceduralCityBuilding>();
                        cityBuilding.Height = 1f;
                        cityBuilding.Generate(random.Next(1, amount - j), directionVec);
                        cityBuilding.OnPlacement();
                        _buildingManager.AddMapPlaceable(cityBuilding);
                        cubes.Add(building);
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

    private int CountNeighbors(Street street)
    {
        int count = 0;
        foreach (PathFindingNode neighborNode in street.NeighborNodes)
        {
            if (!neighborNode) continue;
            count++;
        }

        return count;
    }
    
    private SimpleMapPlaceable PlaceStreetAt(Vector3 position)
    {
        GameObject gameObject = Instantiate(_streetData.Prefab);
        SimpleMapPlaceable simpleMapPlaceable = gameObject.GetComponent<SimpleMapPlaceable>();
        simpleMapPlaceable.ShowPlacementVisuals(false);
        gameObject.transform.position = position;
        simpleMapPlaceable.OnPlacement();
        _buildingManager.AddMapPlaceable(simpleMapPlaceable);
        cubes.Add(simpleMapPlaceable.gameObject);
        return simpleMapPlaceable;
    }

    Sprite GenerateImage(float[,] noise)
    {
        Texture2D texture2D = new Texture2D(_size.x, _size.y);
        for (int x = 0; x < noise.GetLength(0); x++)
        {
            for (int y = 0; y < noise.GetLength(1); y++)
            {
                texture2D.SetPixel(x, y, new Color(noise[x, y], noise[x, y], noise[x, y], 1f) * (_animationCurve.Evaluate(((float)y)/noise.GetLength(1)) * _animationCurve.Evaluate(((float)x)/noise.GetLength(0))));
            }
        }
        texture2D.Apply();
        return Sprite.Create(texture2D, new Rect(0, 0, _size.x, _size.y), Vector2.zero);
    }

    void GenerateCity(float[,] noise)
    {
        for (int x = 0; x < noise.GetLength(0); x++)
        {
            for (int y = 0; y < noise.GetLength(1); y++)
            {
                SimpleMapPlaceable simpleMapPlaceable;
                Vector3 position = new Vector3(x + this.offset.x, 0f, y+offset.z);
                if (x % _buildingBlockSize.x == 0 || y % _buildingBlockSize.y == 0)
                {
                    GameObject gameObject = Instantiate(_streetData.Prefab);
                    simpleMapPlaceable = gameObject.GetComponent<SimpleMapPlaceable>();
                    simpleMapPlaceable.ShowPlacementVisuals(false);
                    gameObject.transform.position = position;
                }
                else
                {
                    float height = Mathf.RoundToInt((0.5f + (_animationCurve.Evaluate(((float)y)/noise.GetLength(1)) * _animationCurve.Evaluate(((float)x)/noise.GetLength(0)) * _buildingScale * noise[x, y]))*4f)/4f;
                    GameObject building = new GameObject("Procedural Building");
                    building.transform.position = position;
                    ProceduralCityBuilding cityBuilding = building.AddComponent<ProceduralCityBuilding>();
                    simpleMapPlaceable = cityBuilding;
                    cityBuilding.Height = height;
                }
                simpleMapPlaceable.OnPlacement();
                _buildingManager.AddMapPlaceable(simpleMapPlaceable);
                cubes.Add(simpleMapPlaceable.gameObject);
            }
        }
    }

    void Reset()
    {
        foreach (GameObject cube in cubes)
        {
            SimpleMapPlaceable mapPlaceable = cube.GetComponent<SimpleMapPlaceable>();
            _buildingManager.RemoveMapPlaceable(mapPlaceable.transform.position);
            Destroy(cube);
        }
        cubes.Clear();
    }
}