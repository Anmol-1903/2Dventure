using UnityEngine;
public class Bullet : MonoBehaviour
{
    private void Update()
    {
        if(GetComponent<Rigidbody2D>().velocity.x > 0)
        {
            transform.localScale = new Vector2(1,1);
        }
        else
        {
            transform.localScale = new Vector2(-1,1);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        gameObject.SetActive(false);
    }
}