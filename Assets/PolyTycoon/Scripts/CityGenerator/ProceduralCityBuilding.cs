using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralCityBuilding : SimpleMapPlaceable
{
    private float _height;
    
    protected override void Initialize()
    {
        
    }

    public override float GetHeight()
    {
        return this._height;
    }

    public void Generate(float height, SimpleMapPlaceable[] neighborPlaceables)
    {
        this._height = height;
        if (neighborPlaceables.Length > 4) Debug.LogError("Too many neighbors");
        for (int i = 0; i < neighborPlaceables.Length; i++)
        {
            if (neighborPlaceables[i] is Street)
            {
                Street street = ((Street) neighborPlaceables[i]);
            } else if (neighborPlaceables[i] is ProceduralCityBuilding)
            {
                ProceduralCityBuilding cityBuilding = (ProceduralCityBuilding) neighborPlaceables[i];
                if (PathFindingNode.Up == i)
                {
                    if (cityBuilding.GetHeight() < height)
                    {
                        // Window or something
                    } else if (Math.Abs(cityBuilding.GetHeight() - height) < 0.1f)
                    {
                        // Only walls, maybe roof
                    }
                    else
                    {
                        // maybe Roof
                    }
                }
            }
        }
    }
}
