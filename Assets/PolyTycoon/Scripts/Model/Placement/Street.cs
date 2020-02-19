using System.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// Is the foundation for street networks
/// </summary>
public class Street : PathFindingConnector
{
	#region Methods
	protected override PathFindingNode AdjacentNodes(int i)
	{
		PathFindingNode node = base.AdjacentNodes(i);
		return node is Rail ? null : node;
	}
	#endregion
	
	#region Traversal
	///<summary>Returns the TraversalVectors as a WayPoint Object.</summary>
	public override WayPoint GetTraversalVectors(int fromDirection, int toDirection)
	{
		
		// Start Points
		if (fromDirection == -1)
		{
			switch (toDirection)
			{
				case Up:
					return new WayPoint(TraversalPoints.CenterBottomRight + TraversalOffset, TraversalPoints.TopRight + TraversalOffset);
				case Right:
					return new WayPoint(TraversalPoints.CenterBottomLeft + TraversalOffset, TraversalPoints.RightBottom + TraversalOffset);
				case Down:
					return new WayPoint(TraversalPoints.CenterTopLeft + TraversalOffset, TraversalPoints.BottomLeft + TraversalOffset);
				case Left:
					return new WayPoint(TraversalPoints.CenterTopRight + TraversalOffset, TraversalPoints.LeftTop + TraversalOffset);
			}
		}

		// End Points

		if (toDirection == -1)
		{
			switch (fromDirection)
			{
				case Up:
					return new WayPoint(TraversalPoints.TopLeft + TraversalOffset, TraversalPoints.CenterBottomLeft + TraversalOffset);
				case Right:
					return new WayPoint(TraversalPoints.RightTop + TraversalOffset, TraversalPoints.CenterTopLeft + TraversalOffset);
				case Down:
					return new WayPoint(TraversalPoints.BottomRight + TraversalOffset, TraversalPoints.CenterTopRight + TraversalOffset);
				case Left:
					return new WayPoint(TraversalPoints.LeftBottom + TraversalOffset, TraversalPoints.CenterBottomRight + TraversalOffset);
			}
		}

		// Straights

		if (fromDirection == Up && toDirection == Down)
		{
			return new WayPoint(TraversalPoints.TopLeft + TraversalOffset, TraversalPoints.BottomLeft + TraversalOffset);
		}
		
		if (fromDirection == Down && toDirection == Up)
		{
			return new WayPoint(TraversalPoints.BottomRight + TraversalOffset, TraversalPoints.TopRight + TraversalOffset);
		}
		
		if (fromDirection == Left && toDirection == Right)
		{
			return new WayPoint(TraversalPoints.LeftBottom + TraversalOffset, TraversalPoints.RightBottom + TraversalOffset);
		}
		
		if (fromDirection == Right && toDirection == Left)
		{
			return new WayPoint(TraversalPoints.RightTop + TraversalOffset, TraversalPoints.LeftTop + TraversalOffset);
		}

		// Inner Corners

		float innerCornerRadius = 0.5f;

		if (fromDirection == Up && toDirection == Left)
		{
			return new WayPoint(TraversalPoints.TopLeft + TraversalOffset, TraversalPoints.CenterTopLeft + TraversalOffset, TraversalPoints.LeftTop + TraversalOffset, innerCornerRadius);
		}

		if (fromDirection == Down && toDirection == Right)
		{
			return new WayPoint(TraversalPoints.BottomRight + TraversalOffset, TraversalPoints.CenterBottomRight + TraversalOffset, TraversalPoints.RightBottom + TraversalOffset, innerCornerRadius);
		}

		if (fromDirection == Left && toDirection == Down)
		{
			return new WayPoint(TraversalPoints.LeftBottom + TraversalOffset, TraversalPoints.CenterBottomLeft + TraversalOffset, TraversalPoints.BottomLeft + TraversalOffset, innerCornerRadius);
		}

		if (fromDirection == Right && toDirection == Up)
		{
			return new WayPoint(TraversalPoints.RightTop + TraversalOffset, TraversalPoints.CenterTopRight + TraversalOffset, TraversalPoints.TopRight + TraversalOffset, innerCornerRadius);
		}

		// Outer Corners

		float outerCornerRadius = 0.75f;

		if (fromDirection == Up && toDirection == Right)
		{
			return new WayPoint(TraversalPoints.TopLeft + TraversalOffset, TraversalPoints.CenterBottomLeft + TraversalOffset, TraversalPoints.RightBottom + TraversalOffset, outerCornerRadius);
		}

		if (fromDirection == Down && toDirection == Left)
		{
			return new WayPoint(TraversalPoints.BottomRight + TraversalOffset, TraversalPoints.CenterTopRight + TraversalOffset, TraversalPoints.LeftTop + TraversalOffset, outerCornerRadius);
		}

		if (fromDirection == Left && toDirection == Up)
		{
			return new WayPoint(TraversalPoints.LeftBottom + TraversalOffset, TraversalPoints.CenterBottomRight + TraversalOffset, TraversalPoints.TopRight + TraversalOffset, outerCornerRadius);
		}

		if (fromDirection == Right && toDirection == Down)
		{
			return new WayPoint(TraversalPoints.RightTop + TraversalOffset, TraversalPoints.CenterTopLeft + TraversalOffset, TraversalPoints.BottomLeft + TraversalOffset, outerCornerRadius);
		}
		Debug.LogError("Should not reach here! Input: " + fromDirection + "; " + toDirection);
		return new WayPoint(Vector3.zero, Vector3.zero);
	}
	#endregion
}