namespace Assets.PolyTycoon.Scripts.Construction.Model.Street
{
	public class Street : PathFindingNode 
	{
		#region Methods
		protected override void Initialize()
		{
			base.Initialize();
			IsDraggable = true;
		}

		public override bool IsNode()
		{
			bool verticalStreet = AdjacentNodes[TOP_NODE] && AdjacentNodes[BOTTOM_NODE] && !AdjacentNodes[LEFT_NODE] && !AdjacentNodes[RIGHT_NODE];
			bool horizontalStreet = !AdjacentNodes[TOP_NODE] && !AdjacentNodes[BOTTOM_NODE] && AdjacentNodes[LEFT_NODE] && AdjacentNodes[RIGHT_NODE];
			return !(verticalStreet || horizontalStreet); // Only corner streets are nodes
		}

		public override bool IsTraversable()
		{
			return true;
		}
		#endregion
	}
}
