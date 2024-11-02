using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: MonoBehaviour
{
    public float moveSpeed = 5f;
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
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
        weapon.GetComponentInParent<Rigidbody2D>().MovePosition(rb.position);
        
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        weapon.GetComponentInParent<Rigidbody2D>().rotation = angle;
    }
}
