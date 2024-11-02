using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: MonoBehaviour
{
    public float moveSpeed = 1000000f;
    public Rigidbody2D rb;
    public Camera cam;
    public GameObject weapon;
    private Laser _laser;
    
    private Vector2 movement;
    private Vector2 lookDir;
    
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        if (Keyboard.current.jKey.isPressed)
        {
            _laser.ShootLaserDamage();
        } else if (Keyboard.current.jKey.wasReleasedThisFrame)
        {
            _laser.RemoveLaserDamage();
        }
        
        if (Keyboard.current.kKey.isPressed)
        {
            _laser.ShootLaserPush();
        } else if (Keyboard.current.kKey.wasReleasedThisFrame)
        {
            _laser.RemoveLaserPush();
        }

        if (movement.magnitude > 0)
        {
            lookDir = movement;
        }
    }
    
    public void FetchLaser()
    {
        _laser = weapon.GetComponent<Laser>();
    }

    public void Awake()
    {
        FetchLaser();
    }

    private void FixedUpdate()
    {
        rb.AddForce(movement * (moveSpeed * Time.fixedDeltaTime));
        
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        weapon.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
