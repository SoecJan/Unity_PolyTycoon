using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class TooltipHandle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

	#region Attributes

	[SerializeField] protected Vector3 _offset;
	[SerializeField] private RectTransform _tipTransform;
	[SerializeField] private float _timeUntilDisplay = 1f;
	private Coroutine _coroutine;

	public RectTransform TipTransform
	{
		get => (RectTransform) (_tipTransform == null ? transform : _tipTransform);
		set => _tipTransform = value;
	}

	public float TimeUntilDisplay
	{
		get => _timeUntilDisplay;
		set => _timeUntilDisplay = value;
	}

	#endregion

	void Start()
	{}

	#region Methods
	void LateUpdate()
	{
		if (TipTransform)
		{
			TipTransform.position = Input.mousePosition + _offset;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (_coroutine != null) StopCoroutine(_coroutine);
		TipTransform.gameObject.SetActive(false);
	}
	#endregion

	public void OnPointerEnter(PointerEventData eventData)
	{
		_coroutine = StartCoroutine(DisplayToolTip());
	}

	private IEnumerator DisplayToolTip()
	{
		yield return new WaitForSeconds(TimeUntilDisplay);
		TipTransform.gameObject.SetActive(true);
		_coroutine = null;
	}
}
