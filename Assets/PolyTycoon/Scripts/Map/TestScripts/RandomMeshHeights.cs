using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMeshHeights : MonoBehaviour {

	public MeshFilter meshFilter;

	public void randomizeMesh() {
		Mesh mesh = meshFilter.sharedMesh;

		Vector3[] verts = mesh.vertices;

		for (int i = 0; i < verts.Length; i++) {
			verts[i] = new Vector3(verts[i].x, Random.Range(0.0f, 2.0f), verts[i].z);
		}

		mesh.vertices = verts;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
