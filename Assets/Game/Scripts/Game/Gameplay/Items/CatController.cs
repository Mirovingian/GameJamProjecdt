using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _clip;

    private SpriteRenderer _spriteRenderer;
    private CircleCollider2D _circleCollider;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _circleCollider = GetComponent<CircleCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameEntryPoint._instance.IncreaseCatsCount();
            _audioSource.Play();
            _spriteRenderer.enabled = false;
            _circleCollider.enabled = false;
        }
    }
}
