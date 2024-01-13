using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class Health : MonoBehaviour
{
    int MAX_HEALTH = 5;
    [SerializeField] int _currentHealth;

    [Header("Player")]
    [SerializeField] GameObject[] emptyHearts;
    [SerializeField] GameObject[] filledHearts;

    ColorAdjustments colorAdjustments;
    Volume volume;

    CinemachineStateDrivenCamera stateDrivenCamera;

    private void Awake()
    {
        stateDrivenCamera = FindObjectOfType<CinemachineStateDrivenCamera>();
        volume = FindObjectOfType<Volume>();
    }

    private void Start()
    {
        _currentHealth = MAX_HEALTH;
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
        if (volume)
        {
            if (volume.profile.TryGet(out colorAdjustments))
            {
                Debug.Log("ColorAdjustments found in the Volume component.");
            }
            else
            {
                Debug.LogWarning("ColorAdjustments not found in the Volume component.");
            }
        }
        else
        {
            Debug.LogError("Volume component not found on the player object.");
        }
    }
    private void FixedUpdate()
    {
        colorAdjustments.colorFilter.value = Color.Lerp(colorAdjustments.colorFilter.value, new Color(1f, _currentHealth / (float)MAX_HEALTH, _currentHealth / (float)MAX_HEALTH), Time.deltaTime);
        if(_currentHealth <= 0)
        {
            colorAdjustments.saturation.value = Mathf.Lerp(colorAdjustments.saturation.value, -100, Time.deltaTime);
        }
    }
    public void TakeDamage(int _amt)
    {
        _currentHealth -= _amt;

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

        if (_currentHealth <= 0)
        {
            GetComponentInChildren<PlayerAnimations>().Dead();
        }
        else
        {
            GetComponentInChildren<PlayerAnimations>().Hurt();
        }

        if (stateDrivenCamera != null)
        {
            CinemachineImpulseSource impulseSource = stateDrivenCamera.LiveChild.VirtualCameraGameObject.GetComponent<CinemachineImpulseSource>();
            impulseSource.GenerateImpulse();
        }
    }
    public void Heal(int _amt)
    {
        _currentHealth += _amt;
        if(_currentHealth > MAX_HEALTH)
        {
            _currentHealth = MAX_HEALTH;
        }
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