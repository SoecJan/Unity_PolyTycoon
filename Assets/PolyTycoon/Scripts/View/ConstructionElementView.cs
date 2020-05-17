using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionElementView : MonoBehaviour
{
	#region Attributes
	private static PlacementView _placementView;
	private BuildingData _buildingData;
	[Header("Visuals")]
	[SerializeField] private Toggle _buildingSelectToggle;
	[SerializeField] private Image _buildingImage;
	[SerializeField] private TextMeshProUGUI _buildingNameText;
	#endregion

	#region Methods
	private void Start()
	{
		if (_placementView == null) _placementView = FindObjectOfType<PlacementView>();
		PlacementController._onObjectPlacement += this.OnObjectPlacement;
		_buildingSelectToggle.onValueChanged.AddListener(OnClick);
	}

	private void OnDestroy()
	{
		PlacementController._onObjectPlacement -= this.OnObjectPlacement;
	}

	public Toggle BuildingSelectToggle => _buildingSelectToggle;

	public BuildingData BuildingData {
		set {
			_buildingData = value;
			_buildingImage.sprite = _buildingData.ConstructionSprite;
			_buildingNameText.text = _buildingData.BuildingName;
			if (!GetComponent<TooltipText>())
			{
				gameObject.AddComponent<TooltipText>().Text = _buildingData.BuildingName + "\n" + value.BuildingPrice;
			}
		}
	}

	void OnObjectPlacement(BuildingData buildingData)
	{
		if (!buildingData)
		{
			_buildingSelectToggle.isOn = false;
		}
		else if (buildingData.BuildingName.Equals(_buildingNameText.text))
		{
			_buildingSelectToggle.isOn = false;
		} 
	}

	private void OnClick(bool value)
	{
		if (!value) return;
		_placementView.PlaceableObjectPrefab = _buildingData;
	}
	#endregion
}