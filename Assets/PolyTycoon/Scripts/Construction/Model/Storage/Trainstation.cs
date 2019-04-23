using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainstation : AbstractStorageContainer
{
    [SerializeField] private Transform _stationTransform;

    public Transform StationTransform
    {
        get { return _stationTransform; }
    }

    protected override void Initialize()
    {
        base.Initialize();
        IsClickable = true;
    }

    public override void OnPlacement()
    {
        base.OnPlacement();
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