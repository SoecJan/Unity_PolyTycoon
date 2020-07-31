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
        return (int) Mathf.Abs(Vector3.Distance(nodeA.ThreadsafePosition, nodeB.ThreadsafePosition));
    }
    #endregion
}
