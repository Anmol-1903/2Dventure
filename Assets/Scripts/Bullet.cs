using UnityEngine;
public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        gameObject.SetActive(false);
    }
}