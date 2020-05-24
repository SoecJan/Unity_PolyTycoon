using UnityEngine;

public class UiMouseEnterOverAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private static readonly int MouseEnter = Animator.StringToHash("OnMouseEnter");
    private static readonly int MouseExit = Animator.StringToHash("OnMouseExit");

    private void Start()
    {
        if (!_animator)
            this._animator = FindObjectOfType<Animator>();
    }

    public void OnMouseEnter()
    {
        this._animator.SetTrigger(MouseEnter);
    }

    public void OnMouseExit()
    {
        this._animator.SetTrigger(MouseExit);
    }
}
