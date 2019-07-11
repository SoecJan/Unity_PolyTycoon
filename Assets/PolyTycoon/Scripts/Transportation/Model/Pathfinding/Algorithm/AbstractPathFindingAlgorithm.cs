using System;
using UnityEngine;

public abstract class AbstractPathFindingAlgorithm
{
    #region Methods
    public abstract Path FindPath(PathFindingNode startNode, PathFindingNode endNode);
    
    protected int GetDistance(PathFindingNode nodeA, PathFindingNode nodeB)
    {
        return (int)Math.Abs((nodeA.ThreadsafePosition - nodeB.ThreadsafePosition).magnitude);
    }

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
