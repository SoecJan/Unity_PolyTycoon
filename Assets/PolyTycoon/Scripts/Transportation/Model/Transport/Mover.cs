using System.Collections.Generic;
using UnityEngine;

namespace Assets.PolyTycoon.Scripts.Transportation.Model.Transport
{
	/// <summary>
	/// This class takes care of a moving vehicle on the ground. 
	/// </summary>
	public class Mover : MonoBehaviour {
	
		#region Attributes
		// Callback called as Vehicle arrives at a GameObject
		public System.Action OnArriveAction;

		private List<Vector3> _wayPointList;
		private Vector3 _previousWayPoint;
		private int _currentTargetIndex = 0;

		[Header("Vehicle Settings")]
		[Tooltip("Top-Speed of this vehicle.")]
		[SerializeField] private float _speed = 3f;
		[SerializeField] private float _minSpeed = 0.05f;
		private bool _wait = false;
		[Tooltip("Time this vehicle has to wait at a station.")]
		[SerializeField] private float _waitTime = 2f;
		private float _elapsedTime;

		[Header("Visual Settings")]
		[Tooltip("The Object that is going to be rotated in the direction of movement.")]
		private GameObject _visualObjectToRotate;
		[Tooltip("If true the visualObjectToRotate will constantly be rotated to face the target transform.")]
		private bool _lookAtTargetConstantly = false;
		private float _zRotationOffset = 180f;

		[Header("Acceleration Settings")]
		[Tooltip("Distance from previous target for acceleration to take effect")]
		[SerializeField] private float _accelerationDistance = 1f;
		[Tooltip("How strong the vehicle will accelerate.")]
		[Range(0,1)]
		[SerializeField] private float _accelerationStrength = 1f;

		[Header("Breaking Settings")]
		[Tooltip("Distance to target for breaking to take effect")]
		[SerializeField] private float _breakDistance = 2f;
		[Tooltip("How strong the vehicle will break.")]
		[Range(0,1)]
		[SerializeField] private float _breakStrength = 1f;

		[Header("Other Settings")]
		[Tooltip("Teleports Vehicle to Waypoint, if close enough.")]
		[SerializeField] private float _teleportDistance = 0.05f;

		public List<Vector3> WayPointList {
			get {
				return _wayPointList;
			}

			set {
				_wayPointList = value;
			}
		}

		public float Speed {
			get {
				return _speed;
			}

			set {
				_speed = value;
			}
		}

		public float MinSpeed {
			get {
				return _minSpeed;
			}

			set {
				_minSpeed = value;
			}
		}

		public float WaitTime {
			get {
				return _waitTime;
			}

			set {
				_waitTime = value;
			}
		}

		public GameObject VisualObjectToRotate {
			get {
				return _visualObjectToRotate;
			}

			set {
				_visualObjectToRotate = value;
			}
		}

		public bool LookAtTargetConstantly {
			get {
				return _lookAtTargetConstantly;
			}

			set {
				_lookAtTargetConstantly = value;
			}
		}

		public float ZRotationOffset {
			get {
				return _zRotationOffset;
			}

			set {
				_zRotationOffset = value;
			}
		}

		public float AccelerationDistance {
			get {
				return _accelerationDistance;
			}

			set {
				_accelerationDistance = value;
			}
		}

		public float AccelerationStrength {
			get {
				return _accelerationStrength;
			}

			set {
				_accelerationStrength = value;
			}
		}

		public float BreakDistance {
			get {
				return _breakDistance;
			}

			set {
				_breakDistance = value;
			}
		}

		public float BreakStrength {
			get {
				return _breakStrength;
			}

			set {
				_breakStrength = value;
			}
		}

		public float TeleportDistance {
			get {
				return _teleportDistance;
			}

			set {
				_teleportDistance = value;
			}
		}

		private Vector3 TargetPosition
		{
			get { return WayPointList[_currentTargetIndex]; }
		}
		#endregion

