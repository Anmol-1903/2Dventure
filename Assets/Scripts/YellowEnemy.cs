using UnityEngine;
using Pathfinding;
public class YellowEnemy : MonoBehaviour
{
    [Header("Pathfinding")]
    [SerializeField] Transform target;
    [SerializeField] float activateDistance;
    [SerializeField] float pathUpdatePerSeconds;

    [Header("Physics")]
    [SerializeField] float speed;
    [SerializeField] float nextWaypointDistance;
    [SerializeField] float jumpNodeHeightRequirement;
    [SerializeField] float jumpModifier;

    [Header("Customs")]
    [SerializeField] Transform _bulletSpawnLocation;
    [SerializeField] bool followPlayer;
    [SerializeField] bool jumpEnabled;
    [SerializeField] bool inverted;
    [SerializeField] float turnInterval;

    Path path;
    Seeker seeker;
    Animator anim;
    Rigidbody2D rb;
    EnemyHealth health;
    BoxCollider2D collider;
    int currentWaypoint;
    bool isGrounded;
    bool shouldJump;
    [SerializeField] float counter;

    Vector2 dir;

    private void Start()
    {
        health = GetComponent<EnemyHealth>();
        seeker = GetComponent<Seeker>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        counter = turnInterval;
        target = FindObjectOfType<PlayerController>().transform;

        InvokeRepeating("UpdatePath", 0f, pathUpdatePerSeconds);
    }
    private void FixedUpdate()
    {
        TargetInDistance();
        if (followPlayer)
        {
            anim.SetBool("PlayerFound", true);
            PathFollow(true);
        }
        else
        {
            anim.SetBool("PlayerFound", false);
            PathFollow(false);
        }
    }
    void UpdatePath()
    {
        if (followPlayer && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }
    void PathFollow(bool isFollowingPlayer)
    {
        if (isFollowingPlayer)
        {
            //RETURNS IF NO PATH BAKED
            if (path == null)
            {
                return;
            }

            if (currentWaypoint >= path.vectorPath.Count)
            {
                return;
            }

            //GROUND STATE CHECK
            isGrounded = collider.IsTouchingLayers(LayerMask.GetMask("Ground"));

            //DIRECTION CHECK
            dir = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = speed * Time.deltaTime * dir;
            //UPDATES IF ENEMY CAN JUMP OR NOT
            if (target.position.y - transform.position.y > 1f)
            {
                jumpEnabled = true;
            }
            else
            {
                jumpEnabled = false;
            }

            //JUMP
            if (dir.y > jumpNodeHeightRequirement && isGrounded && (shouldJump || jumpEnabled))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpModifier);
            }

            // MOVEMENT
            rb.velocity = new Vector2(force.x, rb.velocity.y);

            //NEXT WAWYPOINT
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            //SPRITE FLIP
            if (transform.position.x < target.position.x)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 1, LayerMask.GetMask("Ground"));
                Debug.DrawRay(transform.position, Vector2.right, Color.red);
                inverted = true;
                if (hit.collider != null)
                {
                    shouldJump = true;
                }
                else
                {
                    shouldJump = false;
                }
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, 1, LayerMask.GetMask("Ground"));
                Debug.DrawRay(transform.position, Vector2.left, Color.red);
                inverted = false;
                if (hit.collider != null)
                {
                    shouldJump = true;
                }
                else
                {
                    shouldJump = false;
                }
            }
        }
        else
        {
            if(counter <= 0)
            {
                counter = turnInterval;
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
                inverted = !inverted;
            }
            else
            {
                counter -= Time.deltaTime;
            }
        }
    }
    void TargetInDistance()
    {

        if (inverted)
        {
            transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, activateDistance, LayerMask.GetMask("Player") | LayerMask.GetMask("Ground"));
            Debug.DrawRay(transform.position, Vector2.right * activateDistance, Color.blue);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                followPlayer = true;
            }
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, activateDistance, LayerMask.GetMask("Player") | LayerMask.GetMask("Ground"));
            Debug.DrawRay(transform.position, Vector2.left * activateDistance, Color.blue);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                followPlayer = true;
            }
        }
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
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
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