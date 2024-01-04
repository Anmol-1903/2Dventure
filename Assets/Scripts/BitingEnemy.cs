using UnityEngine;
public class BitingEnemy : MonoBehaviour
{
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("Bite");

            Vector2 pushDirection = other.transform.position - transform.position;
            pushDirection.Normalize();
            
            if (other.gameObject.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.AddForce(pushDirection * 10, ForceMode2D.Impulse);
            }
        }
    }
}