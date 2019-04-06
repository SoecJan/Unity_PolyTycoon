using UnityEngine;
using UnityEngine.UI;

public class ConstructionUIElement : MonoBehaviour
{
	#region Attributes
	private static GroundPlacementController _groundPlacementController;
	private SimpleMapPlaceable _mapPlaceable;
	[Header("Visuals")]
	[SerializeField] private Button _buildingSelectButton;
	[SerializeField] private Image _buildingImage;
	[SerializeField] private Text _buildingNameText;
	#endregion

	#region Methods
	private void Start()
	{
		if (!_groundPlacementController) _groundPlacementController = FindObjectOfType<GroundPlacementController>();
		_buildingSelectButton.onClick.AddListener(OnClick);
	}

	public SimpleMapPlaceable MapPlaceable {
		get {
			return _mapPlaceable;
		}

		set {
			_mapPlaceable = value;
			_buildingImage.sprite = _mapPlaceable.ConstructionUiSprite;
			_buildingNameText.text = _mapPlaceable.BuildingName;
		}
	}

	private void OnClick()
	{
		_groundPlacementController.PlaceableObjectPrefab = _mapPlaceable;
	}
	#endregion
}