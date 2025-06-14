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
    [SerializeField] private ParticleSystem gunParticles;

    [SerializeField] private float fireRate = 1f;
    private float _defaultFireRate;

    private Transform player;
    private float nextFireTime;
    private bool canSeePlayer;

    private Rigidbody2D[] _bones;
    private CapsuleCollider2D _collider;
    private Rigidbody2D _rb;

    private bool isOverdosed = false;

    [SerializeField] private int _currentHealth = _maxHealth;
    private const int _maxHealth = 20;
    private void Awake()
    {
        _bones = GetComponentsInChildren<Rigidbody2D>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
    }

    private void HandleOverdose()
    {
        if (GameEntryPoint._instance.isOverdose && !isOverdosed)
        {
            var forceModule = gunParticles.forceOverLifetime;

            forceModule.enabled = true;

            forceModule.x = 0f;
            forceModule.y = -9.81f / 2f;
            forceModule.z = 0f;

            _defaultFireRate = fireRate;
            fireRate *= 3f;
            isOverdosed = true;

        }
        else if (!GameEntryPoint._instance.isOverdose && !isOverdosed)
        {
            return;
        }
        else if (!GameEntryPoint._instance.isOverdose)
        {
            var forceModule = gunParticles.forceOverLifetime;

            forceModule.enabled = true;

            forceModule.x = 0f;
            forceModule.y = -9.81f;
            forceModule.z = 0f;
            fireRate = _defaultFireRate;
            isOverdosed = false;
        }

    }

    void Update()
    {
        HandleOverdose();
        FindPlayer();
        gun.SetTargetPoint(player);

        if (canSeePlayer && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
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
        if (player == null) return;
        gunParticles.Play();
        gun.Shoot(player);
    }

    public IEnumerator Death()
    {
        foreach (var bone in _bones)
        {
            bone.bodyType = RigidbodyType2D.Dynamic;
        }
        _collider.enabled = false;
        _rb.bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }


    public void ChangeHealth(int amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        if (_currentHealth <= 0)
            StartCoroutine(Death());
    }
}
