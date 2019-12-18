using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class ConstructionChoiceView : AbstractUi
{
	#region Attributes

	private PlacementManager _placementManager;
	
	[Header("Navigation")]
	[SerializeField] private Button _exitButton;
	[SerializeField] private Button _showButton;

	[Header("Scroll View")] 
	[SerializeField] private Toggle _infrastructureButton;
	[SerializeField] private Toggle _productionButton;
	[SerializeField] private ConstructionElementView _constructionElementViewPrefab;
	[SerializeField] private ToggleGroup _constructionElementToggleGroup;
	[SerializeField] private RectTransform _scrollViewContent;
	#endregion

	#region Methods
	private void Start()
	{
		_placementManager = FindObjectOfType<PlacementManager>();
		_exitButton.onClick.AddListener(delegate { SetVisible(false); });
		_showButton.onClick.AddListener(delegate { SetVisible(!VisibleObject.activeSelf); });
		CreateElementViews(_placementManager.InfrastructurePlaceables);
		
		_infrastructureButton.onValueChanged.AddListener(delegate(bool value)
		{
			if (!value) return;
			for (int i = 0; i < _scrollViewContent.childCount; i++)
			{
				Destroy(_scrollViewContent.GetChild(i).gameObject);
			}
			CreateElementViews(_placementManager.InfrastructurePlaceables);
		});
		_productionButton.onValueChanged.AddListener(delegate(bool value)
		{
			if (!value) return;
			for (int i = 0; i < _scrollViewContent.childCount; i++)
			{
				Destroy(_scrollViewContent.GetChild(i).gameObject);
			}
			CreateElementViews(_placementManager.ProductionPlaceables);
		});
		_infrastructureButton.isOn = true;
	}

	private void CreateElementViews(MapPlaceable[] placeables)
	{
		foreach (MapPlaceable mapPlaceable in placeables)
		{
			ConstructionElementView constructionChoiceView = GameObject.Instantiate(_constructionElementViewPrefab, _scrollViewContent);
			constructionChoiceView.MapPlaceable = mapPlaceable;
			constructionChoiceView.BuildingSelectToggle.group = _constructionElementToggleGroup;
			constructionChoiceView.BuildingSelectToggle.isOn = false;
		}
	}
	
	public override void OnShortCut()
	{
		//if (_visibleUi == this || _visibleUi == null)
		//	SetVisible(!VisibleObject.activeSelf);
	}
	#endregion
}