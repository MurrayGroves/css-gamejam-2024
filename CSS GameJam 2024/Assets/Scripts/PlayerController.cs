using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: MonoBehaviour
{
    public float moveSpeed = 1000000f;
    public Rigidbody2D rb;
    public Camera cam;
    public GameObject weapon;

    private Vector2 movement;
    private Vector2 lookDir;
    
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        if (Keyboard.current.jKey.isPressed)
        {
            weapon.GetComponent<Laser>().ShootLaser();
        } else if (Keyboard.current.jKey.wasReleasedThisFrame)
        {
            weapon.GetComponent<Laser>().RemoveLaser();
        }

        if (movement.magnitude > 0)
        {
            lookDir = movement;
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(movement * (moveSpeed * Time.fixedDeltaTime));
        
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        weapon.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
