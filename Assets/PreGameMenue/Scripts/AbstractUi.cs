using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractUi : MonoBehaviour
{
	[SerializeField] private GameObject _visibleObject;

	public GameObject VisibleObject {
		get {
			return _visibleObject? _visibleObject : gameObject;
		}

		set {
			_visibleObject = value;
		}
	}

	public virtual void SetVisible(bool visible)
	{
		Animator anim = VisibleObject.GetComponent<Animator>();
		if (!anim) anim = GetComponentInChildren<Animator>();
		if (visible)
		{
			VisibleObject.SetActive(visible);
		}
		else
		{
			Reset();
			if (anim)
			{
				StartCoroutine(TransitionEndAwait(anim));
			}
			else
			{
				VisibleObject.SetActive(false);
			}
		}

		
	}

	private IEnumerator TransitionEndAwait(Animator animator)
	{
		animator.SetTrigger("Transition");
		while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
//			Debug.Log(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
			yield return null;
		}
		animator.gameObject.SetActive(false);
	}

	public virtual void OnShortCut() { }

	public virtual void Reset()
	{
	}
}
