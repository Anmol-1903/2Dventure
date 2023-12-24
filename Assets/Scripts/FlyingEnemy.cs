using UnityEngine;
using Pathfinding;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Pathfinding")]
    [SerializeField] Transform target;
    [SerializeField] Transform patrolPointA;
    [SerializeField] Transform patrolPointB;
    [SerializeField] float activateDistance;
    [SerializeField] float pathUpdatePerSeconds;

    [Header("Physics")]
    [SerializeField] float speed;
    [SerializeField] float nextWaypointDistance;

    [Header("Customs")]
    [SerializeField] int MAXHEALTH = 2;
    [SerializeField] float viewAngle = 45f;
    [SerializeField] float rayCount = 5f;
    [SerializeField] bool followEnabled;
    [SerializeField] bool followPlayer;
    [SerializeField] bool inverted;

    Path path;
    Seeker seeker;
    Transform currentPatrolPoint;
    Rigidbody2D rb;
    EnemyHealth health;
    int currentWaypoint;
    int currentHealth;

    Vector2 dir;

    private void Start()
    {
        currentHealth = MAXHEALTH;
        currentPatrolPoint = patrolPointA;
        health = GetComponent<EnemyHealth>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        target = FindObjectOfType<PlayerController>().transform;

        rb.gravityScale = 0f; // Disable gravity

        InvokeRepeating("UpdatePath", 0f, pathUpdatePerSeconds);
    }

    private void FixedUpdate()
    {
        TargetInDistance();
        if (followPlayer && followEnabled)
        {
            PathFollow(true);
        }
        else
        {
            PathFollow(false);
        }
    }

    void UpdatePath()
    {
        if (followEnabled && followPlayer && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
        else if (followEnabled && seeker.IsDone())
        {
            seeker.StartPath(rb.position, currentPatrolPoint.position, OnPathComplete);
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
            //DIRECTION CHECK
            dir = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = speed * Time.deltaTime * dir;

            // MOVEMENT
            rb.velocity = new Vector2(force.x, force.y);

            //NEXT WAYPOINT
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            //SPRITE FLIP
            if (transform.position.x < target.position.x)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
            }
        }
        else
        {
            //DIRECTION CHECK
            dir = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = speed * Time.deltaTime * dir;

            // MOVEMENT
            rb.velocity = new Vector2(force.x, force.y);

            //NEXT WAYPOINT
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            if (Mathf.Abs(transform.position.x - patrolPointA.position.x) < nextWaypointDistance)
            {
                currentPatrolPoint = patrolPointB;
            }
            else if (Mathf.Abs(transform.position.x - patrolPointB.position.x) < nextWaypointDistance)
            {
                currentPatrolPoint = patrolPointA;
            }

            //SPRITE FLIP
            if (transform.position.x < currentPatrolPoint.position.x)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
                inverted = true;
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
                inverted = false;
            }
        }
    }

    void TargetInDistance()
    {
        float raySpacing = viewAngle / (rayCount - 1);
        if (inverted)
        {
            transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
            if(followPlayer)
                return;
            for (int i = 0; i < rayCount; i++)
            {
                float angle = transform.eulerAngles.z - viewAngle / 2 + i * raySpacing;
                Vector2 rayDirection = Quaternion.Euler(0, 0, angle) * transform.right;

                RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, activateDistance, LayerMask.GetMask("Player") | LayerMask.GetMask("Ground"));

                Debug.DrawRay(transform.position, rayDirection * activateDistance, Color.blue);

                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    followPlayer = true;
                    break;
                }
            }
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
            if(followPlayer)
                return;
            for (int i = 0; i < rayCount; i++)
            {
                float angle = transform.eulerAngles.z - viewAngle / 2 + i * raySpacing;
                Vector2 rayDirection = Quaternion.Euler(0, 0, angle) * -transform.right;

                RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, activateDistance, LayerMask.GetMask("Player") | LayerMask.GetMask("Ground"));

                Debug.DrawRay(transform.position, rayDirection * activateDistance, Color.blue);

                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    followPlayer = true;
                    break;
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
        if(other.gameObject.layer == 9)
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
}