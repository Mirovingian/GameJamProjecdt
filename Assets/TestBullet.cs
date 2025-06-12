using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBullet : MonoBehaviour
{
    public Rigidbody2D rb;

    public Vector2 speed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.velocity = speed;
    }
}
