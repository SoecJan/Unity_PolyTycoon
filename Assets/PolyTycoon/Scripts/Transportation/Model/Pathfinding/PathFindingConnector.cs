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
        if (AdjacentNodes(1) || AdjacentNodes(3))
        {
            transform.eulerAngles = Vector3.zero;
        }
        
        bool verticalNode = AdjacentNodes(0) || AdjacentNodes(2);
        if (verticalNode)
        {
            _straightTransform.gameObject.SetActive(true);
            _cornerTransform.gameObject.SetActive(false);
            _tIntersectionTransform.gameObject.SetActive(false);
            _intersectionTransform.gameObject.SetActive(false);
            transform.eulerAngles = new Vector3(0f, 90f, 0f);
        }

        for (int i = 0; i < 4; i++)
        {
            bool cornerNode = AdjacentNodes(i) && AdjacentNodes((i+1) % 4);
            if (!cornerNode) continue;
            _straightTransform.gameObject.SetActive(false);
            _cornerTransform.gameObject.SetActive(true);
            _tIntersectionTransform.gameObject.SetActive(false);
            _intersectionTransform.gameObject.SetActive(false);
            transform.eulerAngles = new Vector3(0f, 90f, 0f) * (i);
            break;
        }
        
        for (int i = 0; i < 4; i++)
        {
            bool cornerNode = AdjacentNodes((i+2) % 4) && AdjacentNodes(i) && AdjacentNodes((i+1) % 4);
            if (!cornerNode) continue;
            _straightTransform.gameObject.SetActive(false);
            _cornerTransform.gameObject.SetActive(false);
            _tIntersectionTransform.gameObject.SetActive(true);
            _intersectionTransform.gameObject.SetActive(false);
            transform.eulerAngles = new Vector3(0f, 90f, 0f) * (i);
            break;
        }

        if (AdjacentNodes(0) && AdjacentNodes(1) && AdjacentNodes(2) && AdjacentNodes(3))
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