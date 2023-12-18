using UnityEngine;
public class Health : MonoBehaviour
{
    int MAX_HEALTH = 5;
    [SerializeField] int _currentHealth;

    [Header("Player")]
    [SerializeField] bool isPlayer;
    [SerializeField] GameObject[] emptyHearts;
    [SerializeField] GameObject[] filledHearts;
    private void Start()
    {
        _currentHealth = MAX_HEALTH;
        if (!isPlayer)
            return;
        for(int i = 0; i < emptyHearts.Length; i++)
        {
            if(i >= MAX_HEALTH)
            {
                emptyHearts[i].SetActive(false);
            }
        }
        for(int i = 0; i < filledHearts.Length; i++)
        {
            if(i >= _currentHealth)
            {
                filledHearts[i].SetActive(false);
            }
        }
    }
    public void TakeDamage()
    {
        _currentHealth -= 1;
        if (isPlayer)
        {
            for (int i = 0; i < filledHearts.Length; i++)
            {
                if (i >= _currentHealth)
                {
                    filledHearts[i].SetActive(false);
                }
                else
                {
                    filledHearts[i].SetActive(true);
                }
            }
        }
        if (_currentHealth <= 0)
        {
            GetComponentInChildren<PlayerAnimations>().Dead();
        }
    }
    public void Heal(int _amt)
    {
        _currentHealth += _amt;
        if(_currentHealth > MAX_HEALTH)
        {
            _currentHealth = MAX_HEALTH;
        }
        if(!isPlayer)
            return;
        for(int i = 0; i < filledHearts.Length; i++)
        {
            if(i >= _currentHealth)
            {
                filledHearts[i].SetActive(false);
            }
            else
            {
                filledHearts[i].SetActive(true);
            }
        }
    }
}