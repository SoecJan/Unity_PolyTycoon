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
	[SerializeField] private RectTransform _scrollViewContent;
	#endregion

	#region Methods
	private void Start()
	{
		_placementManager = FindObjectOfType<PlacementManager>();
		_exitButton.onClick.AddListener(delegate { SetVisible(false); });
		_showButton.onClick.AddListener(delegate { SetVisible(!VisibleObject.activeSelf); });
		foreach (MapPlaceable mapPlaceable in _placementManager.InfrastructurePlaceables)
		{
			ConstructionElementView constructionChoiceView = GameObject.Instantiate(_constructionElementViewPrefab, _scrollViewContent);
			constructionChoiceView.MapPlaceable = mapPlaceable;
		}
		
		_infrastructureButton.onValueChanged.AddListener(delegate(bool value)
		{
			if (!value) return;
			for (int i = 0; i < _scrollViewContent.childCount; i++)
			{
				Destroy(_scrollViewContent.GetChild(i).gameObject);
			}
			foreach (MapPlaceable mapPlaceable in _placementManager.InfrastructurePlaceables)
			{
				ConstructionElementView constructionChoiceView = GameObject.Instantiate(_constructionElementViewPrefab, _scrollViewContent);
				constructionChoiceView.MapPlaceable = mapPlaceable;
			}
		});
		_productionButton.onValueChanged.AddListener(delegate(bool value)
		{
			if (!value) return;
			for (int i = 0; i < _scrollViewContent.childCount; i++)
			{
				Destroy(_scrollViewContent.GetChild(i).gameObject);
			}
			foreach (MapPlaceable mapPlaceable in _placementManager.ProductionPlaceables)
			{
				ConstructionElementView constructionChoiceView = GameObject.Instantiate(_constructionElementViewPrefab, _scrollViewContent);
				constructionChoiceView.MapPlaceable = mapPlaceable;
			}
		});
		_infrastructureButton.isOn = true;
	}

	public override void OnShortCut()
	{
		//if (_visibleUi == this || _visibleUi == null)
		//	SetVisible(!VisibleObject.activeSelf);
	}
	#endregion
}