using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerGunController : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private AudioSource _shootSound;
    private Vector3 mousePos;
    private Camera mainCam;
    public GameObject bullet;
    public Transform bulletSpawnPos1;
    public Transform bulletSpawnPos2;
    private float timer;
    private bool canFire = true;
    public float timeBetweenFiring;

    private float maxSpeed = 50f;
    private int bulletsToStop = 20;
    private int bulletsLeft = 20;
    public AnimationCurve speedCurve;

    [SerializeField] private ParticleSystem gunParticles;
    private bool isOverdosed = false;

    private void HandleOverdose()
    {
        if (GameEntryPoint._instance.isOverdose && !isOverdosed)
        {
            var forceModule = gunParticles.forceOverLifetime;

            forceModule.enabled = true;

            forceModule.x = 0f;
            forceModule.y = -9.81f / 2f;
            forceModule.z = 0f;

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
            isOverdosed = false;
        }

    }

    void Start()
    {
        try
        {
            mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }catch
        {
            Debug.LogError("Danil if you read this, this is error because you delete Camera with tag = 'MainCamera'");
        }
    }

    void Update()
    {
        Debug.Log(GameEntryPoint._instance);
        Debug.Log(GameEntryPoint._instance._playerController);
        Debug.Log(GameEntryPoint._instance._playerController.isDead);
        if (!GameEntryPoint._instance._playerController.isDead)
        {
            HandleOverdose();
            mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 rotation = mousePos - transform.position;
            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotZ);

            ProcessBullet();
        }
        
    }


    private void ProcessBullet()
    {
        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }

        if (Input.GetMouseButton(0) && canFire && !GameEntryPoint._instance._playerController.isDead)
        {
            canFire = false;
            this.FireBullet();

            //Decrease bullets here
            bulletsLeft -= 1;
            bulletsLeft = Mathf.Clamp(bulletsLeft, 0, bulletsToStop);
            GameEntryPoint._instance._uiRoot.ChangeBulletBarView((float)bulletsLeft / bulletsToStop);
        }

        if (Input.GetMouseButton(1))
        {
            Debug.Log("Start overdose");
            GameEntryPoint._instance.StartOverdose();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("End overdose");
            GameEntryPoint._instance.EndOverdose();
        }
    }
    [SerializeField] private AudioClip _audioClip;
    private void FireBullet()
    {
        var curBullet = Instantiate(bullet, bulletSpawnPos1.position, Quaternion.identity);
        var bulletRb = curBullet.GetComponent<Rigidbody2D>();
        //Calculate bullet speed
        float speed = maxSpeed;
        float difference = speedCurve.Evaluate((float)bulletsLeft / (float)bulletsToStop);
        speed *= difference;

        //Calculate bullet direction

        Vector2 direction = bulletSpawnPos2.position - bulletSpawnPos1.position;
        bulletRb.velocity = direction.normalized * speed; //here speed

        _impulseSource.GenerateImpulseWithForce(difference);
        if (bulletsLeft != 0)
            gunParticles.Play();

        _shootSound.pitch = Random.Range(0.9f, 1.1f);
        _shootSound.PlayOneShot(_audioClip, (float)bulletsLeft / bulletsToStop);
    }

    public void AddEnergy(int amount)
    {
        bulletsLeft += amount;
        bulletsLeft = Mathf.Clamp(bulletsLeft, 0, bulletsToStop);
        GameEntryPoint._instance._uiRoot.ChangeBulletBarView((float)bulletsLeft / bulletsToStop);
    }
}
