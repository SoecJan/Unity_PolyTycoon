using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class TooltipHandle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

	#region Attributes

	[SerializeField] private Vector3 _offset;
	[SerializeField] private RectTransform _tipTransform;
	[SerializeField] private float _timeUntilDisplay = 1f;
	private static Tooltip _tooltip;
	private Coroutine _coroutine;
	#endregion

	void Start()
	{
		if (!_tooltip) _tooltip = FindObjectOfType<Tooltip>();
		_tipTransform.SetParent(_tooltip.transform);
		//_tipTransform.position = Vector3.zero;
	}

	#region Methods
	void LateUpdate()
	{
		_tipTransform.position = Input.mousePosition + _offset;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (_coroutine != null) StopCoroutine(_coroutine);
		_tipTransform.gameObject.SetActive(false);
	}
	#endregion

	public void OnPointerEnter(PointerEventData eventData)
	{
		_coroutine = StartCoroutine(DisplayToolTip());
	}

	private IEnumerator DisplayToolTip()
	{
		yield return new WaitForSeconds(_timeUntilDisplay);
		_tipTransform.gameObject.SetActive(true);
		_coroutine = null;
	}
}
