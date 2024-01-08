using UnityEngine;
public class BlockEnemy : MonoBehaviour
{
    EnemyHealth health;
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 9)
        {
            animator.SetTrigger("Hurt");
            health.TakeDamage(1);
        }
    }
}