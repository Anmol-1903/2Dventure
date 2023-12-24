using UnityEngine;
public class PlayerAnimations : MonoBehaviour
{
    Animator animator;
    PlayerController playerController;
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerController = GetComponentInParent<PlayerController>();
    }
    public void Dead()
    {
        animator.SetTrigger("Dead");
        playerController.PlayerDeath();
    }
    public void Hurt()
    {
        animator.SetTrigger("Hurt");
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
        playerController.SpawnProjectile();
        playerController.PlayFireFX(true);
    }
    public void OnLadder(bool _bool)
    {
        animator.SetBool("OnLadder", _bool);
    }
    public void ClimbDirection(float _dir)
    {
        animator.SetFloat("ClimbDirection", _dir);
    }
    public void ParticleSystemStop()
    {
        playerController.PlayFireFX(false);
    }
}