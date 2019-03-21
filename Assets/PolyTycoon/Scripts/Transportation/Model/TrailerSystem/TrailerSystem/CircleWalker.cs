using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleWalker : MonoBehaviour
{

	[SerializeField] private float r = 1f;
	[SerializeField] private Vector3 center = Vector3.zero;
	[SerializeField] private float _speed = 1f;

	// Use this for initialization
	void Start () {
		transform.position = new Vector3(center.x, center.y, center.z + r);
	}
	
	// Update is called once per frame
	void Update ()
	{
		float xp1 = transform.position.x;
		float yp1 = transform.position.y;
		float d = _speed * Time.deltaTime;
		float value = 2f*Mathf.PI / 1f;
		float xp2 = xp1 + r * Mathf.Sin(value);
		float yp2 = yp1 - r * (1 - Mathf.Cos(value));
		transform.position = new Vector3(xp2, yp2, 0f);
	}
}
