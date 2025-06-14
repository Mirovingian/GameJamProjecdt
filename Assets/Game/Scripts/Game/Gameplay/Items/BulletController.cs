using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Vector3 mousePos;
    private Camera mainCamera;
    public Rigidbody2D rb;
    private float lifeTime = 10;
    private float timer;

    private int damage = -15;
    private Vector2 velocityBeforeSlowdown;
    private float slowdownNumber = 2f;

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

    private void OnSlowDownStart()
    {
        velocityBeforeSlowdown = rb.velocity;
        rb.velocity = rb.velocity * slowdownNumber;
        rb.gravityScale *= Mathf.Pow(slowdownNumber, 2);
    }
    private void OnSlowDownEnd()
    {
        rb.velocity = velocityBeforeSlowdown;
        rb.gravityScale = 1f;
    }

    public void Start()
    {
        try
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
        catch
        {
            Debug.LogError("Danil if you read this, this is error because you delete Camera with tag = 'MainCamera'");
        }
        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = transform.position - mousePos;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 180);
    }   

    void Update()
    {
        if(timer > lifeTime)
        {
            Destroy(gameObject);
        }
        timer += Time.deltaTime;
        if (rb.velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IEntity>() != null)
        {
            collision.GetComponent<IEntity>().TakeDamage(damage);
        }
        Destroy(gameObject);

    }
}
