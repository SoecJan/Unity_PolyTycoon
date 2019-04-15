using System.Collections.Generic;
using UnityEngine;

// Wraps all MeshData in one Object
public class TerrainChunk
{

	/* Attributes */

	const float colliderGenerationDistanceThreshold = 10;
	public event System.Action<TerrainChunk, bool> onVisibilityChanged;
	public Vector2 coord;
	public GameObject meshObject;

	Vector2 sampleCentre;
	Bounds bounds;

	MeshRenderer meshRenderer;
	public MeshFilter meshFilter;
	MeshCollider meshCollider;

	LODInfo[] detailLevels;
	LODMesh[] lodMeshes;
	int colliderLODIndex;

	HeightMap heightMap;
	bool heightMapReceived;
	int previousLODIndex = -1;
	bool hasSetCollider;
	public static float maxViewDst;

	HeightMapSettings heightMapSettings;
	MeshSettings meshSettings;
	private Material defaultMaterial;
	
	// Biome
	private BiomeSettings biomeSettings;
	private BiomeData[] _biomeDatas;

	static Transform viewer;

	/* Constructors */

	public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, BiomeSettings biomeSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewerTransform, Material material, GameObject waterMeshObject)
	{
		this.coord = coord;
		this.detailLevels = detailLevels;
		this.colliderLODIndex = colliderLODIndex;
		this.heightMapSettings = heightMapSettings;
		this.meshSettings = meshSettings;
		this.biomeSettings = biomeSettings;
		viewer = viewerTransform;
		//Debug.Log("Chunk Coordinates: " + coord + "; Num Verts Per Line: " + meshSettings.numVertsPerLine);

		sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
		Vector2 position = coord * meshSettings.meshWorldSize; // Calculate the position of this TerrainChunk
		bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

		// Instantiate the new object
		meshObject = new GameObject("Terrain Chunk: " + coord.ToString());
		GameObject.Instantiate(waterMeshObject, meshObject.transform);
		meshObject.layer = LayerMask.NameToLayer("Terrain");
		meshRenderer = meshObject.AddComponent<MeshRenderer>();
		meshFilter = meshObject.AddComponent<MeshFilter>();
		meshCollider = meshObject.AddComponent<MeshCollider>();
		defaultMaterial = material;
		meshRenderer.material = material;

		// Set the GameObjects position and parent
		meshObject.transform.position = new Vector3(position.x, 0, position.y);
		meshObject.transform.parent = parent;
		SetVisible(false);

		maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;

		lodMeshes = new LODMesh[detailLevels.Length];
		for (int i = 0; i < detailLevels.Length; i++)
		{
			lodMeshes[i] = new LODMesh(detailLevels[i].lod);
			lodMeshes[i].updateCallback += UpdateTerrainChunk;
			if (i == colliderLODIndex)
			{
				lodMeshes[i].updateCallback += UpdateCollisionMesh;
			}
		}
	}

	/* Getter & Setter */

	#region Getter & Setter
	Vector2 viewerPosition {
		get {
			return new Vector2(viewer.position.x, viewer.position.z);
		}
	}

	public HeightMap HeightMap {
		get {
			return heightMap;
		}

		set {
			heightMap = value;
		}
	}

	public void SetVisible(bool visible)
	{
		meshObject.SetActive(visible);
	}

	public bool IsVisible()
	{
		return meshObject.activeSelf;
	}

	public bool HasMesh()
	{
		return lodMeshes[0].hasMesh;
	}

	public Vector3 GetPositionMultiplier()
	{
		return new Vector3(coord.x * ((float)meshSettings.numVertsPerLine - 3), 0f, coord.y * ((float)meshSettings.numVertsPerLine - 3));
	}
	#endregion
	/* Methods */

	// Called by TerrainGenerator 86 after creating a new TerrainChunk
	public void Load()
	{
		// Request HeightMapData on a different Thread. OnHeightMapReceived is the callback function
		ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine,
			heightMapSettings, sampleCentre), OnHeightMapReceived);

		ThreadedDataRequester.RequestData(() => BiomeGenerator.GenerateBiomeData(biomeSettings, meshSettings.numVertsPerLine -3, meshSettings.numVertsPerLine -3, sampleCentre + new Vector2(22.5f, 22.5f)), OnBiomeDataReceived);
	}

	// Callback function on ThreadedDataRequester invoked by this.Load();
	void OnHeightMapReceived(object heightMapObject)
	{
		this.heightMap = (HeightMap)heightMapObject;
		heightMapReceived = true;
		UpdateTerrainChunk();
	}

	#region Biome
	void OnBiomeDataReceived(object biomeData)
	{
		_biomeDatas = (BiomeData[])biomeData;

		foreach (BiomeData biome in _biomeDatas)
		{
			Texture2D texture2D = new Texture2D(meshSettings.numVertsPerLine -3, meshSettings.numVertsPerLine-3);
			for (int x = 0; x < texture2D.width; x++)
			{
				for (int y = 0; y < texture2D.height; y++)
				{
					texture2D.SetPixel(x, y, GenerateColor(biome.ArrayData[x, texture2D.height- 1-y], biome.ColorMultiplier));
				}
			}
			texture2D.Apply();
			Material biomeMaterial = new Material(Shader.Find("Specular"));
			biomeMaterial.mainTexture = texture2D;
			biome.Material = biomeMaterial;
		}
	}

	private Color GenerateColor(float value, Vector3 colorMultiplier)
	{
		return new Color(value * colorMultiplier.x, value * colorMultiplier.y, value * colorMultiplier.z);
	}

	public BiomeData GetBiomeData(BiomeGenerator.Biome biome)
	{
		foreach (BiomeData biomeData in _biomeDatas)
		{
			if (biomeData.Biome.Equals(biome)) return biomeData;
		}
		return null;
	}

	public void ShowBiome(BiomeGenerator.Biome biome)
	{
		if (biome.Equals(BiomeGenerator.Biome.None))
		{
			meshRenderer.material = defaultMaterial;
		}
		else
		{
			Material biomeMaterial = GetBiomeData(biome).Material;
			meshRenderer.material = biomeMaterial;
		}
	}
	#endregion

	#region Chunk Update
	public void UpdateTerrainChunk()
	{
		if (heightMapReceived)
		{
			float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
			bool wasVisible = IsVisible();
			bool visible = viewerDstFromNearestEdge <= maxViewDst;

			if (visible)
			{
				int lodIndex = 0;

				for (int i = 0; i < detailLevels.Length - 1; i++)
				{
					if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold)
					{
						lodIndex = i + 1;
					}
					else
					{
						break;
					}
				}

				if (lodIndex != previousLODIndex)
				{
					LODMesh lodMesh = lodMeshes[lodIndex];
					if (lodMesh.hasMesh)
					{
						previousLODIndex = lodIndex;
						meshFilter.mesh = lodMesh.mesh;
					}
					else if (!lodMesh.hasRequestedMesh)
					{
						lodMesh.RequestMesh(heightMap, meshSettings);
					}
				}
			}
			if (wasVisible != visible)
			{
				SetVisible(visible);
				onVisibilityChanged(this, visible);
			}
		}
	}

	public void UpdateCollisionMesh()
	{
		if (!hasSetCollider)
		{
			float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);

			if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstThreshold)
			{
				if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
				{
					lodMeshes[colliderLODIndex].RequestMesh(heightMap, meshSettings);
				}
			}

			//if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold) {
			if (lodMeshes[colliderLODIndex].hasMesh)
			{
				meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
				hasSetCollider = true;
			}
			//}
		}
	}
	#endregion
}

class LODMesh
{

	public Mesh mesh;
	public bool hasRequestedMesh;
	public bool hasMesh;
	int lod;
	public event System.Action updateCallback;

	public LODMesh(int lod)
	{
		this.lod = lod;
	}

	// Callback function for ThreadedDataRequester
	void OnMeshDataReceived(object meshDataObject)
	{
		mesh = ((MeshData)meshDataObject).CreateMesh();
		hasMesh = true;

		updateCallback();
	}

	public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
	{
		hasRequestedMesh = true;
		ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
	}
}