using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class holds necessary information on a given BuildingData
/// </summary>
[CreateAssetMenu(fileName = "BuildingData", menuName = "PolyTycoon/BuildingData", order = 1)]
public class BuildingData : ScriptableObject
{
	#region Attributes
	[Header("General")]
	[Tooltip("The construction sprite displayed to the player")]
	[SerializeField] private Sprite _constructionSprite;
	[Tooltip("The building name displayed to the player")]
	[SerializeField] private string _buildingName;
	[Tooltip("The description displayed to the player")]
	[SerializeField] private string _description;

	[SerializeField] private int _buildingPrice;

	[SerializeField] private GameObject _prefab;
	#endregion

	#region Getter & Setter

	public Sprite ConstructionSprite
	{
		get => _constructionSprite;
		set => _constructionSprite = value;
	}

	public string BuildingName
	{
		get => _buildingName;
		set => _buildingName = value;
	}

	public string Description
	{
		get => _description;
		set => _description = value;
	}

	public GameObject Prefab
	{
		get => _prefab;
		set => _prefab = value;
	}

	public int BuildingPrice
	{
		get => _buildingPrice;
		set => _buildingPrice = value;
	}

	#endregion

	public override string ToString()
	{
		return BuildingName + ", Description: " + ": " + Description;
	}

	public override bool Equals(object obj)
	{
		var data = obj as BuildingData;
		return data != null &&
			   base.Equals(obj) && BuildingName.Equals(data.BuildingName);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}