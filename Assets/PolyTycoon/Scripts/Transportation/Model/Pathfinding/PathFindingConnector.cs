using UnityEngine;

public abstract class PathFindingConnector : PathFindingNode
{
    [SerializeField] protected Transform _cornerTransform;
    [SerializeField] protected Transform _straightTransform;
    [SerializeField] protected Transform _tIntersectionTransform;
    [SerializeField] protected Transform _intersectionTransform;

    protected override void Initialize()
    {
        base.Initialize();
        IsDraggable = true;
    }

    public override void Rotate(Vector3 axis, float rotationAmount)
    {
        // Don't rotate these
    }

    protected abstract PathFindingConnector Neighbor(int direction);

    public override void OnPlacement()
    {
        base.OnPlacement();
        transform.name = GetType().ToString() + ": " + transform.position.ToString();
        for (int i = 0; i < 4; i++)
        {
            PathFindingConnector neighbor = Neighbor(i);
            if (neighbor) neighbor.UpdateOrientation();
        }
        UpdateOrientation();
    }
    
    public override bool IsTraversable()
    {
        return true;
    }
    
    public override bool IsNode()
    {
        bool verticalStreet = AdjacentNodes(0) && AdjacentNodes(2) && !AdjacentNodes(3) && !AdjacentNodes(1);
        bool horizontalStreet = !AdjacentNodes(0) && !AdjacentNodes(2) && AdjacentNodes(3) && AdjacentNodes(1);
        return !(verticalStreet || horizontalStreet); // Only corner rails are nodes
    }
    
    protected virtual void UpdateOrientation()
    {
        if (AdjacentNodes(1) || AdjacentNodes(3))
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
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
    // Center points
    private static Vector3 middleLeft = new Vector3(-0.5f, 0f, 0f);
    private static Vector3 middleRight = new Vector3(0.5f, 0f, 0f);
    private static Vector3 middleTop = new Vector3(0f, 0f, 0.5f);
    private static Vector3 middleBottom = new Vector3(0f, 0f, -0.5f);
	
    // Lane points
    private static Vector3 topLeft = new Vector3(-0.25f, 0f, 0.5f);
    private static Vector3 topRight = new Vector3(0.25f, 0f, 0.5f);
    private static Vector3 rightTop = new Vector3(0.5f, 0f, 0.25f);
    private static Vector3 rightBottom = new Vector3(0.5f, 0f, -0.25f);
    private static Vector3 bottomRight = new Vector3(0.25f, 0f, -0.5f);
    private static Vector3 bottomLeft = new Vector3(-0.25f, 0f, -0.5f);
    private static Vector3 leftBottom = new Vector3(-0.5f, 0f, -0.25f);
    private static Vector3 leftTop = new Vector3(-0.5f, 0f, 0.25f);

    // Turn helper
    private static Vector3 centerTopRight = new Vector3(0.25f, 0f, 0.25f);
    private static Vector3 centerTopLeft = new Vector3(-0.25f, 0f, 0.25f);
    private static Vector3 centerBottomRight = new Vector3(0.25f, 0f, -0.25f);
    private static Vector3 centerBottomLeft = new Vector3(-0.25f, 0f, -0.25f);

    public static Vector3 TopLeft => topLeft;
    public static Vector3 TopRight => topRight;
    public static Vector3 RightTop => rightTop;
    public static Vector3 RightBottom => rightBottom;
    public static Vector3 BottomRight => bottomRight;
    public static Vector3 BottomLeft => bottomLeft;
    public static Vector3 LeftBottom => leftBottom;
    public static Vector3 LeftTop => leftTop;
    public static Vector3 CenterTopRight => centerTopRight;
    public static Vector3 CenterTopLeft => centerTopLeft;
    public static Vector3 CenterBottomRight => centerBottomRight;
    public static Vector3 CenterBottomLeft => centerBottomLeft;
    public static Vector3 MiddleLeft => middleLeft;
    public static Vector3 MiddleRight => middleRight;
    public static Vector3 MiddleTop => middleTop;
    public static Vector3 MiddleBottom => middleBottom;
}