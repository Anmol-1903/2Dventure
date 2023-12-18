using UnityEngine;
using Pathfinding;
public class GreenDude : MonoBehaviour
{
    [Header("Pathfinding")]
    [SerializeField] Transform target;
    [SerializeField] Transform A;
    [SerializeField] Transform B;
    [SerializeField] float activateDistance;
    [SerializeField] float pathUpdatePerSeconds;

    [Header("Physics")]
    [SerializeField] float speed;
    [SerializeField] float nextWaypointDistance;
    [SerializeField] float jumpNodeHeightRequirement;
    [SerializeField] float jumpModifier;

    [Header("Customs")]
    [SerializeField] bool followEnabled;
    [SerializeField] bool followPlayer;
    [SerializeField] bool jumpEnabled;
    [SerializeField] bool inverted;

    Path path;
    Seeker seeker;
    Animator anim;
    Transform waypoint;
    Rigidbody2D rb;
    new BoxCollider2D collider;
    int currentWaypoint;
    bool isGrounded;
    bool shouldJump;

    Vector2 dir;

    private void Start()
    {
        waypoint = A;
        seeker = GetComponent<Seeker>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        target = FindObjectOfType<PlayerController>().transform;

        InvokeRepeating("UpdatePath", 0f, pathUpdatePerSeconds);
    }
    private void FixedUpdate()
    {
        TargetInDistance();
        if (followPlayer && followEnabled)
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
        if(followEnabled && followPlayer && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
        else if (followEnabled && seeker.IsDone())
        {
            seeker.StartPath(rb.position, waypoint.position, OnPathComplete);
        }
    }
    void PathFollow(bool isFollowingPlayer)
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

        if (isFollowingPlayer)
        {
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
    }
    void TargetInDistance()
    {

        if (inverted)
        {
            transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * activateDistance, activateDistance, LayerMask.GetMask("Player") | LayerMask.GetMask("Ground"));
            Debug.DrawRay(transform.position, Vector2.right, Color.blue);
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
    }
    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}