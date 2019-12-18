using System.Collections.Generic;
using UnityEngine;

public class TrailerHitch : MonoBehaviour
{
    private List<PositionRotationInformation> _positionRotationInformations;
    [SerializeField] private Transform _trailerTransform;
    [SerializeField] private int _maxMoveNumber = 60;

    void Start()
    {
        _positionRotationInformations = new List<PositionRotationInformation>();
    }

    private void Move(Vector3 position, Vector3 rotation)
    {
        while (_positionRotationInformations.Count >= _maxMoveNumber)
        {
            PositionRotationInformation information = _positionRotationInformations[0];
            transform.SetPositionAndRotation(information.Position, Quaternion.Euler(information.Rotation));
            _positionRotationInformations.RemoveAt(0);
        }
        _positionRotationInformations.Add(new PositionRotationInformation(position, rotation));
    }

    private void LateUpdate()
    {
        Move(_trailerTransform.position, _trailerTransform.eulerAngles);
    }
}

struct PositionRotationInformation
{
    public PositionRotationInformation(Vector3 position, Vector3 rotation)
    {
        Position = position;
        Rotation = rotation;
    }

    public Vector3 Position { get; }

    public Vector3 Rotation { get; private set; }
}