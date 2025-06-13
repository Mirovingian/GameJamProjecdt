using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour, IEntityController
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

    private void FindPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);
        canSeePlayer = false;

        string[] bodyPartsPriority = { "HeadPoint", "Body", "LegLeft", "LegRight" };

        foreach (var bodyPart in bodyPartsPriority)
        {
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player") && hit.gameObject.name == bodyPart)
                {
                    Vector2 directionToPart = (hit.transform.position - transform.position).normalized;

                    RaycastHit2D hitObstacle = Physics2D.Raycast(
                        transform.position,
                        directionToPart,
                        detectionRadius,
                        obstacleLayer);
                    if (!hitObstacle)
                    {
                        player = hit.transform;
                        canSeePlayer = true;
                        return;
                    }
                }
            }
        }
    }
    void Shoot()
    {
        if (player == null) return;

        gun.Shoot(player);
    }

    public void ChangeHealth(int amount)
    {

    }
}
