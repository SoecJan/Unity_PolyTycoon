using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CityGenerator : MonoBehaviour
{
	#region Attributes
	[SerializeField] private CityPlacementData _cityPlacementData;
	[SerializeField] private CityPlaceable[] _possibleCityPlaceables;

	private TerrainGenerator _terrainGenerator; // Needed to determine if the terrain Mesh is ready for placement and to get TerrainChunk information
	private GroundPlacementController _groundPlacementController; // Needed to determine if the position is occupied or not
	private List<CityPlaceable> _citiesToPlace; // A Queue of cities to be placed
	#endregion

	#region Getter & Setter

	public List<CityPlaceable> CitiesToPlace {
		get {
			return _citiesToPlace;
		}

		set {
			_citiesToPlace = value;
		}
	}

	public GroundPlacementController GroundPlacementController {
		get {
			return _groundPlacementController;
		}

		set {
			_groundPlacementController = value;
		}
	}

	public TerrainGenerator TerrainGenerator {
		get {
			return _terrainGenerator;
		}

		set {
			_terrainGenerator = value;
		}
	}

	public CityPlacementData CityPlacementData {
		get {
			return _cityPlacementData;
		}

		set {
			_cityPlacementData = value;
		}
	}
	#endregion

	#region Default methods
	void Start()
	{
		TerrainGenerator = FindObjectOfType<TerrainGenerator>();
		GroundPlacementController = GetComponent<GroundPlacementController>();
		CitiesToPlace = new List<CityPlaceable>();
		foreach (Vector2 placeVec2 in CityPlacementData.CityVec2)
			CitiesToPlace.Add(_possibleCityPlaceables[0]);
	}

	void Update()
	{
		if (CitiesToPlace.Count > 0 && TerrainGenerator.IsReady())
		{
			StartCoroutine(PlaceCities(CitiesToPlace));
		}
	}
	#endregion

	#region CityPlaceable Placement

	/// <summary>
	/// Places cities in the world
	/// </summary>
	/// <param name="cities"></param>
	/// <returns></returns>
	IEnumerator PlaceCities(List<CityPlaceable> cities)
	{
		for (int i = 0; i < cities.Count; i++)
		{
			CityPlaceable cityPlaceable = CitiesToPlace[i];
			CitiesToPlace.Remove(cityPlaceable);
			Move(cityPlaceable);
			FinishPlacement(cityPlaceable);
		}
		yield return null;
	}

	private void Move(CityPlaceable cityPlaceable)
	{
		float chunkSize = TerrainGenerator.GetSlotsPerChunk() / 2;

		int tilePerChunk = (int)TerrainGenerator.GetSlotsPerChunk();
		int x = (int)(((cityPlaceable.transform.position.x % tilePerChunk) + tilePerChunk) % tilePerChunk); // Calculate Modulo between 0 and tilePerChunk
		int z = (int)(((cityPlaceable.transform.position.z % tilePerChunk) + tilePerChunk) % tilePerChunk);

		int targetXMove = 0;
		int targetYMove = 0;
		int currentXMove = 0;
		int currentYMove = 0;
		int direction = -1;

		// Moves the cityPlaceable in a growing square around the starting position until a suitable location is found
		while (!GroundPlacementController.IsFlatTerrain(cityPlaceable) || !GroundPlacementController.BuildingManager.IsPlaceable(cityPlaceable))
		{
			// Move one step at a time
			if (currentXMove > 0)
			{
				cityPlaceable.Translate(direction, 0, 0);
				x += direction;
				currentXMove--;
				continue;
			}
			if (currentYMove > 0)
			{
				cityPlaceable.Translate(0, 0, direction);
				z += direction;
				currentYMove--;
				continue;
			}

			// Control step Amount
			if (targetXMove == targetYMove)
			{
				direction = -direction;
				targetXMove++;
				currentXMove = targetXMove;
			}
			else
			{
				targetYMove = targetXMove;
				currentYMove = targetYMove;
			}

			if (x >= chunkSize || z >= chunkSize || x < -chunkSize || z < -chunkSize) // end of Y
			{
				Debug.Log("CityPlaceable could not be placed at Terrainchunk: " + " X: " + x + " Y: " + z + ";; ChunkSize: " + chunkSize);
				Destroy(cityPlaceable.gameObject);
				return; // CityPlaceable can not be placed in this chunk
			}
		}
	}

	/// <summary>
	/// Searches for a place on the map to place a cityPlaceable. Moves the cityPlaceable if the current place is not suitable.
	/// </summary>
	/// <param name="cityPlaceable"></param>
	/// <param name="chunkCoordinate"></param>
	/// <returns></returns>
	private void FinishPlacement(CityPlaceable cityPlaceable)
	{
		Vector2 chunkVec = TerrainGenerator.GetTerrainChunkPosition(cityPlaceable.transform.position.x, cityPlaceable.transform.position.z);
		TerrainChunk terrainChunk = TerrainGenerator.GetTerrainChunk(chunkVec);
		cityPlaceable.gameObject.transform.parent = terrainChunk.meshObject.transform;
		GroundPlacementController.BuildingManager.AddMapPlaceable(cityPlaceable);
	}
	#endregion

	#region CityPlaceable Generation
	/// <summary>
	/// Generates a random cityPlaceable by layout provided by GetCityLayout()
	/// </summary>
	/// <param name="centerPoint"></param>
	/// <returns></returns>
	//public CityPlaceable GenerateCity(Vector2 centerPoint)
	//{
	//	int citySize = 5;
	//	int[,] generatedCity = GetCityLayout(citySize);
	//	Vector2 offsetVec = new Vector2(0.5f, 0.5f);

	//	GameObject cityGameObject = new GameObject("CityPlaceable");
	//	cityGameObject.transform.position = new Vector3(centerPoint.x + 0.5f, 0f, centerPoint.y + 0.5f);
	//	CityPlaceable cityPlaceable = cityGameObject.AddComponent<CityPlaceable>();

	//	Vector2Int centerOffset =  new Vector2Int(generatedCity.GetLength(0) / 2, generatedCity.GetLength(1) / 2);
	//	cityPlaceable.CenterPosition = centerPoint - centerOffset;
	//	for (int y = 0; y < generatedCity.GetLength(0); y++)
	//	{
	//		for (int x = 0; x < generatedCity.GetLength(1); x++)
	//		{
	//			Vector3 positionOffset = new Vector3(cityPlaceable.CenterPosition.x + x + offsetVec.x, 0f, cityPlaceable.CenterPosition.y + y + offsetVec.y);
	//			GameObject instanceObject = null;
	//			switch (generatedCity[x, y])
	//			{
	//				case StreetCoord:
	//					instanceObject = Instantiate(CityPlacementData.StreetPrefab, positionOffset, Quaternion.Euler(new Vector3(0, 0, 0)));
	//					instanceObject.transform.name = "Street";
	//					break;
	//				case MajorCoord:
	//					instanceObject = Instantiate(CityPlacementData.MainBuildingPrefab, positionOffset, Quaternion.Euler(new Vector3(0, 0, 0)));
	//					instanceObject.transform.name = "MainBuilding";
	//					break;
	//				case CitizenCoord:
	//					instanceObject = Instantiate(CityPlacementData.CitizenBuidlingPrefab, positionOffset, Quaternion.Euler(new Vector3(0, 0, 0)));
	//					instanceObject.transform.name = "CitizenBuilding";
	//					break;
	//				case BusinessCoord:
	//					instanceObject = Instantiate(CityPlacementData.BusinessBuildingPrefab, positionOffset, Quaternion.Euler(new Vector3(0, 0, 0)));
	//					instanceObject.transform.name = "BusinessBuilding";
	//					break;
	//			}
	//			if (instanceObject)
	//			{
	//				instanceObject.transform.parent = cityPlaceable.transform;
	//				cityPlaceable.ChildMapPlaceables.Add(instanceObject.GetComponent<SimpleMapPlaceable>());
	//				CityBuilding cityBuilding = instanceObject.GetComponent<CityBuilding>();
	//				if (cityBuilding)
	//					cityBuilding.CityPlaceable = cityPlaceable;
	//				//cityPlaceable.UsedCoordinates.Add(new Vector3(x - centerOffset.x, 0, y - centerOffset.y));
	//			}
	//		}
	//	}
	//	cityPlaceable.Translate(0,0.5f + TerrainGenerator.TerrainPlaceableHeight,0);
	//	cityPlaceable.Size = citySize;
	//	return cityPlaceable;
	//}

	/// <summary>
	/// Generates predefined CityPlaceable Layouts
	/// </summary>
	/// <param name="size"></param>
	/// <returns>Random and Rotated predefined cityPlaceable layout</returns>
	private int[,] GetCityLayout(int size)
	{
		int random = UnityEngine.Random.Range(0, 3);
		int[,] city = null;

		// Use ints for CityPlaceable Buildings from CityGenerator. e.g. CityGenerator.EMPTY_COORD
		switch (size)
		{
			case 5:
				switch (random)
				{
					case 0:
						city = new int[,] { { -1, 0, 0, 0, 0 }, { -1, 0, 2, 2, 0 }, { 0, 0, 1, 3, 0 }, { -1, 0, 2, 2, 0 }, { -1, 0, 0, 0, 0 } };
						break;
					case 1:
						city = new int[,] { { -1, 2, 0, -1, -1 }, { 2, 3, 0, -1, -1 }, { 0, 0, 0, 1, -1 }, { 2, 2, 0, 0, -1 }, { -1, 3, 0, -1, -1 } };
						break;
					case 2:
						city = new int[,] { { -1, 2, 0, 3, -1 }, { 3, 0, 0, 0, 0 }, { 0, 0, 1, 2, -1 }, { 2, 0, 2, -1, -1 }, { -1, 0, 2, -1, -1 } };
						break;
				}
				break;
		}
		int rotationRandom = UnityEngine.Random.Range(0, 4); // Rotate this cityPlaceable Layout randomly between 0 and 270 degrees clockwise
		for (int i = 0; i < rotationRandom; i++)
			city = RotateMatrixCounterClockwise(city);
		return city;
	}

	/// <summary>
	/// Rotates a matrix clockwise
	/// </summary>
	/// <param name="oldMatrix"></param>
	/// <returns></returns>
	private int[,] RotateMatrixCounterClockwise(int[,] oldMatrix)
	{
		int[,] newMatrix = new int[oldMatrix.GetLength(1), oldMatrix.GetLength(0)];
		int newColumn, newRow = 0;
		for (int oldColumn = oldMatrix.GetLength(1) - 1; oldColumn >= 0; oldColumn--)
		{
			newColumn = 0;
			for (int oldRow = 0; oldRow < oldMatrix.GetLength(0); oldRow++)
			{
				newMatrix[newRow, newColumn] = oldMatrix[oldRow, oldColumn];
				newColumn++;
			}
			newRow++;
		}
		return newMatrix;
	}

	#endregion
}
