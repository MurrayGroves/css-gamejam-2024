using UnityEngine;

public class Laser : MonoBehaviour
{
    private float defDistanceRay = 100;
    public  LineRenderer lineRenderer;
    public Transform firePoint;
    private Transform m_transform;
    LayerMask layerMask;
    
    private void Awake()
    {
        m_transform = GetComponent<Transform>();
        layerMask = LayerMask.GetMask("Ignore Raycast");
        layerMask = ~layerMask;
        lineRenderer.enabled = false;
    }
    

    public void ShootLaser()
    {
        lineRenderer.enabled = true;
        Debug.Log(layerMask.value);

        RaycastHit2D _hit = Physics2D.Raycast(m_transform.position, transform.right, defDistanceRay, layerMask);
        if (_hit)
        {
            Debug.Log(_hit.collider.name);
            Draw2DRay(firePoint.position, _hit.point);
            if (_hit.collider.CompareTag("Enemy"))
            {
                _hit.collider.GetComponent<EnemyController>().Damage(firePoint.transform.right, 100, 20);
            }
        }
        else
        {
            Draw2DRay(firePoint.position, firePoint.transform.right * defDistanceRay);
        }
    }
    
    public void RemoveLaser()
    {
        lineRenderer.enabled = false;
    }
    
    void Draw2DRay(Vector2 start, Vector2 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
}
