using System;
using System.Collections.Generic;
using NUnit.Framework.Internal.Builders;
using UnityEngine;
using Random = System.Random;

public class TreeManager : ITreeManager
{
    private GameObject _coastModel;
    private GameObject _flatlandModel;
    private GameObject _hillModel;
    private GameObject _mountainModel;
    
    private IPlacementController _placementController;
    private ITerrainGenerator _terrainGenerator;
    private ICityManager _cityManager;

    // Start is called before the first frame update
    public TreeManager(IPlacementController placementController, ITerrainGenerator terrainGenerator)
    {
        _placementController = placementController;
        _terrainGenerator = terrainGenerator;
        _coastModel = Resources.Load<GameObject>(PathUtil.Get("Tree_Coast"));
        _flatlandModel = Resources.Load<GameObject>(PathUtil.Get("Tree_Flatland"));
        _hillModel = Resources.Load<GameObject>(PathUtil.Get("Tree_Hill"));
        _mountainModel = Resources.Load<GameObject>(PathUtil.Get("Tree_Mountain"));
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
        Random random = new Random(135);
        ThreadsafePlaceable threadsafePlaceable = (ThreadsafePlaceable) treePlacementObject;
        Vector2 position = _terrainGenerator.GetTerrainChunkPosition(threadsafePlaceable.Position.x, threadsafePlaceable.Position.z);
        Debug.Log("Tree Position Terrain Chunk: " + position);
        TerrainChunk terrainChunk = _terrainGenerator.GetTerrainChunk(position);
        TreeBehaviour treeBehaviour = (TreeBehaviour) threadsafePlaceable.MapPlaceable;
        treeBehaviour.transform.position = threadsafePlaceable.Position;
        treeBehaviour.gameObject.name = "Tree: " + threadsafePlaceable.Position;
        foreach (NeededSpace neededSpace in threadsafePlaceable.NeededSpaces)
        {
            ProceduralNeededSpace proceduralNeededSpace = (ProceduralNeededSpace) neededSpace;
            GameObject prefab = null;
            if (neededSpace.TerrainType == TerrainGenerator.TerrainType.Coast)
            {
                prefab = _coastModel; 
            }
            else if (neededSpace.TerrainType == TerrainGenerator.TerrainType.Flatland)
            {
                prefab = _flatlandModel;
            }
            else if (neededSpace.TerrainType == TerrainGenerator.TerrainType.Hill)
            {
                prefab = _hillModel;
            }
            else if (neededSpace.TerrainType == TerrainGenerator.TerrainType.Mountain)
            {
                prefab = _mountainModel;
            }
            else
            {
                continue;
            }

            Vector3 absolutePosition = neededSpace.UsedCoordinate + threadsafePlaceable.Position;
            Vector2Int pos = new Vector2Int((int) absolutePosition.x, (int) absolutePosition.z);
            if (terrainChunk.EnvDictionary.ContainsKey(pos)) continue;

            float proceduralScalar = Mathf.Clamp(((proceduralNeededSpace.NoiseValue*100f)%10)/7, 0.8f, 2f);
            GameObject go = GameObject.Instantiate(prefab, absolutePosition, Quaternion.Euler(0f, proceduralScalar * 360f, 0f), treeBehaviour.transform);
            // Debug.Log(proceduralNeededSpace.NoiseValue + " _ " + proceduralScalar);
            
            for(int i = 0; i < go.transform.childCount; i++) 
            {
                go.transform.GetChild(i).localScale *= proceduralScalar;
            }
            terrainChunk.EnvDictionary.Add(pos, go);
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
