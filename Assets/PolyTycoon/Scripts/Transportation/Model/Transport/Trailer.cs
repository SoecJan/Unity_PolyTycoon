using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trailer : MonoBehaviour
{
    [SerializeField] private Transform _moverAnchor;
    [SerializeField] private float moverDistance = 1f;
    [SerializeField] private Transform _trailerAnchor;
    private Vector3 _oldAnchorPosition;

    public Transform MoverAnchor
    {
        get { return _moverAnchor; }
        set { _moverAnchor = value; }
    }

    public Transform TrailerAnchor
    {
        get { return _trailerAnchor; }
    }

    // Start is called before the first frame update
    void Start()
    {
//        _oldAnchorPosition = MoverAnchor.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!MoverAnchor) return;
        if (!_oldAnchorPosition.Equals(Vector3.zero))
        {
            Vector3 drivenDistance = MoverAnchor.position - _oldAnchorPosition;
            float distance = drivenDistance.magnitude;
            Vector3 direction = (MoverAnchor.position - transform.position).normalized;
        
            transform.position = MoverAnchor.position - (direction * moverDistance);
        
            transform.LookAt(MoverAnchor.position);
        }
        _oldAnchorPosition = MoverAnchor.position;
    }
}
