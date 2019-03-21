using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWalker : MonoBehaviour
{
	public enum WayType {Corner, Straight}

	[Range(0f, 1f)]
	public float progress = 0f;

	public float speed = 1f;

	private List<WayPoint> _wayPoints;
	private int currentIndex = 0;

	// Use this for initialization
	void Start ()
	{
		_wayPoints = new List<WayPoint>();
		_wayPoints.Add(new WayPoint(new Vector3[] {new Vector3(0.5f, 0f, 0f), new Vector3(4.5f, 0f, 0f)}));
		_wayPoints.Add(new WayPoint(new Vector3[] { new Vector3(4.5f, 0f, 0f), new Vector3(5f, 0f, 0f), new Vector3(5f, 0f, 0.5f) }));
		_wayPoints.Add(new WayPoint(new Vector3[] { new Vector3(5f, 0f, 0.5f), new Vector3(5f, 0f, 4.5f) }));
		_wayPoints.Add(new WayPoint(new Vector3[] { new Vector3(5f, 0, 4.5f), new Vector3(5f, 0f, 5f), new Vector3(4.5f, 0f, 5f)}));
		_wayPoints.Add(new WayPoint(new Vector3[] { new Vector3(4.5f, 0f, 5f), new Vector3(0.5f, 0f, 5f) }));
		_wayPoints.Add(new WayPoint(new Vector3[] { new Vector3(0.5f, 0f, 5f), new Vector3(0f, 0f, 5f), new Vector3(0f, 0f, 4.5f) }));
		_wayPoints.Add(new WayPoint(new Vector3[] { new Vector3(0f, 0f, 4.5f), new Vector3(0f, 0f, 0.5f) }));
		_wayPoints.Add(new WayPoint(new Vector3[] { new Vector3(0f, 0f, 0.5f), new Vector3(0f, 0f, 0f), new Vector3(0.5f, 0f, 0f) }));
	}

	void OnDrawGizmos()
	{
		if (_wayPoints == null) return;
		foreach (WayPoint wayPoint in _wayPoints)
		{
			foreach (Vector3 position in wayPoint.points)
			{
				Gizmos.DrawSphere(position, 0.5f);
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		float distance = Time.deltaTime * speed;
		WayPoint currentWayPoint = _wayPoints[currentIndex];
		if (currentWayPoint.wayType.Equals(WayType.Straight))
		{
			Vector3 difference = currentWayPoint.points[1] - transform.position;
			Vector3 direction = difference.normalized;
			Vector3 futurePosition = transform.position + (direction * speed * Time.deltaTime);
			Vector3 futureDifference = currentWayPoint.points[1] - futurePosition;

			if (difference.magnitude < futureDifference.magnitude)
			{
				transform.position = currentWayPoint.points[1];
				currentIndex = (currentIndex + 1) % _wayPoints.Count;
			}
			else
			{
				transform.position = futurePosition;
			}
		}
		else
		{
			// TODO: Cleanup and change GetPoint to Circle function with Center Point and Angle
			float circumferenceDistanceToAngle = _wayPoints[0].GetAngle(0.5f, Time.deltaTime * speed); // Angle at a Radius of 0.5f Units
			progress += circumferenceDistanceToAngle / 90f; // 90 Degree Turn at each corner
			transform.position = GetPoint(currentWayPoint.points[0], currentWayPoint.points[1], currentWayPoint.points[2], progress);
			transform.LookAt(transform.position + GetFirstDerivative(currentWayPoint.points[0], currentWayPoint.points[1], currentWayPoint.points[2], progress).normalized);
			if (!(progress > 1f)) return;
			currentIndex = (currentIndex + 1) % _wayPoints.Count;
			progress = 0f;
		}
		
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

public struct WayPoint
{
	public TestWalker.WayType wayType;
	public Vector3[] points;

	public WayPoint(Vector3[] points)
	{
		this.points = points;
		this.wayType = points.Length == 2 ? TestWalker.WayType.Straight : TestWalker.WayType.Corner;
	}

	public float GetAngle(float radius, float distance)
	{
		return (distance * 180) / (Mathf.PI * radius);
	}

	public float GetCircumference(float radius)
	{
		return 2 * Mathf.PI * radius;
	}

	public Vector2 PointOnCircle(float radius, float angleInDegree, Vector2 origin)
	{
		float x = (float) (radius * Mathf.Cos(angleInDegree * Mathf.PI / 180f)) + origin.x;
		float y = (float) (radius * Mathf.Sin(angleInDegree * Mathf.PI / 180f)) + origin.y;
		return new Vector2(x, y);
	}
}