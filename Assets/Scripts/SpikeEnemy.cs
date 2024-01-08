using UnityEngine;
using Pathfinding;
public class SpikeEnemy : MonoBehaviour
{
    [Header("Pathfinding")]
    [SerializeField] Transform target;
    [SerializeField] Transform A;
    [SerializeField] Transform B;
    [SerializeField] float pathUpdatePerSeconds;

    [Header("Physics")]
    [SerializeField] float speed;
    [SerializeField] float nextWaypointDistance;
    [SerializeField] float jumpNodeHeightRequirement;
    [SerializeField] float jumpModifier;

    [Header("Customs")]
    [SerializeField] int MAXHEALTH = 2;
    [SerializeField] bool jumpEnabled;
    [SerializeField] bool inverted;

    Path path;
    Seeker seeker;
    Animator anim;
    Transform waypoint;
    Rigidbody2D rb;
    EnemyHealth health;
    new BoxCollider2D collider;
    int currentHealth;
    int currentWaypoint;
    bool isGrounded;
    bool shouldJump;

    Vector2 dir;

    private void Start()
    {
        currentHealth = MAXHEALTH;
        waypoint = A;
        seeker = GetComponent<Seeker>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
        collider = GetComponent<BoxCollider2D>();
        target = FindObjectOfType<PlayerController>().transform;

        InvokeRepeating("UpdatePath", 0f, pathUpdatePerSeconds);
    }
    private void FixedUpdate()
    {
        TargetInDistance();
        PathFollow();
    }
    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, waypoint.position, OnPathComplete);
        }
    }
    void PathFollow()
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
        Vector2 force = dir * speed * Time.deltaTime;

        if (Mathf.Abs(transform.position.x - A.position.x) < nextWaypointDistance)
        {
            waypoint = B;
        }
        else if (Mathf.Abs(transform.position.x - B.position.x) < nextWaypointDistance)
        {
            waypoint = A;
        }

        //UPDATES IF ENEMY CAN JUMP OR NOT
        if (waypoint.position.y - transform.position.y > 1f)
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
        if (transform.position.x < waypoint.position.x)
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
    void TargetInDistance()
    {

        if (inverted)
        {
            transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 9)
        {
            health.TakeDamage(1);
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
}