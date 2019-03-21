using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PolyTycoon.Scripts.Transportation.Model.Transport
{
	public class VehicleMover : MonoBehaviour
	{
		#region Attributes
		private System.Action _onArrive;
		private bool _wait = true;
		private List<Vector3> _wayPointList;
		private int _wayPointIndex;
		private Vector3 _previousWayPoint;

		[Header("General")]
		[SerializeField] private Transform _visibleTransform;
		[SerializeField] private Transform _moveTransform;
		[Header("Vehicle Settings")]
		[SerializeField] private float _cornerSpeed;
		[SerializeField] private float _breakStrength;
		[SerializeField] private float _accelerationStrength;
		[SerializeField] private float _maxSpeed;
		[SerializeField] private float _weight;
		[SerializeField] private AnimationCurve _speedCurve;
		#endregion

		#region Getter & Setter
		private Vector3 TargetWayPoint {
			get { return _wayPointList[_wayPointIndex]; }
		}

		public Action OnArrive {
			get {
				return _onArrive;
			}

			set {
				_onArrive = value;
			}
		}

		public List<Vector3> WayPointList {
			get { return _wayPointList; }
			set {
				_wayPointList = value;
				_previousWayPoint = _wayPointList[0];
				_wayPointIndex = 0;
				_wait = false;
			}
		}

		public float MaxSpeed {
			get {
				return _maxSpeed;
			}

			set {
				_maxSpeed = value;
			}
		}
		#endregion

		#region Methods
		private void Start()
		{
			if (!_moveTransform) _moveTransform = transform;
			OnArrive();
		}

		private void Update()
		{
			if (_wait) return;
			Move();
		}

		private void Move()
		{
			if (_wayPointList == null) return;
			Vector3 targetPositionDifference = TargetWayPoint - _moveTransform.position;
			//Vector3 previousPositionDifference = _previousWayPoint - _moveTransform.position;
			Vector3 targetDirection = targetPositionDifference.normalized;
			Vector3 velocityVector3 = targetDirection * Time.deltaTime * MaxSpeed;
			Vector3 futurePositionDifference = TargetWayPoint - (_moveTransform.position + velocityVector3);

			if (targetPositionDifference.sqrMagnitude < futurePositionDifference.sqrMagnitude)
			{
				velocityVector3 = targetPositionDifference;
				_previousWayPoint = TargetWayPoint;
				_wayPointIndex++;
				if (_wayPointIndex < _wayPointList.Count)
				{
					targetPositionDifference = TargetWayPoint - _moveTransform.position;
					targetDirection = targetPositionDifference.normalized;
					Rotate(targetDirection);
				}
				else
				{
					_wait = true;
					OnArrive();
				}
			}
			else if (targetPositionDifference.sqrMagnitude < _cornerSpeed)
			{
				float multiplier = _speedCurve.Evaluate(targetPositionDifference.sqrMagnitude / _cornerSpeed);
				velocityVector3 *= multiplier;
			}
			//else if (currentPositionDifference.sqrMagnitude < _weight / _breakStrength)
			//{
			//	float speedMultiplier = Mathf.Min(currentPositionDifference.sqrMagnitude, 1);
			//	velocityVector3 = velocityVector3 * Mathf.Max(speedMultiplier, _cornerSpeed);
			//}
			//else if ((_previousWayPoint - _moveTransform.position).sqrMagnitude < _weight / _accelerationStrength)
			//{
			//	float speedMultiplier = Mathf.Max((_previousWayPoint - _moveTransform.position).sqrMagnitude, _cornerSpeed);
			//	velocityVector3 = velocityVector3 * speedMultiplier;
			//}
			_moveTransform.Translate(velocityVector3);
		}

		private void Rotate(Vector3 targetVector3)
		{
			if (!_visibleTransform) return;
			targetVector3.Normalize();
			float rotZ = Mathf.Atan2(targetVector3.z, targetVector3.x) * Mathf.Rad2Deg;
			if (Math.Abs(rotZ - 180) < 1 || Math.Abs(rotZ + 180) < 1 || Math.Abs(rotZ - 360) < 1 || Math.Abs(rotZ) < 1)
			{
				rotZ += 180;
			}
			_visibleTransform.rotation = Quaternion.Euler(0f, rotZ, 0f);
		}
		#endregion
	}
}
