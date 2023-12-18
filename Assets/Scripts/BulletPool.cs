using System.Collections.Generic;
using UnityEngine;
public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private int poolSize = 1;

    private List<GameObject> bulletPool;
    private List<GameObject> enemyBulletPool;
    private void Awake()
    {
        Instance = this;
        InitializePool();
        InitializeEnemyPool();
    }
    private void InitializePool()
    {
        bulletPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Add(bullet);
        }
    }
    private void InitializeEnemyPool()
    {
        enemyBulletPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(enemyBulletPrefab);
            bullet.SetActive(false);
            enemyBulletPool.Add(bullet);
        }
    }
    public GameObject GetBullet()
    {
        foreach (GameObject bullet in bulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
        }
        // If all bullets are in use, create a new one
        GameObject newBullet = Instantiate(bulletPrefab);
        bulletPool.Add(newBullet);
        return newBullet;
    }
    public GameObject GetEnemyBullet()
    {
        foreach (GameObject bullet in enemyBulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
        }
        // If all bullets are in use, create a new one
        GameObject newBullet = Instantiate(enemyBulletPrefab);
        enemyBulletPool.Add(newBullet);
        return newBullet;
    }
}