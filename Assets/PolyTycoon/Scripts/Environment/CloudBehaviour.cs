using UnityEngine;

public class CloudBehaviour : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
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
        if (transform.position.x > 50f)
        {
//            _particleSystem.Stop();
//            Destroy(this.gameObject, this._particleSystem.main.startLifetime.constantMax);
            transform.Translate(-_direction * 100f);
        }
        this.transform.Translate(_direction * _speed * Time.deltaTime);
    }
}
