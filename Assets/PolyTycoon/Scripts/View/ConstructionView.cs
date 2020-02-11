using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ConstructionView : AbstractUi
{
	#region Attributes

	private PlacementController _placementController;
	private ConstructionElementView _constructionElementViewPrefab;
	
	[Header("Navigation")]
	[SerializeField] private Button _exitButton;
	[SerializeField] private Button _showButton;

	[Header("Scroll View")] 
	[SerializeField] private Toggle _infrastructureButton;
	[SerializeField] private Toggle _productionButton;
	
	[SerializeField] private ToggleGroup _constructionElementToggleGroup;
	[SerializeField] private RectTransform _scrollViewContent;
	#endregion

	#region Methods
	private void Start()
	{
		_placementController = FindObjectOfType<GameHandler>().PlacementController;
		_exitButton.onClick.AddListener(delegate { SetVisible(false); });
		_showButton.onClick.AddListener(delegate { SetVisible(!VisibleObject.activeSelf); });
		
		_constructionElementViewPrefab = Resources.Load<ConstructionElementView>(PathUtil.Get("ConstructionElementView"));
		BuildingData[] _productionBuildingData = Resources.LoadAll<BuildingData>("Data/BuildingData/Production");
		BuildingData[] _infrastructureBuildingData = Resources.LoadAll<BuildingData>("Data/BuildingData/Infrastructure");
		CreateElementViews(_infrastructureBuildingData);
		
		_infrastructureButton.onValueChanged.AddListener(delegate(bool value)
		{
			if (!value) return;
			ClearElementViews();
			CreateElementViews(_infrastructureBuildingData);
		});
		_productionButton.onValueChanged.AddListener(delegate(bool value)
		{
			if (!value) return;
			ClearElementViews();
			CreateElementViews(_productionBuildingData);
		});
		_infrastructureButton.isOn = true;
	}

	private void CreateElementViews(BuildingData[] buildingDataArray)
	{
		foreach (BuildingData buildingData in buildingDataArray)
		{
			ConstructionElementView constructionChoiceView = GameObject.Instantiate(_constructionElementViewPrefab, _scrollViewContent);
			constructionChoiceView.BuildingData = buildingData;
			constructionChoiceView.BuildingSelectToggle.group = _constructionElementToggleGroup;
			constructionChoiceView.BuildingSelectToggle.isOn = false;
		}
	}

	private void ClearElementViews()
	{
		for (int i = 0; i < _scrollViewContent.childCount; i++)
		{
			Destroy(_scrollViewContent.GetChild(i).gameObject);
		}
	}
	
	public override void OnShortCut()
	{
		//if (_visibleUi == this || _visibleUi == null)
		//	SetVisible(!VisibleObject.activeSelf);
	}
	#endregion
}