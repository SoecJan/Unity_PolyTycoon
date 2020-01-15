using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the logic for moving an object along a Path consisting of Waypoints.
/// </summary>
public class WaypointMoverController
{
    /// <summary>
    /// Magnitude of the vehicles velocity at the current moment
    /// </summary>
    private float _currentSpeed = 2f;
    /// <summary>
    /// Maximum speed this vehicle can reach
    /// </summary>
    private float _maxSpeed = 2f;

    /// <summary>
    /// The Transform that this mover moves
    /// </summary>
    private Transform _moverTransform;

    /// <summary>
    /// Determines if the current speed should be adjusted
    /// </summary>
    private bool _hasEngine = true;

    /// <summary>
    /// The List of Waypoints containing the path information
    /// </summary>
    private List<WayPoint> _waypointList;
    /// <summary>
    /// The current index of the waypointList element
    /// </summary>
    private int _currentWaypointIndex;

    /// <summary>
    /// Coroutine for driving smoothly through a corner
    /// </summary>
    private bool _waiting = true;
    private Coroutine _cornerCoroutine;

    public WaypointMoverController()
    {}
    
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
        get => _waypointList;
        set
        {
            _waypointList = value;
            CurrentWaypointIndex = 0;
            _waiting = false;
            OnDepart?.Invoke();
        }
    }
    
    /// <summary>
    /// This callback is executed if this mover departs from a Target
    /// </summary>
    public Action OnDepart { get; set; }

    /// <summary>
    /// This callback is executed if this mover reaches the last Waypoint of the waypointList.
    /// </summary>
    public Action OnArrive { get; set; }

    /// <summary>
    /// The current index of the waypointList element
    /// </summary>
    public int CurrentWaypointIndex
    {
        get => _currentWaypointIndex;
        set => _currentWaypointIndex = value;
    }

    /// <summary>
    /// The Transform that this mover moves
    /// </summary>
    public Transform MoverTransform
    {
        get => _moverTransform;
        set => _moverTransform = value;
    }

    public bool HasEngine
    {
        get => _hasEngine;
        set => _hasEngine = value;
    }

    public WaypointMoverController(Transform moverTransform)
    {
        MoverTransform = moverTransform;
    }

    /// <summary>
    /// Function for circles.
    /// </summary>
    /// <param name="radius">The radius of the circle</param>
    /// <param name="distance">The distance driven on the circles circumference</param>
    /// <returns>The change in angle on the given circle.</returns>
    private float GetAngle(float radius, float distance)
    {
        return (distance * 180) / (Mathf.PI * radius); 
        // Get distance of given angle & radius: distance = ((Mathf.PI * radius) * angle) / 180
        // Get angle of given distance & radius: angle = (distance * 180) / (Mathf.PI * radius)
        // Get radius of given distance & angle: radius = ((distance * 180) / angle) / Mathf.PI
    }

    /// <summary>
    /// The point on a circle with 3 vectors that describe the path.
    /// </summary>
    /// <param name="startVector3">Start position of the driven corner</param>
    /// <param name="offsetVector3">The offset Vector that forms the curve</param>
    /// <param name="targetVector3">End position of the driven corner</param>
    /// <param name="progress">Progress that has been made inside the corner. Range: [0,1] </param>
    /// <returns></returns>
    private Vector3 GetPoint(Vector3 startVector3, Vector3 offsetVector3, Vector3 targetVector3, float progress)
    {
        Vector3 startLerp = Vector3.Lerp(startVector3, offsetVector3, progress);
        Vector3 endLerp = Vector3.Lerp(offsetVector3, targetVector3, progress);
        return Vector3.Lerp(startLerp, endLerp, progress);
    }

    /// <summary>
    /// Calculates the direction the vehicle has to face.
    /// </summary>
    /// <param name="startVector3">Start position of the driven corner</param>
    /// <param name="offsetVector3">The offset Vector that forms the curve</param>
    /// <param name="targetVector3">End position of the driven corner</param>
    /// <param name="progress">Progress that has been made inside the corner. Range: [0,1] </param>
    /// <returns>The first derivative of the given points</returns>
    private Vector3 GetFirstDerivative(Vector3 startVector3, Vector3 offsetVector3, Vector3 targetVector3,
        float progress)
    {
        return
            2f * (1f - progress) * (offsetVector3 - startVector3) +
            2f * progress * (targetVector3 - offsetVector3);
    }

    /// <summary>
    /// This coroutine handles the movers logic
    /// </summary>
    /// <returns></returns>
    public IEnumerator Move()
    {
        while (_waypointList == null || Waiting)
        {
            yield return null; // Dont move at all
        }
        while ((_waypointList != null && CurrentWaypointIndex != _waypointList.Count))
        {
            WayPoint currentWayPoint = _waypointList[CurrentWaypointIndex];
            switch (currentWayPoint.TraversalVectors.Length)
            {
                case 2:
                    yield return MoveStraight(currentWayPoint); // Move in a straight line
                    break;
                case 3:
                    yield return MoveCurve(currentWayPoint); // Move along a specified corner
                    break;
            }
        
            CurrentWaypointIndex = (CurrentWaypointIndex + 1);
        }
        _waiting = true;
        OnArrive?.Invoke();
    }

    private IEnumerator MoveStraight(WayPoint currentWayPoint)
    {
        if (currentWayPoint.TraversalVectors.Length != 2) Debug.LogError("Wrong corner detected");
        Vector3 firstTraversalVector = currentWayPoint.TraversalVectors[0];
        Vector3 secondTraversalVector = currentWayPoint.TraversalVectors[1];

        yield return MoveStraight(CurrentWaypointIndex == 0 ? firstTraversalVector : secondTraversalVector);
    }

    private IEnumerator MoveStraight(Vector3 targetPosition)
    {
        MoverTransform.LookAt(targetPosition);
        float acceleration = Time.deltaTime;
        // Cache mover position
        Vector3 currentPosition = MoverTransform.position;
        // Current Difference
        Vector3 difference = targetPosition - currentPosition;
        Vector3 direction = difference.normalized;
        // Next Position
        Vector3 futurePosition = currentPosition + (Time.deltaTime * _currentSpeed * direction);
        Vector3 futureDifference = targetPosition - futurePosition;

        while (_waypointList != null && difference.magnitude > futureDifference.magnitude)
        {
            if (_hasEngine)
            {
                _currentSpeed = Mathf.Min(_currentSpeed + acceleration, _maxSpeed);
            }
            difference = targetPosition - currentPosition; // Difference Current to Target
            currentPosition += (Time.deltaTime * _currentSpeed * direction); // Next Position
            futureDifference = targetPosition - currentPosition; // Difference Next To Target
            MoverTransform.position = currentPosition; // Set Mover Position
            yield return null;
        }

        MoverTransform.position = targetPosition;
    }

    /// <summary>
    /// Drive a smooth curve.
    /// </summary>
    /// <param name="currentWayPoint">The current waypoint of this mover</param>
    /// <returns>Coroutine</returns>
    private IEnumerator MoveCurve(WayPoint currentWayPoint)
    {
        if (currentWayPoint.TraversalVectors.Length != 3) Debug.LogError("Wrong corner detected");
        // Move up to the Corner
        Vector3 firstTraversalVector = currentWayPoint.TraversalVectors[0];
        yield return MoveStraight(firstTraversalVector);

        // Move through the corner
        Vector3 secondTraversalVector = currentWayPoint.TraversalVectors[1];
        Vector3 thirdTraversalVector = currentWayPoint.TraversalVectors[2];
        yield return MoveCurve(currentWayPoint, firstTraversalVector, secondTraversalVector, thirdTraversalVector);
    }

    /// <summary>
    /// Drive a smooth curve.
    /// </summary>
    /// <param name="currentWayPoint">The current waypoint of this mover</param>
    /// <param name="vecA">Start position of the driven corner</param>
    /// <param name="vecB">The offset Vector that forms the curve</param>
    /// <param name="vecC">End position of the driven corner</param>
    /// <returns>Coroutine</returns>
    private IEnumerator MoveCurve(WayPoint currentWayPoint, Vector3 vecA, Vector3 vecB, Vector3 vecC)
    {
        Vector3 currentPosition = MoverTransform.position;
        float deceleration = Time.deltaTime;
        float progress = 0f;
        while (_waypointList != null && progress < 1f)
        {
            if (_hasEngine)
            {
                _currentSpeed = Mathf.Max(_currentSpeed - deceleration, currentWayPoint.Radius * 2f);
            }
            
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

            MoverTransform.position = currentPosition;
            MoverTransform.LookAt(targetRotation);
            
            yield return null;
        }

        MoverTransform.position = vecC;
    }
}

public class WaypointMover : MonoBehaviour
{
    protected WaypointMoverController _waypointMoverController;

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

    public Action OnDepart
    {
        get => _waypointMoverController.OnDepart;
        set => _waypointMoverController.OnDepart = value;
    }

    public Action OnArrive
    {
        get => _waypointMoverController.OnArrive;
        set => _waypointMoverController.OnArrive = value;
    }

    public List<WayPoint> WaypointList
    {
        get => _waypointMoverController.WaypointList;
        set
        {
            _waypointMoverController.WaypointList = value;
            StartCoroutine(_waypointMoverController.Move());
        }
    }

    public int CurrentWaypointIndex
    {
        get => _waypointMoverController.CurrentWaypointIndex;
        set => _waypointMoverController.CurrentWaypointIndex = value;
    }

    public Transform MoverTransform
    {
        get => _waypointMoverController.MoverTransform;
        set => _waypointMoverController.MoverTransform = value;
    }
}