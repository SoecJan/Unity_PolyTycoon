using System;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralCityBuilding : CityBuilding
{
    private float _height;
    private static Dictionary<Vector2Int, GameObject[]> _models;
    private static System.Random _random;

    protected override void Initialize()
    {
        UsedCoordinates = new List<NeededSpace>();
        _isClickable = true;
        _random = new System.Random(0);

        if (_models != null) return;
        _models = new Dictionary<Vector2Int, GameObject[]>
        {
            {new Vector2Int(1, 1), Resources.LoadAll<GameObject>(Util.PathTo("NonModularBuildings") + "/1x1")},
            {new Vector2Int(1, 2), Resources.LoadAll<GameObject>(Util.PathTo("NonModularBuildings") + "/1x2")},
            {new Vector2Int(2, 1), Resources.LoadAll<GameObject>(Util.PathTo("NonModularBuildings") + "/2x1")},
            {new Vector2Int(2, 2), Resources.LoadAll<GameObject>(Util.PathTo("NonModularBuildings") + "/2x2")},
            {new Vector2Int(2, 3), Resources.LoadAll<GameObject>(Util.PathTo("NonModularBuildings") + "/2x3")},
            {new Vector2Int(3, 2), Resources.LoadAll<GameObject>(Util.PathTo("NonModularBuildings") + "/3x2")},
            {new Vector2Int(3, 3), Resources.LoadAll<GameObject>(Util.PathTo("NonModularBuildings") + "/3x3")}
        };
    }

    public float Height
    {
        get => _height;
        set { _height = value; }
    }

    public void Generate(int blockSize, Vector3 freePathDirection, Vector3 streetDirection,
        PlacementController placementController)
    {
        Vector2Int layout = Vector2Int.zero;
        for (int i = 6; i > 1; i--)
        {
            int x = Mathf.CeilToInt(i / 2f);
            int y = Mathf.FloorToInt(i / 2f);
            if (x > blockSize) continue;

            List<NeededSpace> neededSpaces = new List<NeededSpace>();
            for (int j = 0; j < x; j++)
            {
                for (int k = 0; k < y; k++)
                {
                    Vector3Int position = Vector3Int.RoundToInt(freePathDirection * j + -streetDirection * k);
                    // Debug.Log(position + " = " + freePathDirection + " * " + j + " + " + -streetDirection + " * " + k);
                    neededSpaces.Add(new NeededSpace(position, TerrainGenerator.TerrainType.Flatland));
                }
            }

            if (!placementController.IsPlaceable(transform.position, neededSpaces)) continue;
            layout = new Vector2Int(x, y);
            UsedCoordinates = neededSpaces;
            break;
        }

        if (layout == Vector2Int.zero) return;
        // Debug.Log(layout + ", " + UsedCoordinates.Count + ", " + _models[layout][0].name);

        Vector3 relativeCenter = ((layout.y-1) * -streetDirection / 2f) + ((layout.x-1) * freePathDirection / 2f);
        GameObject graphics = Instantiate(_models[layout][_random.Next(1, _models[layout].Length)], transform); // Starting at 1 because the template is always at [0]
        graphics.transform.localPosition = relativeCenter;
        graphics.transform.Rotate(Vector3.up, Util.DirectionVectorToInt(streetDirection) * 90f);
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.size = Quaternion.AngleAxis(Util.DirectionVectorToInt(streetDirection) * 90f, Vector3.up) * new Vector3(layout.x, 0.5f, layout.y);
        boxCollider.center = relativeCenter - (Vector3.up /4f);
    }
}