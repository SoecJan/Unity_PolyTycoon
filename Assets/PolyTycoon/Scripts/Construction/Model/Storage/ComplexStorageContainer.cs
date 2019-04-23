using System.Collections.Generic;

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
}