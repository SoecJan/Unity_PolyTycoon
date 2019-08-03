using System.Collections.Generic;
using UnityEngine;

public interface IPathNode
{
	Path PathTo(PathFindingNode targetNode);

	void AddPath(PathFindingNode targetNode, Path path);

	void RemovePath(PathFindingNode targetNode);
}

public abstract class PathFindingTarget : PathFindingNode, IPathNode
{
	private Dictionary<PathFindingNode, Path> _paths;
	
	protected override void Initialize()
    {
	    base.Initialize();
	    _paths = new Dictionary<PathFindingNode, Path>();
	    _isClickable = true;
    }
    
    public override bool IsTraversable()
    {
	    return false;
    }

    protected override bool IsNode()
    {
	    return true;
    }
    
    public Path PathTo(PathFindingNode targetNode)
    {
	    return _paths.ContainsKey(targetNode) ? _paths[targetNode] : null;
    }

    public void AddPath(PathFindingNode targetNode, Path path)
    {
	    if (_paths.ContainsKey(targetNode))
	    {
		    _paths[targetNode] = path;
	    }
	    else
	    {
		    _paths.Add(targetNode, path);
	    }
    }

    public void RemovePath(PathFindingNode targetNode)
    {
	    _paths.Remove(targetNode);
    }
    
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
}