using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerGunController : MonoBehaviour
{

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
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePos - transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
       


        ProcessBullet();


        //Test pill right there
        if (Input.GetKey(KeyCode.Q) && canFire)
        {
            canFire = false;
            this.AddEnergy(10);
    
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

        if (Input.GetMouseButton(0) && canFire)
        {
            canFire = false;
            this.FireBullet();

            //Decrease bullets here
            bulletsLeft -= 1;
            bulletsLeft = Mathf.Clamp(bulletsLeft, 0, bulletsToStop);

 
        }
    }

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
        curBullet.transform.localScale = Vector3.one * 5; 

       /* Vector3 direction = mousePos - curBullet.transform.position;
        if (Mathf.Abs(mousePos.x) < Mathf.Abs(transform.position.x) && Mathf.Abs(mousePos.y) < Mathf.Abs(transform.position.y))
        {
            direction = curBullet.transform.position - mousePos;
        }
        bulletRb.velocity = new Vector2(direction.x, direction.y).normalized * speed; //here speed
        curBullet.transform.localScale = Vector3.one * 5;*/
    }

    public void AddEnergy(int amount)
    {
        bulletsLeft += amount;
        bulletsLeft = Mathf.Clamp(bulletsLeft, 0, bulletsToStop);
    }
}
