using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Laser : MonoBehaviour
{
    private float defDistanceRay = 100;
    public  LineRenderer lineRendererDamage;
    public  LineRenderer lineRendererPush;
    public Transform firePoint;
    public GameObject bulletPrefab;
    
    LayerMask _layerMask;
    private Vector2 _localDirection;
    private readonly Vector2 _boxExtents = new(2f, 2f);
    private float _angle;
    private bool _canShoot = true;
    private bool _canPush = true;
    private bool _isPushing = false;
    private PlayerController _playerController;
    
    public static Vector2 rotate(Vector2 v, float delta) {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }
    void OnDrawGizmos()

    {
        Vector2 rotatedBox = rotate(new Vector2(0.2f, 0.2f), Mathf.Deg2Rad * _angle);
        DrawBoxLines(transform.position, transform.position + new Vector3(
            _localDirection.x, _localDirection.y, 0
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
        _layerMask = LayerMask.GetMask("Ignore Raycast");
        _layerMask = ~_layerMask;
        lineRendererDamage.enabled = false;
        lineRendererPush.enabled = false;
        _playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }
    
    private void ResetPush()
    {
        _canPush = true;
    }

    private void FixedUpdate()
    {
        if (_isPushing)
        {
            ShootLaserPush();
        }
    }

    private void StopPush()
    {
        _isPushing = false;
        RemoveLaserPush();
    }

    public void TriggerLaserPush()
    {
        if (!_canPush)
        {
            return;
        }
        _isPushing = true;
        _canPush = false;
        Invoke(nameof(ResetPush), 10f);
        Invoke(nameof(StopPush), 1f);
    }
    
    private void ShootLaserPush()
    {
        lineRendererPush.enabled = true;

        _angle = Vector2.SignedAngle(transform.right, new(1, 0));
        _localDirection = transform.right;
        RaycastHit2D[] hit = Physics2D.BoxCastAll(transform.position, _boxExtents, _angle, transform.right, defDistanceRay, _layerMask);
        if (hit.Length > 0)
        {
            foreach (var h in hit)
            {
                if (h.collider.CompareTag("Enemy"))
                {
                    h.collider.GetComponent<EnemyController>().Damage(transform.right, _playerController.pushForce, _playerController.pushDamage, false);
                }
            }
        }

        Draw2DRay(lineRendererPush, transform.position, transform.right * defDistanceRay);
    }

    private void ResetShoot()
    {
        _canShoot = true;
    }
    
    public void ShootLaserDamage()
    {
        if (_canShoot)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            LaserBullet controller = bullet.GetComponent<LaserBullet>();
            controller.direction = transform.right;
            controller.transform.rotation = transform.rotation;
            controller.transform.position = transform.position;
            controller.GetComponent<LineRenderer>().widthMultiplier = _playerController.bulletWidth;
            controller.damage = _playerController.bulletDamage;
            controller.force = _playerController.bulletForce;
            _canShoot = false;
            Invoke(nameof(ResetShoot), 0.1f);
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
