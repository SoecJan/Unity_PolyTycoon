using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private GameSettings _gameSettings;
    [SerializeField] private Transform _terrainViewer;

    private ThreadedDataRequester _threadedDataRequester;
    private ITerrainGenerator _terrainGenerator;
    private IPlacementController _placementController;
    private IBuildingManager _buildingManager;

    private ICityManager _cityCityManager;
    private ITreeManager _treeManager;

    private IVehicleManager _vehicleManager;
    private ITransportRouteManager _transportRouteManager;

    private ProgressionManager _progressionManager;

    private void Start()
    {
        _progressionManager = new ProgressionManager();
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

    public ProgressionManager ProgressionManager
    {
        get => _progressionManager;
        set => _progressionManager = value;
    }

    public ThreadedDataRequester ThreadedDataRequester
    {
        get => _threadedDataRequester;
        set => _threadedDataRequester = value;
    }

    public ITerrainGenerator TerrainGenerator
    {
        get => _terrainGenerator;
        set => _terrainGenerator = value;
    }

    public IVehicleManager VehicleManager
    {
        get => _vehicleManager;
        set => _vehicleManager = value;
    }

    public ITransportRouteManager TransportRouteManager
    {
        get => _transportRouteManager;
        set => _transportRouteManager = value;
    }

    public IPlacementController PlacementController
    {
        get => _placementController;
        set => _placementController = value;
    }

    public IBuildingManager BuildingManager
    {
        get => _buildingManager;
        set => _buildingManager = value;
    }

    public ICityManager CityManager
    {
        get => _cityCityManager;
        set => _cityCityManager = value;
    }

    public ITreeManager TreeManager
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
