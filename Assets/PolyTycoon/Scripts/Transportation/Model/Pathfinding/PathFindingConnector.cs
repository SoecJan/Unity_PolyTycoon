using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Pathfinding Connectors create a network. Using this network the <see cref="PathFinder"/> can find a
/// <see cref="Path"/> from one <see cref="PathFindingNode"/> to an other.
/// </summary>
public abstract class PathFindingConnector : PathFindingNode
{
    [SerializeField] protected Transform _cornerTransform;
    [SerializeField] protected Transform _straightTransform;
    [SerializeField] protected Transform _tIntersectionTransform;
    [SerializeField] protected Transform _intersectionTransform;

    protected override void Initialize()
    {
        base.Initialize();
        _isHighlightable = false;
        IsDraggable = true;
    }

    public override void Rotate(Vector3 axis, float rotationAmount)
    {
        // Don't rotate these entities
    }

    public override void OnPlacement()
    {
        base.OnPlacement();
        transform.name = GetType().ToString() + ": " + transform.position.ToString();
        TraversalOffset = transform.position;
    }

    public override bool IsTraversable()
    {
        return true;
    }

    protected override bool IsNode()
    {
        bool verticalStreet = AdjacentNodes(0) && AdjacentNodes(2) && !AdjacentNodes(3) && !AdjacentNodes(1);
        bool horizontalStreet = !AdjacentNodes(0) && !AdjacentNodes(2) && AdjacentNodes(3) && AdjacentNodes(1);
        return !(verticalStreet || horizontalStreet); // Only corner rails are nodes
    }

    public virtual void UpdateOrientation()
    {
        Boolean[] nodes =
            {AdjacentNodes(0) != null, AdjacentNodes(1) != null, AdjacentNodes(2) != null, AdjacentNodes(3) != null};
        int connectedNodes = nodes.Count(c => c); // The amount of connected nodes

        if (connectedNodes <= 2)
        {
            if (!(nodes[0] || nodes[1] || nodes[2] || nodes[3])) // None connected
            {
                _straightTransform.gameObject.SetActive(true);
                _cornerTransform.gameObject.SetActive(false);
                _tIntersectionTransform.gameObject.SetActive(false);
                _intersectionTransform.gameObject.SetActive(false);
                transform.eulerAngles = Vector3.zero;
            }
            else if ((nodes[0] || nodes[2]) && !(nodes[1] || nodes[3])) // Straight
            {
                _straightTransform.gameObject.SetActive(true);
                _cornerTransform.gameObject.SetActive(false);
                _tIntersectionTransform.gameObject.SetActive(false);
                _intersectionTransform.gameObject.SetActive(false);
                transform.eulerAngles = new Vector3(0f, 90f, 0f);
            }
            else if ((nodes[1] || nodes[3]) && !(nodes[0] || nodes[2])) // Straight
            {
                _straightTransform.gameObject.SetActive(true);
                _cornerTransform.gameObject.SetActive(false);
                _tIntersectionTransform.gameObject.SetActive(false);
                _intersectionTransform.gameObject.SetActive(false);
                transform.eulerAngles = Vector3.zero;
            }
            else // Corner
            {
                for (int i = 0; i < 4; i++)
                {
                    bool cornerNode = nodes[i] && nodes[(i + 1) % 4];
                    if (!cornerNode) continue;
                    _straightTransform.gameObject.SetActive(false);
                    _cornerTransform.gameObject.SetActive(true);
                    _tIntersectionTransform.gameObject.SetActive(false);
                    _intersectionTransform.gameObject.SetActive(false);
                    transform.eulerAngles = new Vector3(0f, 90f, 0f) * (i);
                    break;
                }
            }
        } else if (connectedNodes == 3) // 3-way
        {
            for (int i = 0; i < 4; i++)
            {
                bool cornerNode = nodes[(i + 2) % 4] && nodes[i] && nodes[(i + 1) % 4];
                if (!cornerNode) continue;
                _straightTransform.gameObject.SetActive(false);
                _cornerTransform.gameObject.SetActive(false);
                _tIntersectionTransform.gameObject.SetActive(true);
                _intersectionTransform.gameObject.SetActive(false);
                transform.eulerAngles = new Vector3(0f, 90f, 0f) * (i);
                break;
            }
        }
        else // 4-way
        {
            _straightTransform.gameObject.SetActive(false);
            _cornerTransform.gameObject.SetActive(false);
            _tIntersectionTransform.gameObject.SetActive(false);
            _intersectionTransform.gameObject.SetActive(true);
        }
    }
}

public struct TraversalPoints
{
    public static Vector3 TopLeft { get; } = new Vector3(-0.25f, 0f, 0.5f);

    public static Vector3 TopRight { get; } = new Vector3(0.25f, 0f, 0.5f);

    public static Vector3 RightTop { get; } = new Vector3(0.5f, 0f, 0.25f);

    public static Vector3 RightBottom { get; } = new Vector3(0.5f, 0f, -0.25f);

    public static Vector3 BottomRight { get; } = new Vector3(0.25f, 0f, -0.5f);

    public static Vector3 BottomLeft { get; } = new Vector3(-0.25f, 0f, -0.5f);

    public static Vector3 LeftBottom { get; } = new Vector3(-0.5f, 0f, -0.25f);

    public static Vector3 LeftTop { get; } = new Vector3(-0.5f, 0f, 0.25f);

    public static Vector3 CenterTopRight { get; } = new Vector3(0.25f, 0f, 0.25f);

    public static Vector3 CenterTopLeft { get; } = new Vector3(-0.25f, 0f, 0.25f);

    public static Vector3 CenterBottomRight { get; } = new Vector3(0.25f, 0f, -0.25f);

    public static Vector3 CenterBottomLeft { get; } = new Vector3(-0.25f, 0f, -0.25f);

    public static Vector3 MiddleLeft { get; } = new Vector3(-0.5f, 0f, 0f);

    public static Vector3 MiddleRight { get; } = new Vector3(0.5f, 0f, 0f);

    public static Vector3 MiddleTop { get; } = new Vector3(0f, 0f, 0.5f);

    public static Vector3 MiddleBottom { get; } = new Vector3(0f, 0f, -0.5f);
}