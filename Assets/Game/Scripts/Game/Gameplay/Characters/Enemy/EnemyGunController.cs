using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGunController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float speed;
    public Transform bulletSpawnPos1;
    public Transform bulletSpawnPos2;

    private Transform target;
    private bool isOverdosed = false;

    void Update()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - bulletSpawnPos2.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        } 
    }

    public void Shoot(Transform player)
    {

        Vector2 direction = (player.position - bulletSpawnPos2.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPos1.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = direction * speed;
        bullet.GetComponent<Rigidbody2D>().gravityScale = 0;
    }

    public void SetTargetPoint(Transform targetPoint)
    {
        target = targetPoint;
    }
}
