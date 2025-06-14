using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour, IEntityController
{
    [SerializeField] private float detectionRadius = 20f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private EnemyGunController gun;

    [SerializeField] private float fireRate = 1f;
    private float _defaultFireRate;

    private Transform player;
    private float nextFireTime;
    private bool canSeePlayer;
    private void OnSlowDownStart()
    {
        _defaultFireRate = fireRate;
        fireRate *= 0.5f;
    }
    private void OnSlowDownEnd()
    {
        fireRate = _defaultFireRate;
    }

    private void OnEnable()
    {
        GameEntryPoint._instance.OnOverdoseStart += OnSlowDownStart;
        GameEntryPoint._instance.OnOverdoseEnd += OnSlowDownEnd;
    }

    private void OnDisable()
    {
        GameEntryPoint._instance.OnOverdoseStart -= OnSlowDownStart;
        GameEntryPoint._instance.OnOverdoseEnd += OnSlowDownEnd;
    }

    void Update()
    {
        FindPlayer();

        gun.SetTargetPoint(player);

        if (canSeePlayer && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }

        if (player != null)
        {
            if ((transform.position - player.position).x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            if ((transform.position - player.position).x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
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
                    Debug.Log(hit.gameObject.name);
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
        Debug.Log("Shoot");
        if (player == null) return;

        gun.Shoot(player);
    }

    public void ChangeHealth(int amount)
    {
        Debug.Log("Change Helath Enemy");
    }
}
