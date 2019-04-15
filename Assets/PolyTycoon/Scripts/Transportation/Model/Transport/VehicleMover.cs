using System;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMover : MonoBehaviour
{
	public bool _wait = true;
	private bool _isWayPointReached = false;
	private float progress = 0f;
	private float _speed = 2f;
	private List<WayPoint> _wayPointList;
	private int _wayPointIndex;
	//private WayPoint _previousWayPoint;

	// Start is called before the first frame update
	void Start()
	{
		OnArrive();
	}

	// Update is called once per frame
	void Update()
	{
		if (_wait) return;
		Move();
	}

	public float MaxSpeed
	{
		get { return _speed; }
	}

	public Action OnArrive { get; set; }

	public List<WayPoint> WayPointList
	{
		get { return _wayPointList; }
		set
		{
			_wayPointList = value;
			//_previousWayPoint = _wayPointList[0];
			_wayPointIndex = 0;
			_wait = false;
		}
	}

	private void Move()
	{
		if (_wayPointList == null) return;
		WayPoint currentWayPoint = _wayPointList[_wayPointIndex];
		
		// Drive Straight
		if (currentWayPoint.TraversalVectors.Length == 2)
		{
			if (DriveStraightToDestination(currentWayPoint.TraversalVectors[1]))
			{
				_wayPointIndex = (_wayPointIndex + 1); // Straight intersection
			}
		}
		else
		{
			// Drive straight to Corner
			if (!_isWayPointReached)
			{
				_isWayPointReached = DriveStraightToDestination(currentWayPoint.TraversalVectors[0]); // Drive to Corner intersection
			}
			else
			{
				// Drive smooth corner
				float circumferenceDistanceToAngle = GetAngle(currentWayPoint.Radius, Time.deltaTime * _speed); // Angle at a Radius of 0.5f Units
				progress += circumferenceDistanceToAngle / 90f; // 90 Degree Turn at each corner
				transform.position = GetPoint(currentWayPoint.TraversalVectors[0], currentWayPoint.TraversalVectors[1], currentWayPoint.TraversalVectors[2], progress);
				transform.LookAt(transform.position + GetFirstDerivative(currentWayPoint.TraversalVectors[0], currentWayPoint.TraversalVectors[1], currentWayPoint.TraversalVectors[2], progress).normalized);
				if (!(progress > 1f)) return;
				_wayPointIndex = (_wayPointIndex + 1);
				progress = 0f;
				_isWayPointReached = false;
				//if (Input.GetButtonDown("Jump"))
				//{
				//	_wayPointIndex = (_wayPointIndex + 1) % _wayPointList.Count;
				//	Debug.Log(_wayPointIndex + ", " + currentWayPoint.ToString());
				//}
			}
		}

		if (_wayPointIndex != _wayPointList.Count) return;
		_wait = true;
		OnArrive();
	}

	bool DriveStraightToDestination(Vector3 destinationVector3)
	{
		Vector3 difference = destinationVector3 - transform.position;
		Vector3 direction = difference.normalized;
		Vector3 futurePosition = transform.position + (direction * _speed * Time.deltaTime);
		Vector3 futureDifference = destinationVector3 - futurePosition;

		if (difference.magnitude > futureDifference.magnitude)
		{
			transform.LookAt(futurePosition);
			transform.position = futurePosition;
			return false;
		}
		transform.position = destinationVector3;
		
		return true;
	}

	public static float GetAngle(float radius, float distance)
	{
		return (distance * 180) / (Mathf.PI * radius);
	}

	public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
	}

	public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		return
			2f * (1f - t) * (p1 - p0) +
			2f * t * (p2 - p1);
	}
}