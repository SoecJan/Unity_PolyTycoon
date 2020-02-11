using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private GameSettings _gameSettings;
    [SerializeField] private Transform _terrainViewer;

    private ThreadedDataRequester _threadedDataRequester;
    private TerrainGenerator _terrainGenerator;
    private PlacementController _placementController;
    private BuildingManager _buildingManager;

    private CityManager _cityCityManager;
    private TreeManager _treeManager;

    private VehicleManager _vehicleManager;
    private TransportRouteManager _transportRouteManager;

    private void Start()
    {
        _vehicleManager = new VehicleManager();
        if (_gameSettings == null)
        {
            _gameSettings = Resources.Load<GameSettings>(PathUtil.Get("GameSettings"));
        }
        
        _threadedDataRequester = new ThreadedDataRequester();
        

        // Terrain Generator
        MeshSettings meshSettings = Resources.Load<MeshSettings>(PathUtil.Get("MeshSettings"));
        HeightMapSettings heightMapSettings = Resources.Load<HeightMapSettings>(PathUtil.Get("HeightMapSettings"));
        Material terrainMaterial = Resources.Load<Material>(PathUtil.Get("TerrainMeshMaterial"));
        Transform viewer = _terrainViewer;
        _terrainGenerator = new TerrainGenerator(meshSettings, heightMapSettings, viewer, terrainMaterial, _gameSettings.MapSize);
        
        _buildingManager = new BuildingManager();
        _placementController = new PlacementController(_buildingManager, _terrainGenerator);
        _cityCityManager = new CityManager(_placementController);
        _treeManager = new TreeManager(_placementController);
        _transportRouteManager = new TransportRouteManager(new PathFinder(_terrainGenerator), _vehicleManager);
    }

    public GameSettings GameSettings
    {
        get => _gameSettings;
        set => _gameSettings = value;
    }

    public ThreadedDataRequester ThreadedDataRequester
    {
        get => _threadedDataRequester;
        set => _threadedDataRequester = value;
    }

    public TerrainGenerator TerrainGenerator
    {
        get => _terrainGenerator;
        set => _terrainGenerator = value;
    }

    public VehicleManager VehicleManager
    {
        get => _vehicleManager;
        set => _vehicleManager = value;
    }

    public TransportRouteManager TransportRouteManager
    {
        get => _transportRouteManager;
        set => _transportRouteManager = value;
    }

    public PlacementController PlacementController
    {
        get => _placementController;
        set => _placementController = value;
    }

    public BuildingManager BuildingManager
    {
        get => _buildingManager;
        set => _buildingManager = value;
    }

    public CityManager CityManager
    {
        get => _cityCityManager;
        set => _cityCityManager = value;
    }

    public TreeManager TreeManager
    {
        get => _treeManager;
        set => _treeManager = value;
    }

    private void Update()
    {
        _threadedDataRequester.Update();
        _terrainGenerator.Update();
    }

    private void OnDestroy()
    {
        _threadedDataRequester.OnDestroy();
    }
}
