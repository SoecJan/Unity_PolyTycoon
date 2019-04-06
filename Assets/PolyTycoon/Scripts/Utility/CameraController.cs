using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private Transform _moveTransform;
	[SerializeField] private float _speed = 2f;
	[SerializeField] private float _minYValue = 4f;
	[SerializeField] private float _maxYValue = 30f;

	void Start()
	{
		if (!_moveTransform) _moveTransform = transform;
	}

	void Update()
	{
		HandleKeyboardInput();
	}

	void HandleKeyboardInput()
	{
		Vector3 direction = new Vector3();
		if (Input.GetKey(KeyCode.W))
		{
			direction += Vector3.forward;
		}
		if (Input.GetKey(KeyCode.S))
		{
			direction += Vector3.back;
		}
		if (Input.GetKey(KeyCode.A))
		{
			direction += Vector3.left;
		}
		if (Input.GetKey(KeyCode.D))
		{
			direction += Vector3.right;
		}
		if (Input.GetKey(KeyCode.Q))
		{
			direction += Vector3.down;
		}
		if (Input.GetKey(KeyCode.E))
		{
			direction += Vector3.up;
		}

		if (_moveTransform.position.y <= _minYValue && direction.y < 0)
		{
			_moveTransform.position = new Vector3(_moveTransform.position.x, _minYValue, _moveTransform.position.z);
			direction.y = 0;
		}
		else if (_moveTransform.position.y >= _maxYValue && direction.y > 0)
		{
			_moveTransform.position = new Vector3(_moveTransform.position.x, _maxYValue, _moveTransform.position.z);
			direction.y = 0;
		}

		if (Input.GetKey(KeyCode.LeftShift))
		{
			_moveTransform.Translate(direction * 3 * _speed * Time.deltaTime);
		}
		else
		{
			_moveTransform.Translate(direction * _speed * Time.deltaTime);
		}

	}

	Vector3 GetRotationDirection()
	{
		Vector3 direction = new Vector3();
		if (Input.GetMouseButtonDown(2))
		{
			if (Input.GetKey(KeyCode.W))
			{
				direction += Vector3.forward;
			}
			if (Input.GetKey(KeyCode.S))
			{
				direction += Vector3.back;
			}
			if (Input.GetKey(KeyCode.A))
			{
				direction += Vector3.left;
			}
			if (Input.GetKey(KeyCode.D))
			{
				direction += Vector3.right;
			}
			if (Input.GetKey(KeyCode.Q))
			{
				direction += Vector3.down;
			}
			if (Input.GetKey(KeyCode.E))
			{
				direction += Vector3.up;
			}
		}
		return direction;
	}
}