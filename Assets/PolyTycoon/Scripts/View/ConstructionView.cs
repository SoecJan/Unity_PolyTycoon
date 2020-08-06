using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionView : AbstractUi
{
	#region Attributes
	private ConstructionElementView _constructionElementViewPrefab;
	
	[Header("Navigation")]
	[SerializeField] private Button _exitButton;
	[SerializeField] private Button _showButton;
	#endregion

	#region Methods
	private void Start()
	{
		GameHandler gameHandler = FindObjectOfType<GameHandler>();
		_exitButton.onClick.AddListener(delegate { SetVisible(false); });
		_showButton.onClick.AddListener(delegate { SetVisible(!VisibleObject.activeSelf); });
		
		gameHandler.ProgressionManager.onBuildingUnlock += delegate(BuildingData[] buildingDataArray)
		{
			Debug.Log("BuildingUnlock");
		};
	}

	public override void OnShortCut()
	{
		//if (_visibleUi == this || _visibleUi == null)
		//	SetVisible(!VisibleObject.activeSelf);
	}
	#endregion
}