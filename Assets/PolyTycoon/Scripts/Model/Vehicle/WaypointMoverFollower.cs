using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMoverFollower : WaypointMover
{
    private float _targetDistance = 1f;
    private WaypointMover _parentMover;

    public float TargetDistance
    {
        get => _targetDistance;
        set => _targetDistance = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        _targetDistance = MoverTransform.lossyScale.z;
        _parentMover = GetComponent<WaypointMover>();
        this._parentMover.OnArrive += () =>
        {
            this._waypointMoverController.WaypointList = null;
            this.Waiting = true;
        };

        this._parentMover.OnDepart += () => { StartCoroutine(KeepDistance()); };
        this._waypointMoverController.HasEngine = false;
        this._waypointMoverController.CurrentSpeed = 0;
    }

    IEnumerator KeepDistance()
    { 
        this.MoverTransform.position = _parentMover.MoverTransform.position;
        Vector3 difference = _parentMover.MoverTransform.position - MoverTransform.position;
        float distance = difference.magnitude;
        while (distance < _targetDistance)
        {
            difference = _parentMover.MoverTransform.position - MoverTransform.position;
            distance = difference.magnitude;
            yield return null;
        }
        this._waypointMoverController.WaypointList = _parentMover.WaypointList;
        StartCoroutine(_waypointMoverController.Move());
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 difference = _parentMover.MoverTransform.position - MoverTransform.position;
        float distance = difference.magnitude;
        this.CurrentSpeed = distance > _targetDistance ? _parentMover.CurrentSpeed * 1.1f : distance < _targetDistance ? _parentMover.CurrentSpeed * 0.9f : _parentMover.CurrentSpeed;
    }
}