using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Mover : MonoBehaviour
{
	[SerializeField] private float speed;
	[SerializeField] private Vector3 targetPoint;

	// Use this for initialization
	void Start () {
		transform.LookAt(targetPoint);
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 difference = targetPoint - transform.position;
		transform.position = transform.position + (speed * Time.deltaTime * difference.normalized);
	}
}
