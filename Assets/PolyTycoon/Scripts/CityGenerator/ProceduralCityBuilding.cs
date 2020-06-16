using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProceduralCityBuilding : SimpleMapPlaceable
{
    private float _height;

    protected override void Initialize()
    {
        UsedCoordinates = new List<NeededSpace>();
    }

    public float Height
    {
        get => _height;
        set
        {
            _height = value;
        }
    }


    public void Generate(int amount, Vector3 directionVec)
    {
        for (int i = 0; i < amount; i++)
        {
            NeededSpace neededSpace = new NeededSpace(Vector3Int.RoundToInt(directionVec * i),
                TerrainGenerator.TerrainType.Flatland);
            UsedCoordinates.Add(neededSpace);
            GameObject child = GameObject.CreatePrimitive(PrimitiveType.Cube);
            child.transform.parent = transform;
            child.transform.localScale = Vector3.forward + (Vector3.up * _height) + Vector3.right;
            child.transform.localPosition = (Vector3.down / 2f + (Vector3.up * _height) / 2f) + Vector3Int.RoundToInt(directionVec * i);
        }
    }
}