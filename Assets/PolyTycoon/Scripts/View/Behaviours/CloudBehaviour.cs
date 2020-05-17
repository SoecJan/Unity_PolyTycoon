using UnityEngine;

public class CloudBehaviour : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _resetDistance = 45f;
    [SerializeField] private Vector3 _direction = Vector3.forward;
    private ParticleSystem _particleSystem;
    
    // Start is called before the first frame update
    void Start()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.localPosition.x > _resetDistance)
        {
            transform.Translate(-_direction * (_resetDistance * 2));
        }
        this.transform.Translate(_direction * (_speed * Time.deltaTime));
    }
}
