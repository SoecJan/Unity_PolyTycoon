using System.Collections.Generic;
using UnityEngine;

public class ProceduralCityBuilding : CityBuilding
{
    private float _height;

    protected override void Initialize()
    {
        UsedCoordinates = new List<NeededSpace>();
        _isClickable = true;
    }

    public float Height
    {
        get => _height;
        set
        {
            _height = value;
        }
    }


    public void Generate(int amount, Vector3 freePathDirection, Vector3 streetDirection, PlacementController placementController)
    {
        gameObject.AddComponent<BoxCollider>();
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < amount; x++)
            {
                Vector3Int position = Vector3Int.RoundToInt(freePathDirection * x) +
                                      Vector3Int.RoundToInt(-streetDirection * y);
                if (!placementController.IsPlaceable(transform.position + position, new List<NeededSpace>() {NeededSpace.Zero(TerrainGenerator.TerrainType.Flatland)})) continue;
                NeededSpace neededSpace = new NeededSpace(position, TerrainGenerator.TerrainType.Flatland);
                UsedCoordinates.Add(neededSpace);
                
                GameObject child = GameObject.CreatePrimitive(PrimitiveType.Cube);
                child.transform.parent = transform;
                child.transform.localScale = Vector3.forward + (Vector3.up * _height) + Vector3.right;
                child.transform.localPosition = (Vector3.down / 2f + (Vector3.up * _height) / 2f) + position;
            }
        }
    }
}