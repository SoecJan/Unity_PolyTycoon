using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This interface defines functionality for optimizing the use of the Pathfinder.
/// Instead of calculating new paths on every request this interface enables existing paths to be saved and reused.
/// </summary>
public interface IPathNode
{
	/// <summary>
	/// This Method enables other classes to receive the Path from one node to another target node.
	/// </summary>
	/// <param name="targetNode">The Node that is supposed to be reached</param>
	/// <returns>A path from the IPathNode instance to the targetNode</returns>
	Path PathTo(PathFindingNode targetNode);

	/// <summary>
	/// This Method can be used to add a Path from the current instance to a target Node.
	/// </summary>
	/// <param name="targetNode">The target node that can be accessed by this path</param>
	/// <param name="path">The path that leads to the target node</param>
	void AddPath(PathFindingNode targetNode, Path path);

	/// <summary>
	/// This Method can be used to remove a Path from the current instance.
	/// </summary>
	/// <param name="targetNode">The target Node this instance should no longer hold a path to</param>
	void RemovePath(PathFindingNode targetNode);
}

/// <summary>
/// An endpoint.
/// The Pathfinding algorithm finds Paths from and to PathFindingTargets using <see cref="PathFindingConnector"/>.
/// </summary>
public class PathFindingTarget : PathFindingNode, IPathNode
{
	private Dictionary<PathFindingNode, Path> _paths; // Paths for the IPathNode Interface

	private void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.color = new Color(1, 1, 0, 0.75F);
		WayPoint wayPoint = this.GetTraversalVectors(-1, 1);
		Gizmos.DrawLine(wayPoint.TraversalVectors[0], wayPoint.TraversalVectors[1]);
		wayPoint = this.GetTraversalVectors(1, -1);
		Gizmos.DrawLine(wayPoint.TraversalVectors[0], wayPoint.TraversalVectors[1]);
	}

	public override void Start()
	{
		base.Start();
	    _paths = new Dictionary<PathFindingNode, Path>(); 
	}
    
    public override bool IsTraversable()
    {
	    return false; // It is not possible for movers to traverse through a PathFindingTarget
    }

    protected override bool IsNode()
    {
	    return true; // A PathFindingTarget can always be accessed via the node graph
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

    protected override void OnPlacement(SimpleMapPlaceable simpleMapPlaceable)
    {
	    base.OnPlacement(simpleMapPlaceable);
	    TraversalOffset = UsedCoordinates[0].UsedCoordinate + transform.position;
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