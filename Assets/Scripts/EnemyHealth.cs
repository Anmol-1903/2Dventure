using UnityEngine;
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int MAXHEALTH;
    [SerializeField] bool StayAfterDeath;

    int currentHealth;
    public int GetCuttentHealth()
    {
        return currentHealth;
    }
    private void Start()
    {
        currentHealth = MAXHEALTH;
    }
    public void TakeDamage(int amt)
    {
        currentHealth -= amt;
        if(currentHealth <= 0 )
        {
            EnemyDeath();
        }
    }
    public void EnemyDeath()
    {
        if( StayAfterDeath )
        {
            GetComponent<Animator>().SetTrigger("Dead");
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}