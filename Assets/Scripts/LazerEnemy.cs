using System.Collections.Generic;
using UnityEngine;

public class LaserEnemy : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] Transform waypointA;
    [SerializeField] Transform waypointB;
    [SerializeField] float patrolSpeed = 5f;
    [SerializeField] float maxSpeed = 5.0f;
    [SerializeField] float waitTime = 5f;

    [Header("Laser")]
    [SerializeField] int MAXHEALTH = 2;
    [SerializeField] float laserCooldown = 2f;
    [SerializeField] float laserLength = 12.5f;
    [SerializeField] Material laserMaterial;
    [SerializeField] GameObject particleEffects;
    [SerializeField] GameObject[] laserEndParticles;

    int currentHealth;
    private Transform waypoints;
    private bool isWaiting = false;
    private bool attacking = false;
    private Animator animator;
    EnemyHealth health;
    private Rigidbody2D rb;

    private ParticleSystem[] allParticals;

    private LineRenderer upLineRenderer;
    private LineRenderer downLineRenderer;
    private LineRenderer leftLineRenderer;
    private LineRenderer rightLineRenderer;
    private HashSet<LineRenderer> lasersThatHitPlayer = new HashSet<LineRenderer>();

    void Start()
    {
        currentHealth = MAXHEALTH;
        waypoints = waypointA;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        allParticals = particleEffects.GetComponentsInChildren<ParticleSystem>();
        InitializeLineRenderers();
        DisableLasers();
    }

    void FixedUpdate()
    {
        if (!isWaiting)
        {
            Patrol();
        }
        if (attacking)
        {
            RotateDuringAttack();

            // Shoot lasers in four directions (up, down, left, right)
            for (int i = 0; i < 4; i++)
            {
                float hitscanAngle = i * 90;

                Vector2 rayDirection = Quaternion.Euler(0, 0, hitscanAngle) * transform.right;

                RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, laserLength, LayerMask.GetMask("Player") | LayerMask.GetMask("Ground"));

                // Update LineRenderer positions
                UpdateLineRendererPosition(i, hit.point, rayDirection);

                // Handle particle system position and rotation
                if (hit.collider != null)
                {
                    // Move particle system to the hit point
                    laserEndParticles[i].transform.position = hit.point;

                    // Rotate the particle system to match the normal of the collider plus 180 degrees
                    Vector2 normal = hit.normal;
                    float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg + 180f;
                    laserEndParticles[i].transform.rotation = Quaternion.Euler(0, 0, angle);

                    // If the player is hit and the LineRenderer hasn't hit the player before during this attack
                    if (hit.collider.CompareTag("Player") && lasersThatHitPlayer.Add(GetLineRendererByDirection(i)))
                    {
                        hit.collider.GetComponent<Health>().TakeDamage();
                    }
                }
                else
                {
                    // If the collider is not hitting anything, move the particle system to the end of the ray
                    Vector3 endPoint = transform.position + (Vector3)rayDirection * laserLength;
                    laserEndParticles[i].transform.position = endPoint;

                    // Set a default rotation for when there is no hit
                    laserEndParticles[i].transform.rotation = Quaternion.Euler(0, 0, 180f);
                }
            }
        }
    }

    void InitializeLineRenderers()
    {
        // Create LineRenderers
        upLineRenderer = CreateLineRenderer("UpLineRenderer");
        downLineRenderer = CreateLineRenderer("DownLineRenderer");
        leftLineRenderer = CreateLineRenderer("LeftLineRenderer");
        rightLineRenderer = CreateLineRenderer("RightLineRenderer");
    }

    LineRenderer CreateLineRenderer(string name)
    {
        GameObject lineRendererObject = new GameObject(name);
        lineRendererObject.transform.parent = transform;
        LineRenderer lineRenderer = lineRendererObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.useWorldSpace = true;
        lineRenderer.material = laserMaterial;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.startWidth = .25f;
        lineRenderer.endWidth = .25f;
        lineRenderer.numCapVertices = 5;

        return lineRenderer;
    }

    void UpdateLineRendererPosition(int directionIndex, Vector3 hitPoint, Vector2 rayDirection)
    {
        LineRenderer lineRenderer = GetLineRendererByDirection(directionIndex);

        if (hitPoint != Vector3.zero)
        {
            // Laser hits something
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hitPoint);
        }
        else
        {
            // Laser doesn't hit anything, show it at the max length
            Vector3 endPoint = transform.position + (Vector3)rayDirection * laserLength;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, endPoint);
        }
    }

    LineRenderer GetLineRendererByDirection(int directionIndex)
    {
        switch (directionIndex)
        {
            case 0: return upLineRenderer;
            case 1: return downLineRenderer;
            case 2: return leftLineRenderer;
            case 3: return rightLineRenderer;
            default: return null;
        }
    }

    void Patrol()
    {
        // Move towards the current waypoint using Lerp
        transform.position = Vector3.Lerp(transform.position, waypoints.position, Time.deltaTime * patrolSpeed);

        // Limit the speed to the maximum
        float currentSpeed = Vector3.Distance(transform.position, waypoints.position) / Time.deltaTime;
        if (currentSpeed > maxSpeed)
        {
            transform.position = Vector3.Lerp(transform.position, waypoints.position, Time.deltaTime * maxSpeed);
        }

        // Check if the enemy has reached the current waypoint
        float distance = Vector3.Distance(transform.position, waypoints.position);
        if (distance < 0.1f)
        {
            isWaiting = true;
            ShootLaser();
        }
    }

    void RotateDuringAttack()
    {
        float rotationSpeed = 360f / laserCooldown;
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Get the current rotation angle
        float currentRotation = transform.rotation.eulerAngles.z;

        // Apply rotation to the enemy
        transform.rotation = Quaternion.Euler(0, 0, currentRotation + rotationAmount);

        // Set the exact rotation if it exceeds 360 degrees
        if (transform.rotation.eulerAngles.z >= 360)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void DisableLasers()
    {
        for (int i = 0; i < allParticals.Length; i++)
        {
            allParticals[i].Stop();
        }
        upLineRenderer.gameObject.SetActive(false);
        downLineRenderer.gameObject.SetActive(false);
        leftLineRenderer.gameObject.SetActive(false);
        rightLineRenderer.gameObject.SetActive(false);
    }

    private void EnableLasers()
    {
        for (int i = 0; i < allParticals.Length; i++)
        {
            allParticals[i].Play();
        }
        upLineRenderer.gameObject.SetActive(true);
        downLineRenderer.gameObject.SetActive(true);
        leftLineRenderer.gameObject.SetActive(true);
        rightLineRenderer.gameObject.SetActive(true);
    }

    void ShootLaser()
    {
        EnableLasers();
        rb.velocity = Vector2.zero;
        animator.SetBool("Attack", true);
        attacking = true;

        // Wait for the cooldown before moving to the next waypoint
        Invoke("EndShoot", laserCooldown);
        Invoke("NextWaypoint", waitTime);
    }

    void EndShoot()
    {
        DisableLasers();
        animator.SetBool("Attack", false);
        attacking = false;
        lasersThatHitPlayer.Clear();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 9)
        {
            health.TakeDamage(2);
        }
    }

    void NextWaypoint()
    {
        isWaiting = false;
        if (waypoints == waypointA)
        {
            waypoints = waypointB;
        }
        else
        {
            waypoints = waypointA;
        }
    }
}
