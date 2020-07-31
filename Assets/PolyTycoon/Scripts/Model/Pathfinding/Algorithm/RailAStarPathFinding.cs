public class RailAStarPathFinding : NetworkAStarPathFinding
{
	public override Path FindPath(PathFindingNode startNode, PathFindingNode endNode)
	{
		Trainstation fromStation = startNode.GetComponent<Trainstation>();
		Trainstation toStation = endNode.GetComponent<Trainstation>();
		if (fromStation && toStation)
		{
			return base.FindPath(fromStation.AccessRail, toStation.AccessRail);
		}
		return null;
	}
}