		#region Default Methods
		private void Update () {
			if (!_wait) {
				Move ();
			} else {
				_elapsedTime += Time.deltaTime;
				if (_elapsedTime >= WaitTime ) {
					_wait = false;
					_elapsedTime = 0f;
				}
			}
		}
		#endregion

		#region Movement
		void Move() {
			if (WayPointList.Count == 0 || _currentTargetIndex > WayPointList.Count)
				return; 
			// Calculate needed Vectors
			Vector3 distanceToTargetV3 = WayPointList[_currentTargetIndex] - transform.position;
			Vector3 targetDirection = distanceToTargetV3.normalized;
			//if (fixedZAxis) 
			//	targetDirection.z = 0f;

			// Calculate needed Distances
			float currentTargetDistanceXz = Mathf.Abs (distanceToTargetV3.x) + Mathf.Abs(distanceToTargetV3.z);
			float previousTargetDistanceXz = -1f;
		
			Vector3 previousTargetDistance = _previousWayPoint - transform.position;
			previousTargetDistanceXz = Mathf.Abs (previousTargetDistance.x) + Mathf.Abs(previousTargetDistance.z);
		

			// Calculate BreakMultiplier for speeding up an slowing down
			float breakMultiplier = CalculateBreakMultiplier(currentTargetDistanceXz, previousTargetDistanceXz);
			Vector3 velocity = targetDirection * Speed * Time.deltaTime * breakMultiplier;

			// Use results
			if (currentTargetDistanceXz > TeleportDistance) {
				// Move as vehicle has not arrived at it's target location
				transform.Translate (velocity);
			} else {
				// Close enough to teleport to waypoint (Needed to keep being aligned to the grid)
				if (_currentTargetIndex == WayPointList.Count-1)
				{
					Arrive(WayPointList[_currentTargetIndex]);
					_currentTargetIndex = 0;
					RotateToTarget(distanceToTargetV3);
				}
				else
				{
					SelectNextWaypoint();
					RotateToTarget(distanceToTargetV3);
				}
			
			}

			// Look at target
			if (LookAtTargetConstantly) {
				RotateToTarget (distanceToTargetV3);
			}
		}

		// Calculated a multiplier that is between 0 and 1 to decrease our speed when starting/stopping.
		float CalculateBreakMultiplier(float targetDistance, float previousTargetDistance) {
			if (targetDistance < BreakDistance) {
				// Break
				float multiplier = (targetDistance * BreakStrength) / BreakDistance;
				return Mathf.Max(multiplier, MinSpeed);
			} else if (previousTargetDistance >= 0f) {
				// Accelerate
				if (previousTargetDistance < AccelerationDistance && previousTargetDistance > TeleportDistance) {
					float multiplier = (previousTargetDistance * AccelerationStrength) / AccelerationDistance;
					return Mathf.Max(multiplier, MinSpeed);
				}
			}
			// Fullspeed
			return 1f;
		}

		// Rotates a given Transforms z-Axis to look at a target Vector3
		private void RotateToTarget(Vector3 targetDistanceV3)
		{
			if (VisualObjectToRotate)
			{
				targetDistanceV3.Normalize();
				float rotZ = Mathf.Atan2(targetDistanceV3.z, targetDistanceV3.x) * Mathf.Rad2Deg;
				VisualObjectToRotate.transform.rotation = Quaternion.Euler(0f, 0f, rotZ - ZRotationOffset);
			}
		}
		#endregion

		#region Navigation
		// Teleports this vehicle to the target and selects the next waypoint from the wayPointList 
		private void SelectNextWaypoint() {
			// Teleport
			transform.position = WayPointList [_currentTargetIndex];
			// Select next Waypoint
			_previousWayPoint = WayPointList [_currentTargetIndex];
			_currentTargetIndex++;
			_currentTargetIndex %= WayPointList.Count;
		}

		// Called as this vehicle arrives at a given GameObject.
		private void Arrive(Vector3 arrivedTargetObject) {
			_wait = true;
			OnArriveAction ();
		}
		#endregion
	}
}
