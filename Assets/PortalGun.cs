using UnityEngine;
using UnityEngine.InputSystem;

public class PortalGun : MonoBehaviour
{
    [SerializeField] Transform _shootPos;
    [SerializeField] GameObject Portal_blue;
    [SerializeField] GameObject Portal_or;
    [SerializeField] int _speed;
    [SerializeField] float _cooldown = 0.5f;
    float _nextShootTime;
    [SerializeField] bool canShoot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
  

    void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (Time.time < _nextShootTime) return; // todavía en cooldown

        if (mouse.leftButton.wasPressedThisFrame)
        {
            Shoot(Portal_blue);
            _nextShootTime = Time.time + _cooldown;
        }

        if (mouse.rightButton.wasPressedThisFrame)
        {
            Shoot(Portal_or);
            _nextShootTime = Time.time + _cooldown;
        }
    }

    void Shoot(GameObject prefab)
    {
        

        GameObject bullet = Instantiate(prefab, _shootPos.position, _shootPos.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = _shootPos.forward * _speed;
    }
}
