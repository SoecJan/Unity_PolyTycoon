using System;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionElementView : MonoBehaviour
{
	#region Attributes
	private static PlacementManager placementManager;
	private MapPlaceable _mapPlaceable;
	[Header("Visuals")]
	[SerializeField] private Toggle _buildingSelectToggle;
	[SerializeField] private Image _buildingImage;
	[SerializeField] private TextMeshProUGUI _buildingNameText;
	#endregion

	#region Methods
	private void Start()
	{
		if (!placementManager) placementManager = FindObjectOfType<PlacementManager>();
		PlacementManager._onObjectPlacement += this.OnObjectPlacement;
		_buildingSelectToggle.onValueChanged.AddListener(OnClick);
	}

	private void OnDestroy()
	{
		PlacementManager._onObjectPlacement -= this.OnObjectPlacement;
	}

	public Toggle BuildingSelectToggle => _buildingSelectToggle;

	public MapPlaceable MapPlaceable {
		set {
			_mapPlaceable = value;
			_buildingImage.sprite = _mapPlaceable.ConstructionUiSprite;
			_buildingNameText.text = _mapPlaceable.BuildingName;
			if (!GetComponent<TooltipText>())
			{
				gameObject.AddComponent<TooltipText>().Text = _mapPlaceable.BuildingName + "\n" + value.BuildingPrice;
			}
		}
	}

	void OnObjectPlacement(MapPlaceable mapPlaceable)
	{
		if (!mapPlaceable)
		{
			_buildingSelectToggle.isOn = false;
		}
		else if (mapPlaceable.ConstructionUiSprite.Equals(_buildingImage.sprite))
		{
			_buildingSelectToggle.isOn = false;
		} 
	}

	private void OnClick(bool value)
	{
		if (!value) return;
		placementManager.PlaceableObjectPrefab = _mapPlaceable;
	}
	#endregion
}