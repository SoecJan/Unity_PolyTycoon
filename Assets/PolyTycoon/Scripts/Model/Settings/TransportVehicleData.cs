using System;
using UnityEngine;

public enum PathType
{
    Water,
    Road,
    Rail,
    Air
};
[CreateAssetMenu(fileName = "TransportVehicleData", menuName = "PolyTycoon/TransportVehicleData", order = 1)]
public class TransportVehicleData : ScriptableObject
{
    [SerializeField] private string _vehicleName;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private GameObject _model;
    [SerializeField] private PathType _pathType = PathType.Road;
    [SerializeField] private int _maxCapacity;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0f, 0.5f, 1f, 1f);
    [Header("Deceleration factor at given ratio of: current speed / max speed. Default: 0.1")]
    [SerializeField] private AnimationCurve decelerationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f); // relative from current speed to max speed. 
    [SerializeField] private ColliderInfo _colliderInfo;
    public string VehicleName
    {
        get => _vehicleName;
        set => _vehicleName = value;
    }

    public AnimationCurve AccelerationCurve
    {
        get => accelerationCurve;
        set => accelerationCurve = value;
    }

    public AnimationCurve DecelerationCurve
    {
        get => decelerationCurve;
        set => decelerationCurve = value;
    }

    public Sprite Sprite
    {
        get => _sprite;
        set => _sprite = value;
    }

    public GameObject Model
    {
        get => _model;
        set => _model = value;
    }

    public int MaxCapacity
    {
        get => _maxCapacity;
        set => _maxCapacity = value;
    }

    public float MaxSpeed
    {
        get => _maxSpeed;
        set => _maxSpeed = value;
    }

    public PathType PathType
    {
        get => _pathType;
        set => _pathType = value;
    }

    public ColliderInfo ColliderInfo
    {
        get => _colliderInfo;
        set => _colliderInfo = value;
    }
}

[Serializable]
public struct ColliderInfo
{
    public Vector3 center;
    public Vector3 size;
}