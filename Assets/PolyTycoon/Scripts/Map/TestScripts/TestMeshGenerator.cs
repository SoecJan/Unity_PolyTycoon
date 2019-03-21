using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMeshGenerator : MonoBehaviour {

	const int meshScale = 1;

	public int meshSize;
	public Material material;

	void Awake() {
		GameObject chunk = new GameObject("Chunk");
		chunk.AddComponent<MeshFilter>();
		chunk.GetComponent<MeshFilter>().sharedMesh = CreateMesh(1, 0.2f);
		chunk.AddComponent<MeshRenderer>();
		chunk.GetComponent<MeshRenderer>().material = material;
	}

	Mesh CreateMesh(float width, float height) {
		Mesh m = new Mesh();
		m.name = "ScriptedMesh";
		m.vertices = new Vector3[] {
		 new Vector3(-width, 0, -width),
		 new Vector3(-width, 0, width),
		 new Vector3(width, 0, width),
		 new Vector3(width, 0, -width)
	 };
		m.uv = new Vector2[] {
		 new Vector2 (0, 0),
		 new Vector2 (0, 1),
		 new Vector2(1, 1),
		 new Vector2 (1, 0)
	 };
		m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
		m.RecalculateNormals();

		return m;
	}
}
