using UnityEngine;
public class PinkEnemy : MonoBehaviour
{
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] Transform _bulletSpawnLocation;
    [SerializeField] float _turnInterval;
    [SerializeField] bool inverted;
    [SerializeField] bool eyesOpen;

    Animator animator;
    EnemyHealth health;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
    }
    private void Start()
    {
        eyesOpen = true;
    }
    private void FixedUpdate()
    {
        if (UIManager.Instance.IsPaused())
            return;
        if (eyesOpen)
        {
            TargetInDistance();
        }
    }
    void TargetInDistance()
    {
        if (inverted)
        {
            transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 10, LayerMask.GetMask("Player") | LayerMask.GetMask("Ground"));
            Debug.DrawRay(transform.position, Vector2.right * 10, Color.blue);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                animator.SetBool("Angry", true);
            }
            else
            {
                animator.SetBool("Angry", false);
            }
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, 10, LayerMask.GetMask("Player") | LayerMask.GetMask("Ground"));
            Debug.DrawRay(transform.position, Vector2.left * 10, Color.blue);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                animator.SetBool("Angry", true);
            }
            else
            {
                animator.SetBool("Angry", false);
            }
        }
    }
    public void CloseEyes()
    {
        eyesOpen = false;
    }
    public void OpenEyes()
    {
        eyesOpen = true;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
        if (other.gameObject.layer == 9)
        {
            health.TakeDamage(2);
        }
    }
    public void ShootProjectile()
    {
        GameObject bullet = BulletPool.Instance.GetEnemyBullet();
        bullet.transform.parent = FindAnyObjectByType<BulletPool>().transform;
        bullet.transform.SetPositionAndRotation(_bulletSpawnLocation.position, Quaternion.identity);
        bullet.SetActive(true);

        if (inverted)
        {
            bullet.GetComponent<Rigidbody2D>().velocity = Vector2.right * 16;
        }
        else
        {
            bullet.GetComponent<Rigidbody2D>().velocity = Vector2.left * 16;
        }
    }
}