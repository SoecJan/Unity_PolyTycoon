public class RailAStarPathFinding : NetworkAStarPathFinding
{
	public override Path FindPath(PathFindingNode startNode, PathFindingNode endNode)
	{
		Trainstation fromStation = startNode as Trainstation;
		Trainstation toStation = endNode as Trainstation;
		if (fromStation && toStation)
		{
			return base.FindPath(fromStation.AccessRail, toStation.AccessRail);
		}
		return null;
	}
}