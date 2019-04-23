using System.Collections.Generic;
using UnityEngine;

public class TrailerHitch : MonoBehaviour
{
//    [SerializeField] private Trailer _trailer;
//    [SerializeField] private Transform _trailerAnchor;
    private List<PositionRotationInformation> _positionRotationInformations;
    [SerializeField] private Transform _trailerTransform;
    [SerializeField] private int _maxMoveNumber = 60;

    // Start is called before the first frame update
    void Start()
    {
        _positionRotationInformations = new List<PositionRotationInformation>();
//        if (_trailer)
//        {
//            Trailer trailer0 = Instantiate(_trailer, transform.position, Quaternion.identity);
//            trailer0.MoverAnchor = _trailerAnchor;
//        }
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
    private Vector3 position;
    private Vector3 rotation;

    public PositionRotationInformation(Vector3 position, Vector3 rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public Vector3 Position
    {
        get { return position; }
        set { position = value; }
    }

    public Vector3 Rotation
    {
        get { return rotation; }
        set { rotation = value; }
    }
}