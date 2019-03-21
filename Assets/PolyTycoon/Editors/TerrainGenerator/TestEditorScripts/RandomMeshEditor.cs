using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(RandomMeshHeights))]
public class RandomMeshEditor : Editor {
	public override void OnInspectorGUI() {
		RandomMeshHeights randomMeshHeights = (RandomMeshHeights)target;

		if (GUILayout.Button("Generate")) {
			randomMeshHeights.randomizeMesh();
		}
	}
}
#endif
