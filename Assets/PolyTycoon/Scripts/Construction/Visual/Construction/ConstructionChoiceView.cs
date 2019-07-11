using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class ConstructionChoiceView : AbstractUi
{
	#region Attributes
	[Header("Navigation")]
	[SerializeField] private Button _exitButton;
	[SerializeField] private Button _showButton;
	[Header("Scroll View")]
	[SerializeField] private ConstructionElementView _constructionElementViewPrefab;
	[SerializeField] private RectTransform _scrollViewContent;
	#endregion

	#region Methods
	private void Start()
	{
		_exitButton.onClick.AddListener(delegate { SetVisible(false); });
		_showButton.onClick.AddListener(delegate { SetVisible(!VisibleObject.activeSelf); });
		FillView();
	}

	private void FillView()
	{
		GroundPlacementController buildingManager = GameObject.FindObjectOfType<GroundPlacementController>();
		foreach (MapPlaceable mapPlaceable in buildingManager.Buildings)
		{
			ConstructionElementView constructionChoiceView = GameObject.Instantiate(_constructionElementViewPrefab, _scrollViewContent);
			constructionChoiceView.MapPlaceable = mapPlaceable;
		}
	}

	public override void OnShortCut()
	{
		//if (_visibleUi == this || _visibleUi == null)
		//	SetVisible(!VisibleObject.activeSelf);
	}
	#endregion
}