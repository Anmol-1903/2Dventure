using Pathfinding;
using UnityEngine;
public class PlatformEnemy : MonoBehaviour
{
    [Header("Pathfinding")]
    [SerializeField] Transform A;
    [SerializeField] Transform B;
    [SerializeField] float pathUpdatePerSeconds;

    [Header("Physics")]
    [SerializeField] float speed;
    [SerializeField] float nextWaypointDistance;

    [Header("Customs")]
    [SerializeField] int MAXHEALTH = 2;
    [SerializeField] bool inverted;
    [SerializeField] bool isDead;

    Path path;
    Seeker seeker;
    Transform waypoint;
    Rigidbody2D rb;
    EnemyHealth health;
    int currentWaypoint;

    Vector2 dir;

    private void Start()
    {
        waypoint = A;
        health = GetComponent<EnemyHealth>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, pathUpdatePerSeconds);
    }
    private void FixedUpdate()
    {
        TargetInDistance();
        if (health.GetCuttentHealth() <= 0)
        {
            rb.velocity = Vector2.zero;
            gameObject.layer = 3;
        }
        else
        {
            PathFollow();
        }
    }
    void UpdatePath()
    {
        seeker.StartPath(rb.position, waypoint.position, OnPathComplete);
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
            inverted = true;
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
            inverted = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 9)
        {
            health.TakeDamage(2);
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
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}