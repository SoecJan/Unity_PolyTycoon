using UnityEngine;
using UnityEngine.UI;

public class ConstructionElementView : MonoBehaviour
{
	#region Attributes

	private static ConstructionElementView _selectedElementView;
	private static PlacementView _placementView;
	[SerializeField] private BuildingData _buildingData;
	[Header("Visuals")] 
	[SerializeField] private Button _buildingSelectButton;
	[SerializeField] private Image _backgroundImage;
	[SerializeField] private Image _buildingImage;
	private float _offAlpha;
	#endregion

	#region Methods
	private void Start()
	{
		if (_placementView == null) _placementView = FindObjectOfType<PlacementView>();
		PlacementController._onObjectPlacement += OnObjectPlacement;
		_buildingSelectButton.onClick.AddListener(delegate
		{
			Color color = _backgroundImage.color;
			if (color.a > _offAlpha)
			{
				color = new Color(color.r, color.g, color.b,  _offAlpha);
				_selectedElementView = null;
			}
			else
			{
				if (_selectedElementView)
				{
					_selectedElementView._backgroundImage.color = color = new Color(color.r, color.g, color.b,  _offAlpha);
				}

				_selectedElementView = this;
				_placementView.PlaceableObjectPrefab = _buildingData;
				color = new Color(color.r, color.g, color.b,  1f);
			}
			_backgroundImage.color = color;
		});
		if (_buildingData) UpdateView(_buildingData);
	}

	private void OnDestroy()
	{
		PlacementController._onObjectPlacement -= this.OnObjectPlacement;
	}

	public BuildingData BuildingData {
		set {
			_buildingData = value;
			UpdateView(value);
		}
	}

	private void UpdateView(BuildingData value)
	{
		_buildingImage.sprite = _buildingData.ConstructionSprite;
		if (!GetComponent<TooltipText>())
		{
			gameObject.AddComponent<TooltipText>().Text = _buildingData.BuildingName + "\n" + value.BuildingPrice;
		}
	}

	void OnObjectPlacement(BuildingData buildingData)
	{
		Color color = _backgroundImage.color;
		color = new Color(color.r, color.g, color.b, _offAlpha);
		_selectedElementView = null;
		_backgroundImage.color = color;
	}
	#endregion
}