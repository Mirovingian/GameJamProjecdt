using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGunController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float speed;

    void Update()
    {
        
    }

    public void Shoot(Transform player)
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        bullet.GetComponent<Rigidbody2D>().velocity = direction * speed;
        bullet.GetComponent<Rigidbody2D>().gravityScale = 0;
    }
}
