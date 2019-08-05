using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMoverController
{
    private float _currentSpeed = 2f;
    private float _maxSpeed = 2f;

    private Transform _moverTransform;

    private List<WayPoint> _waypointList;
    private int _currentWaypointIndex;

    private bool _waiting = true;
    private Coroutine _cornerCoroutine;

    public WaypointMoverController()
    {
    }
    
    public float CurrentSpeed
    {
        get => _currentSpeed;
        set => _currentSpeed = value;
    }

    public float MaxSpeed
    {
        get => _maxSpeed;
        set => _maxSpeed = value;
    }

    public bool Waiting
    {
        get => _waiting;
        set => _waiting = value;
    }

    public List<WayPoint> WaypointList
    {
        set
        {
            _waypointList = value;
            _currentWaypointIndex = 0;
            _waiting = false;
        }
    }

    public Action OnArrive { get; set; }

    public WaypointMoverController(Transform moverTransform)
    {
        _moverTransform = moverTransform;
    }

    private float GetAngle(float radius, float distance)
    {
        return (distance * 180) / (Mathf.PI * radius);
    }

    private Vector3 GetPoint(Vector3 startVector3, Vector3 offsetVector3, Vector3 targetVector3, float progress)
    {
        return Vector3.Lerp(Vector3.Lerp(startVector3, offsetVector3, progress),
            Vector3.Lerp(offsetVector3, targetVector3, progress), progress);
    }

    private Vector3 GetFirstDerivative(Vector3 startVector3, Vector3 offsetVector3, Vector3 targetVector3,
        float progress)
    {
        return
            2f * (1f - progress) * (offsetVector3 - startVector3) +
            2f * progress * (targetVector3 - offsetVector3);
    }

    public IEnumerator Move()
    {
        while (_waypointList == null || Waiting)
        {
            yield return 0;
        }
        while (_currentWaypointIndex != _waypointList.Count)
        {
            WayPoint currentWayPoint = _waypointList[_currentWaypointIndex];
            switch (currentWayPoint.TraversalVectors.Length)
            {
                case 2:
                    yield return MoveStraight(currentWayPoint);
                    break;
                case 3:
                    yield return MoveCurve(currentWayPoint);
                    break;
            }
        
            _currentWaypointIndex = (_currentWaypointIndex + 1);
        }
        _waiting = true;
        OnArrive();
    }

    private IEnumerator MoveStraight(WayPoint currentWayPoint)
    {
        if (currentWayPoint.TraversalVectors.Length != 2) Debug.LogError("Wrong corner detected");
        Vector3 firstTraversalVector = currentWayPoint.TraversalVectors[0];
        Vector3 secondTraversalVector = currentWayPoint.TraversalVectors[1];

        yield return MoveStraight(_currentWaypointIndex == 0 ? firstTraversalVector : secondTraversalVector);
    }

    private IEnumerator MoveStraight(Vector3 targetPosition)
    {
        _moverTransform.LookAt(targetPosition);
        // Cache mover position
        Vector3 currentPosition = _moverTransform.position;
        // Current Difference
        Vector3 difference = targetPosition - currentPosition;
        Vector3 direction = difference.normalized;
        // Next Position
        Vector3 futurePosition = currentPosition + (direction * _currentSpeed * Time.deltaTime);
        Vector3 futureDifference = targetPosition - futurePosition;

        while (difference.magnitude > futureDifference.magnitude)
        {
            difference = targetPosition - currentPosition; // Difference Current to Target
            currentPosition = currentPosition + (direction * _currentSpeed * Time.deltaTime); // Next Position
            futureDifference = targetPosition - currentPosition; // Difference Next To Target
            _moverTransform.position = currentPosition; // Set Mover Position
            yield return null;
        }

        _moverTransform.position = targetPosition;
    }

    private IEnumerator MoveCurve(WayPoint currentWayPoint)
    {
        if (currentWayPoint.TraversalVectors.Length != 3) Debug.LogError("Wrong corner detected");
        // Drive smooth corner
     
        // Move up to the Corner
        Vector3 firstTraversalVector = currentWayPoint.TraversalVectors[0];
        yield return MoveStraight(firstTraversalVector);

        // Move through the corner
        Vector3 secondTraversalVector = currentWayPoint.TraversalVectors[1];
        Vector3 thirdTraversalVector = currentWayPoint.TraversalVectors[2];
        yield return MoveCurve(currentWayPoint, firstTraversalVector, secondTraversalVector, thirdTraversalVector);
    }

    private IEnumerator MoveCurve(WayPoint currentWayPoint, Vector3 vecA, Vector3 vecB, Vector3 vecC)
    {
        Vector3 currentPosition = _moverTransform.position;
        float progress = 0f;
        while (progress < 1f)
        {
            // Get Angle on a circle with given radius and distance driven
            float circumferenceDistanceToAngle = GetAngle(currentWayPoint.Radius, Time.deltaTime * _currentSpeed);
            // Add to the progress that was already made
            progress += circumferenceDistanceToAngle / 90f; // 90 Degree Turn at each corner
            // Set the vehicle position to the one on the circle
            currentPosition = GetPoint(vecA,vecB,vecC, progress);
            // Set the vehicle rotation to face driven direction
            Vector3 targetRotation = currentPosition +
                                     GetFirstDerivative(vecA,vecB,vecC,
                                         progress).normalized;

            _moverTransform.position = currentPosition;
            _moverTransform.LookAt(targetRotation);
            
            yield return null;
        }

        _moverTransform.position = vecC;
    }
}

public class WaypointMover : MonoBehaviour
{
    private WaypointMoverController _waypointMoverController;

    void Awake()
    {
        _waypointMoverController = new WaypointMoverController(transform);
    }

    private void Start()
    {
        OnArrive.Invoke();
    }

    public float CurrentSpeed
    {
        get => _waypointMoverController.CurrentSpeed;
        set => _waypointMoverController.CurrentSpeed = value;
    }

    public float MaxSpeed
    {
        get => _waypointMoverController.MaxSpeed;
        set => _waypointMoverController.MaxSpeed = value;
    }

    public bool Waiting
    {
        get => _waypointMoverController.Waiting;
        set => _waypointMoverController.Waiting = value;
    }

    public Action OnArrive
    {
        get => _waypointMoverController.OnArrive;
        set => _waypointMoverController.OnArrive = value;
    }

    protected List<WayPoint> WaypointList
    {
        set
        {
            Debug.Log("Set waypoint list and now moving");
            _waypointMoverController.WaypointList = value;
            StartCoroutine(_waypointMoverController.Move());
        }
    }
}