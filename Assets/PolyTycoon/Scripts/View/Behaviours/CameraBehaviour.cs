using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CameraBehaviour : MonoBehaviour
{
    private Transform _transform; //camera transform
    private Transform _parentTransform; //parent transform
    private int _maxCameraPosition;
    
    // Movement speeds
    [SerializeField] private float _keyboardMovementSpeed = 5f; //speed with keyboard movement
    [SerializeField] private float _screenEdgeMovementSpeed = 3f; //speed with screen edge movement
    [SerializeField] private float _followingSpeed = 5f; //speed when following a target
    [SerializeField] private float _panningSpeed = 10f;
    [SerializeField] private float _rotationSpeed = 3f;
    [SerializeField] private float _mouseRotationSpeed = 20f;

    // Position
    [SerializeField] private float _maxHeight = 25f;
    [SerializeField] private float _minHeight = 5f;
    [SerializeField] private float _heightDampening = 5f;
    [SerializeField] private float _keyboardZoomingSensitivity = 2f;
    [SerializeField] private float _scrollViewZoomingSensitivity = 25f;
    
    // Following
    [SerializeField] private Transform _targetTransform; //target to follow
    
    // Inputs
    [SerializeField] private float _screenBorderInputWidth = 3f;
    [SerializeField] private string _horizontalInputAxis = "Horizontal";
    [SerializeField] private string _verticalInputAxis = "Vertical";
    [SerializeField] private KeyCode _panningKey = KeyCode.Mouse2;
    [SerializeField] private KeyCode _zoomInKey = KeyCode.E;
    [SerializeField] private KeyCode _zoomOutKey = KeyCode.Q;
    [SerializeField] private string _zoomingAxis = "Mouse ScrollWheel";
    [SerializeField] private KeyCode _mouseRotationKey = KeyCode.Mouse1;
    
    
    private Vector2 KeyboardInput => new Vector2(Input.GetAxisRaw(_horizontalInputAxis), Input.GetAxisRaw(_verticalInputAxis));

    private Vector2 MouseInput => Input.mousePosition;

    private float ScrollWheel => Input.GetAxisRaw(_zoomingAxis);

    private Vector2 MouseAxis => new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

    private int ZoomDirection
    {
        get
        {
            bool zoomIn = Input.GetKey(_zoomInKey);
            bool zoomOut = Input.GetKey(_zoomOutKey);
            if (zoomIn && zoomOut)
                return 0;
            if (!zoomIn && zoomOut)
                return 1;
            if (zoomIn && !zoomOut)
                return -1; 
            return 0;
        }
    }

    private bool IsBorderScreenInput()
    {
        bool leftRect = MouseInput.x <= _screenBorderInputWidth && MouseInput.x >= 0;
        bool rightRect = MouseInput.x >= Screen.width -_screenBorderInputWidth && MouseInput.x < Screen.width;
        bool downRect = MouseInput.y <= _screenBorderInputWidth && MouseInput.y >= 0;
        bool upRect = MouseInput.y >= Screen.height - _screenBorderInputWidth && MouseInput.y < Screen.height;
        return leftRect || rightRect || downRect || upRect;
    }
    
    private bool IsInputDetected()
    {
        bool panning = Input.GetKey(_panningKey);
        bool escape = Input.GetKey(KeyCode.Escape);
        bool keyboardMove = !Vector2.zero.Equals(KeyboardInput);
        return IsBorderScreenInput() || panning || escape || keyboardMove;
    }
    
    void Start()
    {
        _transform = transform;
        _parentTransform = _transform.parent;
        _maxCameraPosition = (FindObjectOfType<GameHandler>().GameSettings.MapSize-1) * 45 + 25;
        PauseMenueView._onActivation += delegate(bool value) { enabled = !value; };
        InputFieldSelectionUtility.OnSelectionChange += delegate(bool value) { enabled = !value; };
    }

    void Update() // Not in fixed update because of timescale
    {
        if (IsInputDetected()) _targetTransform = null;
        if (_targetTransform)
            FollowTarget(_parentTransform);
        else
            Move(_parentTransform);
        Rotation(_parentTransform);
        Zoom(_transform);
    }

    private void Zoom(Transform movingTransform)
    {
        Vector3 currentPositionVector = movingTransform.localPosition;
        Vector3 changePositionVector = ((ZoomDirection * _keyboardZoomingSensitivity) + (ScrollWheel * _scrollViewZoomingSensitivity)) * Time.unscaledDeltaTime * Vector3.forward;
        Vector3 futurePositionVector = currentPositionVector + changePositionVector;
        if (futurePositionVector.z >= -0.5f) futurePositionVector.z = -0.5f;
        if (futurePositionVector.z <= -30f) futurePositionVector.z = -30f;
        movingTransform.localPosition = futurePositionVector;
    }

    private void Rotation(Transform rotationTransform)
    {
        if (Input.GetKey(_mouseRotationKey))
        {
            Vector3 currentRotationVector = rotationTransform.localRotation.eulerAngles;
            Vector3 rotationChangeVector = new Vector3(MouseAxis.y, MouseAxis.x, 0f);
            Vector3 futureRotationVector = currentRotationVector + rotationChangeVector;
            if (futureRotationVector.x < 5f) futureRotationVector.x = 5f;
            if (futureRotationVector.x > 89f) futureRotationVector.x = 89f;
            rotationTransform.localRotation = Quaternion.Euler(futureRotationVector);
        }
    }

    private void FollowTarget(Transform followingTransform)
    {
        Vector3 targetPos = _targetTransform.position;
        followingTransform.position = Vector3.MoveTowards(followingTransform.position, targetPos, _followingSpeed);
    }

    private void Move(Transform moveTransform)
    {
        Vector3 desiredMove = default(Vector3);
        if (!KeyboardInput.Equals(Vector2.zero))
        {
            desiredMove = new Vector3(KeyboardInput.x, 0, KeyboardInput.y);
            desiredMove *= _keyboardMovementSpeed;
        }

        if (IsBorderScreenInput())
        {
            bool leftRect = MouseInput.x <= _screenBorderInputWidth;
            bool rightRect = MouseInput.x >= Screen.width -_screenBorderInputWidth;
            bool downRect = MouseInput.y <= _screenBorderInputWidth;
            bool upRect = MouseInput.y >= Screen.height - _screenBorderInputWidth;
            
            if (leftRect || rightRect || downRect || upRect)
            {
                desiredMove = new Vector3 {x = leftRect ? -1 : rightRect ? 1 : 0, z = upRect ? 1 : downRect ? -1 : 0};
                desiredMove *= _screenEdgeMovementSpeed;
            }
        }

        if (Input.GetKey(_panningKey) && MouseAxis != Vector2.zero)
        {
            desiredMove = new Vector3(-MouseAxis.x, 0, -MouseAxis.y);
            desiredMove *= _panningSpeed;
        }

        if (desiredMove == default(Vector3)) return;
        desiredMove *= Time.unscaledDeltaTime;

        desiredMove = ApplyBounds(moveTransform.position, desiredMove);
        
        
        desiredMove = Quaternion.Euler(new Vector3(0f, moveTransform.eulerAngles.y, 0f)) * desiredMove;
        desiredMove = moveTransform.InverseTransformDirection(desiredMove);

        if (!(desiredMove.magnitude < 10f)) return;
        moveTransform.Translate(desiredMove, Space.Self);
    }

    public Vector3 ApplyBounds(Vector3 currentPosition, Vector3 desiredMove)
    {
        Vector3 futurePosition = (currentPosition + desiredMove);
        if (futurePosition.x > _maxCameraPosition)
        {
            desiredMove = new Vector3(futurePosition.x - _maxCameraPosition, 0, desiredMove.z);
        } else if (futurePosition.x < -_maxCameraPosition)
        {
            desiredMove = new Vector3(-(_maxCameraPosition + futurePosition.x), 0, desiredMove.z);
        }
        if (futurePosition.z > _maxCameraPosition)
        {
            desiredMove = new Vector3(desiredMove.x, 0, futurePosition.z - _maxCameraPosition);
        } else if (futurePosition.z < -_maxCameraPosition)
        {
            desiredMove = new Vector3(desiredMove.x, 0, -(_maxCameraPosition + futurePosition.z));
        }

        return desiredMove;
    }

    public void SetTarget(Transform target)
    {
        this._targetTransform = target;
    }
}
