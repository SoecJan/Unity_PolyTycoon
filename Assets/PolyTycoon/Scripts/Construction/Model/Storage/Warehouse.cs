using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warehouse : AbstractStorageContainer
{
    protected override void Initialize()
    {
        base.Initialize();
        IsClickable = true;
    }

    public override bool IsTraversable()
    {
        return false;
    }

    public override bool IsNode()
    {
        return true;
    }
}
