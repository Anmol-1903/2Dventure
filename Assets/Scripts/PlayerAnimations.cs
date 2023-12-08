using UnityEngine;
public class PlayerAnimations : MonoBehaviour
{
    Animator animator;
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public void Run(bool _bool)
    {
        animator.SetBool("Running", _bool);
    }
    public void AttackIdle()
    {
        animator.SetBool("Running", false);
        animator.SetTrigger("Attack");
    }
    public void AttackRunning()
    {
        animator.SetBool("Running", true);
        animator.SetTrigger("Attack");
    }
    public void Jump()
    {
        animator.SetTrigger("Jump");
    }
    public void DoubleJump()
    {
        animator.SetTrigger("DoubleJump");
    }
    public void LandJump()
    {
        animator.SetTrigger("Land");
    }
    public void ShootProjectile()
    {
        Debug.Log("Event Called");
        GetComponentInParent<PlayerController>().SpawnProjectile();
    }
}