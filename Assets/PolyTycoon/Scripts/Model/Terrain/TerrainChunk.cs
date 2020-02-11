using UnityEngine;

// Wraps all MeshData in one Object
public class TerrainChunk
{
    /* Attributes */
    public event System.Action<TerrainChunk, bool> onVisibilityChanged;
    public Vector2 coord; // Coordinates
    public GameObject meshObject;

    Vector2 sampleCentre;
    Bounds bounds;

    MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    HeightMap heightMap;
    private bool _hasMesh;

    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;

    private CloudBehaviour _cloudBehaviour;

    static Transform viewer;

    /* Constructors */
    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings,
        Transform parent, Transform viewerTransform, Material material)
    {
        this.coord = coord;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        viewer = viewerTransform;

        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.meshWorldSize; // Calculate the position of this TerrainChunk
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

        // Instantiate the new object
        meshObject = new GameObject("Terrain Chunk: " + coord.ToString());
        meshObject.SetActive(false);
        GameObject.Instantiate(Resources.Load<GameObject>(PathUtil.Get("Lake")), meshObject.transform);
        meshObject.layer = LayerMask.NameToLayer("Terrain");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        BoxCollider boxCollider = meshObject.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(46f, 0.1f, 46f);
        meshRenderer.material = material;
        meshObject.isStatic = true;

        // Set the GameObjects position and parent
        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;
    }

    /* Getter & Setter */

    #region Getter & Setter

    Vector2 viewerPosition
    {
        get { return new Vector2(viewer.position.x, viewer.position.z); }
    }

    public HeightMap HeightMap
    {
        get { return heightMap; }

        set { heightMap = value; }
    }

    public bool HasHeightMap()
    {
        return HeightMap.values != null && HeightMap.values.Length != 0;
    }

    public bool HasMesh()
    {
        return _hasMesh;
    }

    public Vector3 GetPositionMultiplier()
    {
        return new Vector3(coord.x * ((float) meshSettings.numVertsPerLine - 3), 0f,
            coord.y * ((float) meshSettings.numVertsPerLine - 3));
    }

    #endregion

    /* Methods */

    // Called by TerrainGenerator 86 after creating a new TerrainChunk
    public void Load()
    {
        // Request HeightMapData on a different Thread. OnHeightMapReceived is the callback function
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine,
            meshSettings.numVertsPerLine,
            heightMapSettings, sampleCentre), OnHeightMapReceived);
    }

    // Callback function on ThreadedDataRequester invoked by this.Load();
    void OnHeightMapReceived(object heightMapObject)
    {
        this.heightMap = (HeightMap) heightMapObject;
        GameHandler gameHandler = GameObject.FindObjectOfType<GameHandler>();

        Vector3 vec3 = new Vector3(this.sampleCentre.x + 0.5f, 0f, this.sampleCentre.y + 0.5f);
        // City Placement
        CityPlaceable cityPlaceable = gameHandler.CityManager.GetRandomCityPrefab();
        ThreadsafePlaceable cityToPlace =
            new ThreadsafePlaceable(cityPlaceable, vec3);
        ThreadedDataRequester.RequestData(
            () => ThreadsafePlacementManager.MoveToPlaceablePosition(gameHandler.PlacementController,
                gameHandler.TerrainGenerator, cityToPlace), gameHandler.CityManager.OnPlacementPositionFound,
            OnCityPlacement);

        // Cloud
        CloudBehaviour cloudBehaviour = Resources.Load<CloudBehaviour>(PathUtil.Get("Cloud"));
        Vector3 cloudHeight = (Vector3.up * Random.Range(10, 15));
        Vector3 cloudOffset = (Vector3.forward * Random.Range(-25, 25)) + (Vector3.left * Random.Range(-25, 25));
        _cloudBehaviour = GameObject.Instantiate(cloudBehaviour, vec3 + cloudHeight + cloudOffset, Quaternion.identity,
            meshObject.transform);

        // Mesh Generation
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, 0),
            OnMeshDataReceived);
        UpdateTerrainChunk();
    }

    void OnCityPlacement(object cityPlacementObject)
    {
        // Tree Placement
        GameHandler gameHandler = GameObject.FindObjectOfType<GameHandler>();
        TreeManager treeManager = gameHandler.TreeManager;
        TreeBehaviour treeBehaviour = treeManager.GetRandomTree();
        ThreadsafePlaceable treeToPlace =
            new ThreadsafePlaceable(treeBehaviour,
                new Vector3(this.sampleCentre.x + 0.5f, 0f, this.sampleCentre.y + 0.5f));
        ThreadedDataRequester.RequestData(
            () => ThreadsafePlacementManager.MoveToPlaceablePosition(gameHandler.PlacementController,
                gameHandler.TerrainGenerator, treeToPlace), gameHandler.TreeManager.OnTreePositionFound);
    }

    void OnMeshDataReceived(object meshDataObject)
    {
        meshFilter.mesh = ((MeshData) meshDataObject).CreateMesh();
        _hasMesh = true;
    }

    #region Chunk Update

    public void UpdateTerrainChunk()
    {
        float viewerDistance = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
        bool wasVisible = meshObject.activeSelf;
        bool visible = viewerDistance <= 120f;

        if (wasVisible != visible)
        {
            meshObject.SetActive(visible);
            onVisibilityChanged?.Invoke(this, visible);
        }
    }

    #endregion
}