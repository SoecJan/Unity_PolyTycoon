using System;
using UnityEngine;

public abstract class AbstractAStarNode : IComparable<AbstractAStarNode>
{
    protected AbstractAStarNode(int hCost, int gCost)
    {
        HCost = hCost;
        GCost = gCost;
    }

    private int FCost => HCost + GCost;

    public int HCost { private get; set; }

    public int GCost { get; set; }

    public int CompareTo(AbstractAStarNode other)
    {
        int compare = FCost.CompareTo(other.FCost);
        if (compare == 0)
        {
            compare = HCost.CompareTo(other.HCost);
        }

        return compare;
    }
}

public class NetworkNode : AbstractAStarNode
{
    #region Constructors
    public NetworkNode(PathFindingNode pathFindingNode) : base(0,0)
    {
        PathFindingNode = pathFindingNode;
    }

    public NetworkNode(PathFindingNode pathFindingNode, int hCost, int gCost) : base (hCost, gCost)
    {
        PathFindingNode = pathFindingNode;
    }
    #endregion

    #region Getter & Setter
    public PathFindingNode PathFindingNode { get; }

    public NetworkNode Parent { get; set; }

    public override bool Equals(object obj)
    {
        return obj is NetworkNode other && PathFindingNode.Equals(other.PathFindingNode);
    }
    #endregion
}

class TileNode : AbstractAStarNode
{
    public TileNode(Vector2Int position) : base(0, 0)
    {
        PositionVector2 = position;
    }

    public TileNode(TileNode node, Vector2Int position, int hCost, int gCost) : base(hCost, gCost)
    {
        Parent = node;
        PositionVector2 = position;
    }

    public Vector2Int PositionVector2 { get; }

    public TileNode Parent { get; set; }

    public override bool Equals(object obj)
    {
        return obj is TileNode other && PositionVector2.Equals(other.PositionVector2);
    }
}