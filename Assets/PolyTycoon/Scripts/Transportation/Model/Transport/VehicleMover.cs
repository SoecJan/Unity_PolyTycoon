using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class MoverController
{
    private float _currentSpeed = 2f;
    private float _maxSpeed = 2f;

    private Transform _moverTransform;

    private List<WayPoint> _wayPointList;
    private int _wayPointIndex;

    private bool _waiting = true;
    private Coroutine _cornerCoroutine;

    public MoverController()
    {
    }

    public bool IsWaiting
    {
        get => _waiting;
    }

    public float MaxSpeed => _maxSpeed;

    public List<WayPoint> WayPointList
    {
        set
        {
            _wayPointList = value;
            _wayPointIndex = 0;
            _waiting = false;
        }
    }

    public Action OnArrive { get; set; }

    public MoverController(Transform moverTransform)
    {
        _moverTransform = moverTransform;
    }

    private bool IsBetweenPoints(Vector2 pointA, Vector2 pointB, Vector2 pointChecked)
    {
        return Math.Abs(Distance(pointA, pointChecked) + Distance(pointB, pointChecked) - Distance(pointA, pointB)) <
               0.1f;
    }

    private float Distance(Vector2 pointA, Vector2 pointB)
    {
        return (pointA - pointB).sqrMagnitude;
    }

    private Vector2 Vec2(Vector3 inputVector)
    {
        return new Vector2(inputVector.x, inputVector.z);
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
        while (_wayPointList == null || IsWaiting)
        {
            yield return 0;
        }
        while (_wayPointIndex != _wayPointList.Count)
        {
            WayPoint currentWayPoint = _wayPointList[_wayPointIndex];
            switch (currentWayPoint.TraversalVectors.Length)
            {
                case 2:
                    yield return MoveStraight(currentWayPoint);
                    
                    break;
                case 3:
                    yield return MoveCurve(currentWayPoint);
                    break;
            }
        
            _wayPointIndex = (_wayPointIndex + 1);
        }
        _waiting = true;
        OnArrive();
    }

    private IEnumerator MoveStraight(WayPoint currentWayPoint)
    {
        if (currentWayPoint.TraversalVectors.Length != 2) Debug.LogError("Wrong corner detected");
        Vector3 firstTraversalVector = currentWayPoint.TraversalVectors[0];
        Vector3 secondTraversalVector = currentWayPoint.TraversalVectors[1];

        yield return MoveStraight(_wayPointIndex == 0 ? firstTraversalVector : secondTraversalVector);
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
            Debug.Log("Move Straight");
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
            Debug.Log("Move Corner" + progress + ", " + circumferenceDistanceToAngle + ", r: " + currentWayPoint.Radius + ", s:" + _currentSpeed + ", t: " + Time.deltaTime);
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

public class VehicleMover : MonoBehaviour
{
    private MoverController _moverController;

    void Awake()
    {
        _moverController = new MoverController(transform);
    }

    private void Start()
    {
        OnArrive.Invoke();
    }

    public float MaxSpeed => _moverController.MaxSpeed;

    public Action OnArrive
    {
        get => _moverController.OnArrive;
        set => _moverController.OnArrive = value;
    }

    public List<WayPoint> WayPointList
    {
        set
        {
            _moverController.WayPointList = value;
            StartCoroutine(_moverController.Move());
        }
    }
}