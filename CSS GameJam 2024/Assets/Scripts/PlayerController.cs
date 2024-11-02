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
    private Vector2 mousePos;
    
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        
        if (Mouse.current.leftButton.isPressed)
        {
            weapon.GetComponent<Laser>().ShootLaser();
        } else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            weapon.GetComponent<Laser>().RemoveLaser();
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(movement * (moveSpeed * Time.fixedDeltaTime));
        
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        weapon.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
