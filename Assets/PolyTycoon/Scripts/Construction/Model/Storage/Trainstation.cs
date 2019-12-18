using UnityEngine;

public class Trainstation : AbstractStorageContainer
{
    [SerializeField] private Rail _accessRail;

    public Rail AccessRail => _accessRail;
}