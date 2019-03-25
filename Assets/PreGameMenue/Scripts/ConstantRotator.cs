using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotator : MonoBehaviour {

	[SerializeField] private Transform _rotatedTransform;
	[SerializeField] private Vector3 _rotationAxis;

	void Start()
	{
		if (!_rotatedTransform) _rotatedTransform = transform;
	}

	void Update ()
	{
		_rotatedTransform.eulerAngles += _rotationAxis;
	}
}
