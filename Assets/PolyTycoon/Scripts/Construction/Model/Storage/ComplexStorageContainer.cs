using System.Collections.Generic;
using UnityEngine;

public class ComplexStorageContainer : ComplexMapPlaceable
{
    protected override void Initialize()
    {
//        this.IsClickable = true;
    }

    public override void OnPlacement()
    {
        base.OnPlacement();
        transform.Translate(0f, -transform.position.y + 0.5f, 0f);
    }

    public override void Rotate(Vector3 axis, float rotationAmount)
    {
        base.Rotate(axis, rotationAmount);
        foreach (SimpleMapPlaceable childMapPlaceable in ChildMapPlaceables)
        {
            childMapPlaceable.Rotate(axis, rotationAmount);
        }
    }
}