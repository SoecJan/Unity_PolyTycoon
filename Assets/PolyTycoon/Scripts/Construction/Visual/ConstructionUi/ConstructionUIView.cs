using UnityEngine;
using UnityEngine.UI;


public class ConstructionUIView : AbstractUi
{
	#region Attributes
	[Header("Navigation")]
	[SerializeField] private Button _exitButton;
	[SerializeField] private Button _showButton;
	[Header("Scroll View")]
	[SerializeField] private GameObject _elementPrefab;
	[SerializeField] private ScrollViewHandle _scrollViewHandle;
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
		foreach (SimpleMapPlaceable mapPlaceable in buildingManager.Buildings)
		{
			GameObject instance = _scrollViewHandle.AddObject((RectTransform)_elementPrefab.transform);
			ConstructionUIElement constructionUiElement = instance.GetComponent<ConstructionUIElement>();
			constructionUiElement.MapPlaceable = mapPlaceable;
		}
	}

	public override void OnShortCut()
	{
		//if (_visibleUi == this || _visibleUi == null)
		//	SetVisible(!VisibleObject.activeSelf);
	}
	#endregion
}