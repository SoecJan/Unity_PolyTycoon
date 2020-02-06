using System.Collections;
using System.Threading;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    [SerializeField] private SimpleMapPlaceable _treeBehaviour;
    private PlacementManager _placementManager;
    private CityManager _cityManager;

    // Start is called before the first frame update
    void Start()
    {
        _placementManager = FindObjectOfType<PlacementManager>();
    }

    public void PlaceTrees(TerrainChunk terrainChunk, bool isVisible)
    {
        if (isVisible && terrainChunk.heightMapReceived)
        {
            terrainChunk.onVisibilityChanged -= PlaceTrees;
            StartCoroutine(PlaceTreesRoutine(terrainChunk));
        }
    }

    private IEnumerator PlaceTreesRoutine(TerrainChunk terrainChunk)
    {
        yield return new WaitForSeconds(5);
        int amountOfForests = 2;
        for (int i = 1; i < amountOfForests+1; i++)
        {
            int amountOfTrees = Random.Range(5, 10);
            Vector3Int startPosition = new Vector3Int((int)terrainChunk.coord.x*45, 0, (int)terrainChunk.coord.y*45);
            ThreadedDataRequester.RequestData(() => PlaceTrees(amountOfTrees, startPosition), OnTreePositionFound);
        }
    }

    private void OnTreePositionFound(object positions)
    {
        Vector3[] treePositions = (Vector3[]) positions;
        foreach (Vector3 treePosition in treePositions)
        {
            SimpleMapPlaceable go = Instantiate(_treeBehaviour, treePosition, Quaternion.identity);
            if (!_placementManager.PlaceObject(go))
            {
                Destroy(go.gameObject);
            }
        }
    }
    
    Vector3[] PlaceTrees(int amountOfTrees, Vector3 startPosition)
    {
        while (!_placementManager.TerrainGenerator.IsReady())
        {
            Thread.Sleep(100);
        }
        
        Vector3 currentPosition = startPosition + new Vector3(0.5f, 0, 0.5f);
        
        int stepSize = 1;
        int targetXMove = 0;
        int targetYMove = 0;
        int currentXMove = 0;
        int currentYMove = 0;
        int direction = -1;
        
        Vector3[] vector3s = new Vector3[amountOfTrees];

        for (int i = 0; i < amountOfTrees; i++)
        {
            // Moves the cityPlaceable in a growing square around the starting position until a suitable location is found
            while (!_placementManager.IsPlaceable(currentPosition, _treeBehaviour.UsedCoordinates))
            {
                // Move one step at a time
                if (currentXMove > 0)
                {
                    currentPosition += new Vector3(direction * stepSize,0,0);
                    currentXMove--;
                    continue;
                }

                if (currentYMove > 0)
                {
                    currentPosition += new Vector3(0, 0, direction * stepSize);
                    currentYMove--;
                    continue;
                }

                // Control step Amount
                if (targetXMove == targetYMove)
                {
                    direction = -direction;
                    targetXMove++;
                    currentXMove = targetXMove;
                }
                else
                {
                    targetYMove = targetXMove;
                    currentYMove = targetYMove;
                }
            }
            vector3s[i] = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z);
            // Move one step at a time
            if (currentXMove > 0)
            {
                currentPosition += new Vector3(direction * stepSize,0,0);
                currentXMove--;
                continue;
            }

            if (currentYMove > 0)
            {
                currentPosition += new Vector3(0, 0, direction * stepSize);
                currentYMove--;
                continue;
            }

            // Control step Amount
            if (targetXMove == targetYMove)
            {
                direction = -direction;
                targetXMove++;
                currentXMove = targetXMove;
            }
            else
            {
                targetYMove = targetXMove;
                currentYMove = targetYMove;
            }
        }

        return vector3s;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
