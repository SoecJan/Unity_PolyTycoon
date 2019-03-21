using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{

	// Lane points
	private static Vector3 topLeft;
	private static Vector3 topRight;
	private static Vector3 rightTop;
	private static Vector3 rightBottom;
	private static Vector3 bottomRight;
	private static Vector3 bottomLeft;
	private static Vector3 leftBottom;
	private static Vector3 leftTop;

	// Turn helper
	private static Vector3 centerTopRight;
	private static Vector3 centerTopLeft;
	private static Vector3 centerBottomRight;
	private static Vector3 centerBottomLeft;

	// Use this for initialization
	void Start () {

		if (topLeft != default(Vector3)) return;
		topLeft = new Vector3(-0.25f, 0f, 0.5f);
		topRight = new Vector3(0.25f, 0f, 0.5f);
		rightTop = new Vector3(0.5f, 0f, 0.25f);
		rightBottom = new Vector3(0.5f, 0f, -0.25f);
		bottomRight = new Vector3(0.25f, 0f, -0.5f);
		bottomLeft = new Vector3(-0.25f, 0f, -0.5f);
		leftBottom = new Vector3(-0.5f, 0f, -0.25f);
		leftTop = new Vector3(-0.5f, 0f, 0.25f);

		centerTopRight = new Vector3(-0.25f, 0f, 0.25f);
		centerTopLeft = new Vector3(0.25f, 0f, 0.25f);
		centerBottomRight = new Vector3(0.25f, 0f, -0.25f);
		centerBottomLeft = new Vector3(-0.25f, 0f, -0.25f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
