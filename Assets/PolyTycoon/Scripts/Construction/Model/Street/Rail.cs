using UnityEngine;

public class Rail : PathFindingNode
{
	[SerializeField] private Transform _cornerTransform;
	[SerializeField] private Transform _straightTransform;
	[SerializeField] private Transform _tIntersectionTranform;
	[SerializeField] private Transform _intersectionTransform;
    protected override void Initialize()
    {
        base.Initialize();
        IsDraggable = true;
    }

    public override void OnPlacement()
    {
        base.OnPlacement();
        transform.name = "Rail: " + transform.position.ToString();
        for (int i = 0; i < 4; i++)
        {
            Rail rail = AdjacentNodes(i) as Rail;
            if (rail) rail.UpdateOrientation();
        }
        UpdateOrientation();
    }
    
    protected override SimpleMapPlaceable AdjacentNodes(int i)
    {
	    SimpleMapPlaceable simpleMapPlaceable = base.AdjacentNodes(i);
	    Trainstation trainstation = simpleMapPlaceable as Trainstation;
	    return simpleMapPlaceable is Rail || (trainstation != null && trainstation.AccessRail == this) ? simpleMapPlaceable : null;
    }

    private Rail NeighborRail(int i)
    {
	    Rail neighborPlaceable = null;
	    switch (i)
	    {
		    case 0:
			    neighborPlaceable = BuildingManager.GetNode(gameObject.transform.position + Vector3.forward) as Rail;
			    break;
		    case 1:
			    neighborPlaceable = BuildingManager.GetNode(gameObject.transform.position + Vector3.right) as Rail;
			    break;
		    case 2:
			    neighborPlaceable = BuildingManager.GetNode(gameObject.transform.position + Vector3.back) as Rail;
			    break;
		    case 3:
			    neighborPlaceable = BuildingManager.GetNode(gameObject.transform.position + Vector3.left) as Rail;
			    break;
	    }

	    return neighborPlaceable;
    }

    private void UpdateOrientation()
    {
        bool verticalNode = NeighborRail(0) || NeighborRail(2);
        
        if (verticalNode)
        {
	        _straightTransform.gameObject.SetActive(true);
	        _cornerTransform.gameObject.SetActive(false);
	        _tIntersectionTranform.gameObject.SetActive(false);
	        _intersectionTransform.gameObject.SetActive(false);
            transform.eulerAngles = new Vector3(0f, 90f, 0f);
        }

        for (int i = 0; i < 4; i++)
        {
	        bool cornerNode = NeighborRail(i) && NeighborRail((i+1) % 4);
	        if (!cornerNode) continue;
	        _straightTransform.gameObject.SetActive(false);
	        _cornerTransform.gameObject.SetActive(true);
	        _tIntersectionTranform.gameObject.SetActive(false);
	        _intersectionTransform.gameObject.SetActive(false);
	        transform.eulerAngles = new Vector3(0f, 90f, 0f) * (i);
	        break;
        }
        
        for (int i = 0; i < 4; i++)
        {
	        bool cornerNode = NeighborRail((i+2) % 4) && NeighborRail(i) && NeighborRail((i+1) % 4);
	        if (!cornerNode) continue;
	        _straightTransform.gameObject.SetActive(false);
	        _cornerTransform.gameObject.SetActive(false);
	        _tIntersectionTranform.gameObject.SetActive(true);
	        _intersectionTransform.gameObject.SetActive(false);
	        transform.eulerAngles = new Vector3(0f, 90f, 0f) * (i);
	        break;
        }

        if (NeighborRail(0) && NeighborRail(1) && NeighborRail(2) && NeighborRail(3))
        {
	        _straightTransform.gameObject.SetActive(false);
	        _cornerTransform.gameObject.SetActive(false);
	        _tIntersectionTranform.gameObject.SetActive(false);
	        _intersectionTransform.gameObject.SetActive(true);
        }
    }

