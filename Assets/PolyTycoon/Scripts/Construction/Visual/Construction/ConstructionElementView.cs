using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionElementView : MonoBehaviour
{
	#region Attributes
	private static PlacementManager placementManager;
	private MapPlaceable _mapPlaceable;
	[Header("Visuals")]
	[SerializeField] private Button _buildingSelectButton;
	[SerializeField] private Image _buildingImage;
	[SerializeField] private TextMeshProUGUI _buildingNameText;
	#endregion

	#region Methods
	private void Start()
	{
		if (!placementManager) placementManager = FindObjectOfType<PlacementManager>();
		_buildingSelectButton.onClick.AddListener(OnClick);
	}

	public MapPlaceable MapPlaceable {
		get => _mapPlaceable;

		set {
			_mapPlaceable = value;
			_buildingImage.sprite = _mapPlaceable.ConstructionUiSprite;
			_buildingNameText.text = _mapPlaceable.BuildingName;
		}
	}

	private void OnClick()
	{
		placementManager.PlaceableObjectPrefab = _mapPlaceable;
	}
	#endregion
}