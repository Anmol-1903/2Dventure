using UnityEngine;
public class Health : MonoBehaviour
{
    int MAX_HEALTH = 5;
    [SerializeField] int _currentHealth;
    private void Start()
    {
        _currentHealth = MAX_HEALTH;
    }
    public void TakeDamage()
    {
        _currentHealth -= 1;
        if (_currentHealth <= 0)
        {
            // Dead
        }
    }
    public void Heal(int _amt)
    {
        _currentHealth += _amt;
        if(_currentHealth > MAX_HEALTH)
        {
            _currentHealth = MAX_HEALTH;
        }
    }
}