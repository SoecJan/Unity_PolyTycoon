using Debug = UnityEngine.Debug;

public class Street : PathFindingNode
{
	#region Methods
	protected override void Initialize()
	{
		base.Initialize();
		IsDraggable = true;
	}

	public override void OnPlacement()
	{
		base.OnPlacement();
		transform.name = "Street" + transform.position.ToString();
	}

	public override bool IsNode()
	{
		bool verticalStreet = AdjacentNodes(0) && AdjacentNodes(2) && !AdjacentNodes(3) && !AdjacentNodes(1);
		bool horizontalStreet = !AdjacentNodes(0) && !AdjacentNodes(2) && AdjacentNodes(3) && AdjacentNodes(1);
		return !(verticalStreet || horizontalStreet); // Only corner streets are nodes
	}

	public override bool IsTraversable()
	{
		return true;
	}
	#endregion
}