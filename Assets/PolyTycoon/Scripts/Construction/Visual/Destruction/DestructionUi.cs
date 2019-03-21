using UnityEngine;
using UnityEngine.UI;

public class DestructionUi : AbstractUi
{
	private static DestructionController _destructionController;

	[SerializeField] private Button _showButton;
	[SerializeField] private Image _destructionCursorImage;
	[SerializeField] private Vector3 _cursorImageOffset;

	public override void OnShortCut()
	{
		base.OnShortCut();
		//if (_visibleUi == this || _visibleUi == null)
		//	SetVisible(!VisibleObject.activeSelf);
		DestructionController.DestructionActive = !DestructionController.DestructionActive;
		SetVisible(DestructionController.DestructionActive);
	}

	// Use this for initialization
	void Start ()
	{
		if (!_destructionController) _destructionController = FindObjectOfType<DestructionController>();
		_showButton.onClick.AddListener(delegate { SetVisible(!VisibleObject.activeSelf);
			DestructionController.DestructionActive = VisibleObject.activeSelf;
		});
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(1) && DestructionController.DestructionActive)
		{
			SetVisible(false);
			DestructionController.DestructionActive = false;
		}
	}

	// Update is called once per frame
	void LateUpdate () {
		if (_destructionCursorImage)
		{
			_destructionCursorImage.transform.position = Input.mousePosition + _cursorImageOffset;
		}
	}
}
