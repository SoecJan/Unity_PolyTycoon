using System;
using UnityEngine;

/// <summary>
/// Base class for all PathFindingAlgorithms. Used in <see cref="PathFinder"/>.
/// </summary>
public abstract class AbstractPathFindingAlgorithm
{
    #region Methods
    /// <summary>
    /// This function calculates the path from startNode to endNode.
    /// </summary>
    /// <param name="startNode">The node this Path is supposed to start at</param>
    /// <param name="endNode">The node this Path is supposed to end at</param>
    /// <returns>The Path that was found. Null if there was no Path found.</returns>
    public abstract Path FindPath(PathFindingNode startNode, PathFindingNode endNode);
    
    /// <summary>
    /// This function calculates the distance between two Nodes.
    /// </summary>
    /// <param name="nodeA">Node A to calculate distance</param>
    /// <param name="nodeB">Node B to calculate distance</param>
    /// <returns>The distance between NodeA and NodeB</returns>
    protected int GetDistance(PathFindingNode nodeA, PathFindingNode nodeB)
    {
        return (int)Math.Abs((nodeA.ThreadsafePosition - nodeB.ThreadsafePosition).magnitude);
    }

    /// <summary>
    /// Transforms a direction Vector3 into an integer representing a direction.
    /// The values correspond to: <see cref="PathFindingNode"/>
    /// </summary>
    /// <param name="normalizedDirection">The direction that was normalized</param>
    /// <returns>The integer representing a direction</returns>
	protected int DirectionVectorToInt(Vector3 normalizedDirection)
	{
		if (normalizedDirection.Equals(Vector3.forward))
		{
			return PathFindingNode.Up;
		}
		else if (normalizedDirection.Equals(Vector3.right))
		{
			return PathFindingNode.Right;
		}
		else if (normalizedDirection.Equals(Vector3.back))
		{
			return PathFindingNode.Down;
		}
		else if (normalizedDirection.Equals(Vector3.left))
		{
			return PathFindingNode.Left;
		}

		return -1;
	}
    #endregion
}
