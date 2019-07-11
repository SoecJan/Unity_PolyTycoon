using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warehouse : AbstractStorageContainer
{
    protected override void Initialize()
    {
        base.Initialize();
        _isClickable = true;
    }
}
