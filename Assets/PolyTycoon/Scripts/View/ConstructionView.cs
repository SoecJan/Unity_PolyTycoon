using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ConstructionView : AbstractUi
{
	#region Attributes

	private IPlacementController _placementController;
	private ConstructionElementView _constructionElementViewPrefab;
	private Dictionary<BuildingData.BuildingCategory, List<BuildingData>> _buildingDictionary;
	
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
		_buildingDictionary = new Dictionary<BuildingData.BuildingCategory, List<BuildingData>>();
		GameHandler gameHandler = FindObjectOfType<GameHandler>();
		_placementController = gameHandler.PlacementController;
		_exitButton.onClick.AddListener(delegate { SetVisible(false); });
		_showButton.onClick.AddListener(delegate { SetVisible(!VisibleObject.activeSelf); });
		
		gameHandler.ProgressionManager.onBuildingUnlock += delegate(BuildingData[] buildingDataArray)
		{
			Debug.Log("BuildingUnlock");
			foreach (BuildingData buildingData in buildingDataArray)
			{
				if (!_buildingDictionary.ContainsKey(buildingData.Category))
				{
					_buildingDictionary.Add(buildingData.Category, new List<BuildingData>());
				}
				_buildingDictionary[buildingData.Category].Add(buildingData);
			}
			CreateElementViews(BuildingData.BuildingCategory.Infrastructure);
		};
		
		_constructionElementViewPrefab = Resources.Load<ConstructionElementView>(PathUtil.Get("ConstructionElementView"));

		_infrastructureButton.onValueChanged.AddListener(delegate(bool value)
		{
			if (!value) return;
			ClearElementViews();
			CreateElementViews(BuildingData.BuildingCategory.Infrastructure);
		});
		_productionButton.onValueChanged.AddListener(delegate(bool value)
		{
			if (!value) return;
			ClearElementViews();
			CreateElementViews(BuildingData.BuildingCategory.Production);
		});
		_infrastructureButton.isOn = true;
	}

	private void CreateElementViews(BuildingData.BuildingCategory buildingCategory)
	{
		List<BuildingData> buildingDataArray = this._buildingDictionary[buildingCategory];
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