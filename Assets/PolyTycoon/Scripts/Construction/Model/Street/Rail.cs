using UnityEngine;

/// <summary>
/// Is the foundation for rail networks.
/// </summary>
public class Rail : PathFindingConnector
{
	protected override PathFindingNode AdjacentNodes(int direction)
    {
	    PathFindingNode pathFindingNode = base.AdjacentNodes(direction);
	    return (pathFindingNode is Trainstation || pathFindingNode is Rail) ? pathFindingNode : null;
    }

    public override WayPoint GetTraversalVectors(int fromDirection, int toDirection)
    {
        Vector3 offset = ThreadsafePosition;
		// Start Points
		if (fromDirection == -1)
		{
			switch (toDirection)
			{
				case Up:
					return new WayPoint(offset, offset + TraversalPoints.MiddleTop);
				case Right:
					return new WayPoint(offset, offset + TraversalPoints.MiddleRight);
				case Down:
					return new WayPoint(offset, offset + TraversalPoints.MiddleBottom);
				case Left:
					return new WayPoint(offset, offset + TraversalPoints.MiddleLeft);
			}
		}

		// End Points

		if (toDirection == -1)
		{
			switch (fromDirection)
			{
				case Up:
					return new WayPoint(offset + TraversalPoints.MiddleTop, offset);
				case Right:
					return new WayPoint(offset + TraversalPoints.MiddleRight, offset);
				case Down:
					return new WayPoint(offset + TraversalPoints.MiddleBottom, offset);
				case Left:
					return new WayPoint(offset + TraversalPoints.MiddleLeft, offset);
			}
		}

		// Straights

		if (fromDirection == Up && toDirection == Down)
		{
			return new WayPoint(TraversalPoints.MiddleTop + offset, TraversalPoints.MiddleBottom + offset);
		}
		
		if (fromDirection == Down && toDirection == Up)
		{
			return new WayPoint(TraversalPoints.MiddleBottom + offset, TraversalPoints.MiddleTop + offset);
		}
		
		if (fromDirection == Left && toDirection == Right)
		{
			return new WayPoint(TraversalPoints.MiddleLeft + offset, TraversalPoints.MiddleRight + offset);
		}
		
		if (fromDirection == Right && toDirection == Left)
		{
			return new WayPoint(TraversalPoints.MiddleRight + offset, TraversalPoints.MiddleLeft + offset);
		}

		// Inner Corners

		float innerCornerRadius = 0.5f;

		if (fromDirection == Up && toDirection == Left)
		{
			return new WayPoint(TraversalPoints.MiddleTop + offset, offset, TraversalPoints.MiddleLeft + offset, innerCornerRadius);
		}
		
		if (fromDirection == Up && toDirection == Right)
		{
			return new WayPoint(TraversalPoints.MiddleTop + offset, offset, TraversalPoints.MiddleRight + offset, innerCornerRadius);
		}

		if (fromDirection == Down && toDirection == Right)
		{
			return new WayPoint(TraversalPoints.MiddleBottom + offset, offset, TraversalPoints.MiddleRight + offset, innerCornerRadius);
		}
		
		if (fromDirection == Down && toDirection == Left)
		{
			return new WayPoint(TraversalPoints.MiddleBottom + offset, offset, TraversalPoints.MiddleLeft + offset, innerCornerRadius);
		}

		if (fromDirection == Left && toDirection == Down)
		{
			return new WayPoint(TraversalPoints.MiddleLeft + offset, offset, TraversalPoints.MiddleBottom + offset, innerCornerRadius);
		}
		
		if (fromDirection == Left && toDirection == Up)
		{
			return new WayPoint(TraversalPoints.MiddleLeft + offset, offset, TraversalPoints.MiddleTop + offset, innerCornerRadius);
		}

		if (fromDirection == Right && toDirection == Up)
		{
			return new WayPoint(TraversalPoints.MiddleRight + offset, offset, TraversalPoints.MiddleTop + offset, innerCornerRadius);
		}
		
		if (fromDirection == Right && toDirection == Down)
		{
			return new WayPoint(TraversalPoints.MiddleRight + offset, offset, TraversalPoints.MiddleBottom + offset, innerCornerRadius);
		}
		
		return new WayPoint(offset, offset);
    }
}