    public override bool IsNode()
    {
        bool verticalStreet = AdjacentNodes(0) && AdjacentNodes(2) && !AdjacentNodes(3) && !AdjacentNodes(1);
        bool horizontalStreet = !AdjacentNodes(0) && !AdjacentNodes(2) && AdjacentNodes(3) && AdjacentNodes(1);
        return !(verticalStreet || horizontalStreet); // Only corner rails are nodes
    }

    public override bool IsTraversable()
    {
        return true;
    }

    public override WayPoint GetTraversalVectors(int fromDirection, int toDirection)
    {
        Vector3 offset = transform.position;
		// Start Points
		if (fromDirection == -1)
		{
			switch (toDirection)
			{
				case Up:
					return new WayPoint(offset, offset + TraversalPoint.MiddleTop);
				case Right:
					return new WayPoint(offset, offset + TraversalPoint.MiddleRight);
				case Down:
					return new WayPoint(offset, offset + TraversalPoint.MiddleBottom);
				case Left:
					return new WayPoint(offset, offset + TraversalPoint.MiddleLeft);
			}
		}

		// End Points

		if (toDirection == -1)
		{
			switch (fromDirection)
			{
				case Up:
					return new WayPoint(offset + TraversalPoint.MiddleTop, offset);
				case Right:
					return new WayPoint(offset + TraversalPoint.MiddleRight, offset);
				case Down:
					return new WayPoint(offset + TraversalPoint.MiddleBottom, offset);
				case Left:
					return new WayPoint(offset + TraversalPoint.MiddleLeft, offset);
			}
		}

		// Straights

		if (fromDirection == Up && toDirection == Down)
		{
			return new WayPoint(TraversalPoint.MiddleTop + offset, TraversalPoint.MiddleBottom + offset);
		}
		
		if (fromDirection == Down && toDirection == Up)
		{
			return new WayPoint(TraversalPoint.MiddleBottom + offset, TraversalPoint.MiddleTop + offset);
		}
		
		if (fromDirection == Left && toDirection == Right)
		{
			return new WayPoint(TraversalPoint.MiddleLeft + offset, TraversalPoint.MiddleRight + offset);
		}
		
		if (fromDirection == Right && toDirection == Left)
		{
			return new WayPoint(TraversalPoint.MiddleRight + offset, TraversalPoint.MiddleLeft + offset);
		}

		// Inner Corners

		float innerCornerRadius = 0.5f;

		if (fromDirection == Up && toDirection == Left)
		{
			return new WayPoint(TraversalPoint.MiddleTop + offset, offset, TraversalPoint.MiddleLeft + offset, innerCornerRadius);
		}
		
		if (fromDirection == Up && toDirection == Right)
		{
			return new WayPoint(TraversalPoint.MiddleTop + offset, offset, TraversalPoint.MiddleRight + offset, innerCornerRadius);
		}

		if (fromDirection == Down && toDirection == Right)
		{
			return new WayPoint(TraversalPoint.MiddleBottom + offset, offset, TraversalPoint.MiddleRight + offset, innerCornerRadius);
		}
		
		if (fromDirection == Down && toDirection == Left)
		{
			return new WayPoint(TraversalPoint.MiddleBottom + offset, offset, TraversalPoint.MiddleLeft + offset, innerCornerRadius);
		}

		if (fromDirection == Left && toDirection == Down)
		{
			return new WayPoint(TraversalPoint.MiddleLeft + offset, offset, TraversalPoint.MiddleBottom + offset, innerCornerRadius);
		}
		
		if (fromDirection == Left && toDirection == Up)
		{
			return new WayPoint(TraversalPoint.MiddleLeft + offset, offset, TraversalPoint.MiddleTop + offset, innerCornerRadius);
		}

		if (fromDirection == Right && toDirection == Up)
		{
			return new WayPoint(TraversalPoint.MiddleRight + offset, offset, TraversalPoint.MiddleTop + offset, innerCornerRadius);
		}
		
		if (fromDirection == Right && toDirection == Down)
		{
			return new WayPoint(TraversalPoint.MiddleRight + offset, offset, TraversalPoint.MiddleBottom + offset, innerCornerRadius);
		}
		
		return new WayPoint(offset, offset);
    }
}