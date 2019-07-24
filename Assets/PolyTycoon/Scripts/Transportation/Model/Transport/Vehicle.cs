using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(VehicleMover))]
public class Vehicle : MonoBehaviour
{
	#region Attributes

	public enum PathType { Water, Road, Rail, Air };
	
	private int _routeIndex = -1;
	private List<Path> _pathList;
	[SerializeField] private PathType _pathType = PathType.Road;
	[SerializeField] private VehicleMover _mover;
	[SerializeField] private Sprite _sprite;
	private Outline _outline;
	#endregion

	#region Getter & Setter

	public Outline Outline
	{
		get => _outline;
		set => _outline = value;
	}

	public VehicleMover Mover {
		get => _mover;
		set => _mover = value;
	}

	public Sprite Sprite => _sprite;

	public List<Path> PathList {
		get => _pathList;

		set {
			_pathList = value;
			_routeIndex = -1;
		}
	}

	public int RouteIndex => _routeIndex;

	public PathType MoveType
	{
		get => _pathType;
		set => _pathType = value;
	}

	#endregion

	#region Methods
	// Use this for initialization
	private void Start()
	{
		if (!Mover)
		{
			Mover = gameObject.AddComponent<VehicleMover>();
		}

		if (!Outline)
		{
			Outline = gameObject.AddComponent<Outline>();
			Outline.OutlineMode = Outline.Mode.OutlineAll;
			Outline.OutlineColor = Color.yellow;
			Outline.OutlineWidth = 5f;
			Outline.enabled = false;
		}
		
		Mover.OnArrive += OnArrive;
	}

	protected virtual void OnArrive()
	{
		_routeIndex = (RouteIndex + 1) % _pathList.Count;
		_mover.WayPointList = _pathList[RouteIndex].WayPoints;
	}
	#endregion
}