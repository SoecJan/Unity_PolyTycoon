using System.Collections.Generic;
using UnityEngine;

public class TreeManager : ITreeManager
{
    private GameObject _grassModel;
    private GameObject _treeModel;
    private IPlacementController _placementController;
    private ICityManager _cityManager;

    // Start is called before the first frame update
    public TreeManager(IPlacementController placementController)
    {
        _placementController = placementController;
        _grassModel = Resources.Load<GameObject>(PathUtil.Get("Grass"));
        _treeModel = Resources.Load<GameObject>(PathUtil.Get("Tree"));
    }

    public TreeBehaviour GetRandomTree()
    {
        GameObject gameObject = new GameObject("Tree");
        TreeBehaviour treeBehaviour = gameObject.AddComponent<TreeBehaviour>();
        treeBehaviour.UsedCoordinates = new List<NeededSpace>();
        treeBehaviour.UsedCoordinates.Add(new NeededSpace(Vector3Int.zero, TerrainGenerator.TerrainType.Flatland));
        treeBehaviour.UsedCoordinates.Add(new NeededSpace(Vector3Int.left, TerrainGenerator.TerrainType.Flatland));
        treeBehaviour.UsedCoordinates.Add(new NeededSpace(Vector3Int.right, TerrainGenerator.TerrainType.Flatland));
        return treeBehaviour;
    }

    public void OnTreePositionFound(object treePlacementObject)
    {
        ThreadsafePlaceable threadsafePlaceable = (ThreadsafePlaceable) treePlacementObject;
        TreeBehaviour treeBehaviour = (TreeBehaviour) threadsafePlaceable.MapPlaceable;
        treeBehaviour.transform.position = threadsafePlaceable.Position;
        treeBehaviour.gameObject.name = "Tree: " + threadsafePlaceable.Position;
        foreach (NeededSpace neededSpace in threadsafePlaceable.NeededSpaces)
        {
            GameObject prefab = null;
            if (neededSpace.TerrainType == TerrainGenerator.TerrainType.Coast)
            {
                prefab = _grassModel;
            }
            else
            {
                prefab = _treeModel;
            }
            GameObject go = GameObject.Instantiate(prefab, neededSpace.UsedCoordinate + threadsafePlaceable.Position, Quaternion.identity, treeBehaviour.transform);
        }
        if (!_placementController.PlaceObject(treeBehaviour))
        {
            GameObject.Destroy(treeBehaviour.gameObject);
        }
    }

    public Texture2D GetRandomForrestBlueprint()
    {
        return Resources.Load<Texture2D>(PathUtil.Get("ForrestBlueprint"));
    }
}
