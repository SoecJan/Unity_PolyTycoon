using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractUi : MonoBehaviour
{
	[SerializeField] private GameObject _visibleObject;

	public GameObject VisibleObject {
		get {
			return _visibleObject;
		}

		set {
			_visibleObject = value;
		}
	}

	public virtual void SetVisible(bool visible)
	{
		OnVisibilityChange(visible);
		Animator anim = GetComponentInChildren<Animator>();
		if (visible)
		{
			VisibleObject.SetActive(visible);
		}
		else
		{
			
			if (anim)
			{
				StartCoroutine(TransitionEndAwait(anim));
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}

	private IEnumerator TransitionEndAwait(Animator animator)
	{
		animator.SetTrigger("Transition");
		while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
			Debug.Log(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
			yield return null;
		}
		Debug.Log("Inactive");
		animator.gameObject.SetActive(false);
	}

	public abstract void Reset();

	protected abstract void OnVisibilityChange(bool visible);
}
