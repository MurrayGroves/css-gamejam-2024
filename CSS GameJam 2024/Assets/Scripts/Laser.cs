using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Laser : MonoBehaviour
{
    private float defDistanceRay = 100;
    public  LineRenderer lineRendererDamage;
    public  LineRenderer lineRendererPush;
    public Transform firePoint;
    private Transform m_transform;
    LayerMask layerMask;
    private Vector2 LocalDirection;
    private Vector2 BoxExtents = new(2f, 2f);
    private float angle;
    
    public static Vector2 rotate(Vector2 v, float delta) {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }
    void OnDrawGizmos()

    {
        Vector2 rotatedBox = rotate(BoxExtents, Mathf.Deg2Rad * angle);
        DrawBoxLines(transform.position, transform.position + new Vector3(
            LocalDirection.x, LocalDirection.y, 0
        ), rotatedBox, true);
    }

    protected void DrawBoxLines(Vector3 p1, Vector3 p2, Vector3 extents, bool boxes = false)

    {

        var length = (p2 - p1).magnitude;

        var halfExtents = extents / 2;

        var halfExtentsZ = transform.forward*halfExtents.z;

        var halfExtentsY = transform.up*halfExtents.y;

        var halfExtentsX = transform.right*halfExtents.x;

        if (boxes)

        {

            var matrix = Gizmos.matrix;

            Gizmos.matrix = Matrix4x4.TRS(p1, transform.rotation, Vector3.one);

            Gizmos.DrawWireCube(Vector3.zero, extents);

            Gizmos.matrix = Matrix4x4.TRS(p2, transform.rotation, Vector3.one);

            Gizmos.DrawWireCube(Vector3.zero, extents);

            Gizmos.matrix = matrix;

        }

// draw connect lines 1

        Gizmos.DrawLine(p1 - halfExtentsX - halfExtentsY - halfExtentsZ, p2 - halfExtentsX - halfExtentsY - halfExtentsZ);

        Gizmos.DrawLine(p1 + halfExtentsX - halfExtentsY - halfExtentsZ, p2 + halfExtentsX - halfExtentsY - halfExtentsZ);

        Gizmos.DrawLine(p1 - halfExtentsX + halfExtentsY - halfExtentsZ, p2 - halfExtentsX + halfExtentsY - halfExtentsZ);

        Gizmos.DrawLine(p1 + halfExtentsX + halfExtentsY - halfExtentsZ, p2 + halfExtentsX + halfExtentsY - halfExtentsZ);

// draw connect lines 2

        Gizmos.DrawLine(p1 - halfExtentsX - halfExtentsY + halfExtentsZ, p2 - halfExtentsX - halfExtentsY + halfExtentsZ);

        Gizmos.DrawLine(p1 + halfExtentsX - halfExtentsY + halfExtentsZ, p2 + halfExtentsX - halfExtentsY + halfExtentsZ);

        Gizmos.DrawLine(p1 - halfExtentsX + halfExtentsY + halfExtentsZ, p2 - halfExtentsX + halfExtentsY + halfExtentsZ);

        Gizmos.DrawLine(p1 + halfExtentsX + halfExtentsY + halfExtentsZ, p2 + halfExtentsX + halfExtentsY + halfExtentsZ);

    } 
    
    private void Awake()
    {
        m_transform = GetComponent<Transform>();
        layerMask = LayerMask.GetMask("Ignore Raycast");
        layerMask = ~layerMask;
        lineRendererDamage.enabled = false;
        lineRendererPush.enabled = false;
    }
    

    public void ShootLaserPush()
    {
        lineRendererPush.enabled = true;
        Debug.Log(layerMask.value);

        angle = Vector2.SignedAngle(transform.right, new(1, 0));
        LocalDirection = transform.right;
        RaycastHit2D[] hit = Physics2D.BoxCastAll(firePoint.position, BoxExtents, angle, transform.right, defDistanceRay, layerMask);
        if (hit.Length > 0)
        {
            foreach (var h in hit)
            {
                if (h.collider.CompareTag("Enemy"))
                {
                    h.collider.GetComponent<EnemyController>().Damage(firePoint.transform.right, 200, 0);
                }
            }
        }

        Draw2DRay(lineRendererPush, firePoint.position, firePoint.transform.right * defDistanceRay);
    }

    public void ShootLaserDamage()
    {
        lineRendererDamage.enabled = true;

        RaycastHit2D hit = Physics2D.Raycast(m_transform.position, transform.right, defDistanceRay, layerMask);
        if (hit)
        {
            Draw2DRay(lineRendererDamage, firePoint.position, hit.point);
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<EnemyController>().Damage(firePoint.transform.right, 10, 100);
            }
        }
        else
        {
            Draw2DRay(lineRendererDamage, firePoint.position, firePoint.transform.right * defDistanceRay);
        }   
    }
    
    public void RemoveLaserDamage()
    {
        lineRendererDamage.enabled = false;
    }
    
    public void RemoveLaserPush()
    {
        lineRendererPush.enabled = false;
    }
    
    void Draw2DRay(LineRenderer lineRenderer, Vector2 start, Vector2 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
}
