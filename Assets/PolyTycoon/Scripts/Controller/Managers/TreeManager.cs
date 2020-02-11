using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TreeManager
{
    private GameObject _treeModel;
    private PlacementController _placementController;
    private CityManager _cityManager;

    // Start is called before the first frame update
    public TreeManager(PlacementController placementController)
    {
        _placementController = placementController;
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
        foreach (NeededSpace neededSpace in treeBehaviour.UsedCoordinates)
        {
            GameObject go = GameObject.Instantiate(_treeModel, neededSpace.UsedCoordinate + threadsafePlaceable.Position, Quaternion.identity, treeBehaviour.transform);
        }
        if (!_placementController.PlaceObject(treeBehaviour))
        {
            Debug.LogError("TreeBehaviour not placeable at " + threadsafePlaceable.Position);
            GameObject.Destroy(treeBehaviour.gameObject);
        }
    }
}
