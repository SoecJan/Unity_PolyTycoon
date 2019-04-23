using UnityEngine;
using Debug = UnityEngine.Debug;

public class Street : PathFindingNode
{
	[SerializeField] private Transform _cornerTransform;
	[SerializeField] private Transform _straightTransform;
	[SerializeField] private Transform _tIntersectionTranform;
	[SerializeField] private Transform _intersectionTransform;

	#region Methods
	protected override void Initialize()
	{
		base.Initialize();
		IsDraggable = true;
	}
	
	private void UpdateOrientation()
	{
		bool verticalNode = NeightborAlign(0) || NeightborAlign(2);
        
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
			bool cornerNode = NeightborAlign(i) && NeightborAlign((i+1) % 4);
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
			bool cornerNode = NeightborAlign((i+2) % 4) && NeightborAlign(i) && NeightborAlign((i+1) % 4);
			if (!cornerNode) continue;
			_straightTransform.gameObject.SetActive(false);
			_cornerTransform.gameObject.SetActive(false);
			_tIntersectionTranform.gameObject.SetActive(true);
			_intersectionTransform.gameObject.SetActive(false);
			transform.eulerAngles = new Vector3(0f, 90f, 0f) * (i);
			break;
		}

		if (NeightborAlign(0) && NeightborAlign(1) && NeightborAlign(2) && NeightborAlign(3))
		{
			_straightTransform.gameObject.SetActive(false);
			_cornerTransform.gameObject.SetActive(false);
			_tIntersectionTranform.gameObject.SetActive(false);
			_intersectionTransform.gameObject.SetActive(true);
		}
	}

	private bool NeightborAlign(int i)
	{
		SimpleMapPlaceable neighborPlaceable = null;
		switch (i)
		{
			case 0:
				neighborPlaceable = BuildingManager.GetNode(gameObject.transform.position + Vector3.forward);
				break;
			case 1:
				neighborPlaceable = BuildingManager.GetNode(gameObject.transform.position + Vector3.right);
				break;
			case 2:
				neighborPlaceable = BuildingManager.GetNode(gameObject.transform.position + Vector3.back);
				break;
			case 3:
				neighborPlaceable = BuildingManager.GetNode(gameObject.transform.position + Vector3.left);
				break;
		}

		return (neighborPlaceable && !(neighborPlaceable as Rail));
	}

	public override void OnPlacement()
	{
		base.OnPlacement();
		transform.name = "Street" + transform.position.ToString();
		for (int i = 0; i < 4; i++)
		{
			Street street = AdjacentNodes(i) as Street;
			if (street) street.UpdateOrientation();
		}
		UpdateOrientation();
	}

	public override bool IsNode()
	{
		bool verticalStreet = AdjacentNodes(0) && AdjacentNodes(2) && !AdjacentNodes(3) && !AdjacentNodes(1);
		bool horizontalStreet = !AdjacentNodes(0) && !AdjacentNodes(2) && AdjacentNodes(3) && AdjacentNodes(1);
		return !(verticalStreet || horizontalStreet); // Only corner streets are nodes
	}

	public override bool IsTraversable()
	{
		return true;
	}
	#endregion
}