using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerController : MonoBehaviour
{
    private GameInput _gameInput;

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _groundLayer;
    private Vector2 _directionToMove;
    [SerializeField] private PlayerVisualController _visualController;

    private Rigidbody2D[] _bones;
    private CapsuleCollider2D _collider;

    //--------------Ñharacteristics-------------------//
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;

    private void Awake()
    {
        _gameInput = new GameInput();
    }

    private void Start()
    {
        _bones = GetComponentsInChildren<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
    }

    private void OnEnable()
    {
        _gameInput.Enable();
        _gameInput.Gameplay.Move.performed += ChangeDirectionToMove;
        _gameInput.Gameplay.Jump.performed += Jump;
    }

    private void OnDisable()
    {
        _gameInput.Disable();
        _gameInput.Gameplay.Move.performed -= ChangeDirectionToMove;
        _gameInput.Gameplay.Jump.performed -= Jump;
    }


    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Death();
        }

        if (OnGround())
            _visualController.Jump(false);
        else
            _visualController.Jump(true);
    }

    private void ChangeDirectionToMove(InputAction.CallbackContext context)
    {
        _directionToMove = new Vector2(context.ReadValue<Vector2>().x, _rb.velocity.y);
        if (_directionToMove.x < 0)
            _visualController.FlipLeft();
        else if (_directionToMove.x > 0)
            _visualController.FlipRight();
        else
        {
            _visualController.Idle();
            return;
        }
        _visualController.Run();
    }


    private void Move()
    {
        _rb.velocity = new Vector2 (_directionToMove.x * _speed, _rb.velocity.y);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (OnGround())
        {
            _rb.AddForce(Vector2.up * _jumpForce);
        }
    }


    private bool OnGround()
    {
        if (Physics2D.OverlapCircle(_groundCheckPoint.position, _groundCheckRadius, _groundLayer) != null)
        {
            return true;
        }
        return false;
    }

    private void Death()
    {
        foreach(var bone in _bones)
        {
            bone.bodyType = RigidbodyType2D.Dynamic;
        }
        _collider.enabled = false;
    }
}
