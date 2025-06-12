using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private EnemyGunController gun;

    [SerializeField] private float fireRate = 1f;

    private Transform player;
    private float nextFireTime;
    private bool canSeePlayer;
    void Update()
    {
        FindPlayer();

        if (canSeePlayer && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void FindPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);

        canSeePlayer = false;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                player = hit.transform;
                Vector2 directionToPlayer = (player.position - transform.position).normalized;

                RaycastHit2D hitObstacle = Physics2D.Raycast(
                    transform.position,
                    directionToPlayer,
                    detectionRadius,
                    obstacleLayer);

                if (!hitObstacle)
                {
                    canSeePlayer = true;
                    break;
                }
            }
        }
    }
    void Shoot()
    {
        if (player == null) return;

        gun.Shoot(player);
    }
}
