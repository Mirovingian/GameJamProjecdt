

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;



public class PlayerController : MonoBehaviour, IEntityController
{
    private GameInput _gameInput;

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _groundLayer;
    private Vector2 _directionToMove;
    [SerializeField] private PlayerVisualController _visualController;
    public PlayerGunController _gunController;

    private Rigidbody2D[] _bones;
    private CapsuleCollider2D _collider;

    //--------------Ñharacteristics-------------------//
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private int _currentHealth = _maxHealth;
    private const int _maxHealth = 100;

    private void Awake()
    {
        _gameInput = new GameInput();
    }

    private void Start()
    {
        _light.pointLightOuterRadius = _maxLightRange;
        _light.pointLightInnerRadius = _maxLightRange - 2.4f;
        _currentHealth = _maxHealth;
        GameEntryPoint._instance._uiRoot.ChangeHealthBarView(_currentHealth / _maxHealth);
        GameEntryPoint._instance._uiRoot.ChangeBulletBarView(1);
        _bones = GetComponentsInChildren<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
    }

    private void OnEnable()
    {
        _gameInput.Enable();
        _gameInput.Gameplay.Move.performed += ChangeDirectionToMove;
        _gameInput.Gameplay.Jump.performed += Jump;
        _gameInput.Gameplay.IncreaseVisibilityArea.performed += IncreaseVisibilityArea;
        _gameInput.Gameplay.IncreaseBulletForce.performed += IncreaseBulletForce;

    }

    private void OnDisable()
    {
        _gameInput.Disable();
        _gameInput.Gameplay.Move.performed -= ChangeDirectionToMove;
        _gameInput.Gameplay.Jump.performed -= Jump;
        _gameInput.Gameplay.IncreaseVisibilityArea.performed -= IncreaseVisibilityArea;
        _gameInput.Gameplay.IncreaseBulletForce.performed += IncreaseBulletForce;
    }


    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }


        if (OnGround())
            _visualController.Jump(false);
        else
            _visualController.Jump(true);

        DecreasingLightRange();


        if (_currentLightRange <= 0)
        {
            GameEntryPoint._instance._uiRoot.ShowRestartScreen();
        }
        else
        {
            GameEntryPoint._instance._uiRoot.HideRestartScreen();
        }

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
        _rb.velocity = new Vector2(_directionToMove.x * _speed, _rb.velocity.y);
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


    public bool isDead = false;
    public void Death()
    {
        isDead = true;
        _gameInput.Disable();
        foreach (var bone in _bones)
        {
            bone.bodyType = RigidbodyType2D.Dynamic;
        }
        _collider.enabled = false;
        _rb.bodyType = RigidbodyType2D.Static;
        _speedOfDecreasingRangeView += 1.5f;
    }

    [SerializeField] private Light2D _light;
    private const float _maxLightRange = 17;
    private const float _minLightRange = 3;
    private float _currentLightRange = _maxLightRange;
    public void IncreaseLightRange(float value)
    {
        _currentLightRange = _currentLightRange + value;
        _currentLightRange = Mathf.Clamp(_currentLightRange, _minLightRange, _maxLightRange);
        _light.pointLightOuterRadius = _currentLightRange;
        _light.pointLightInnerRadius = _currentLightRange - 2.4f;

        float valueForBar = (_currentLightRange - _minLightRange) / _maxLightRange - _minLightRange;
        GameEntryPoint._instance._uiRoot.ChangeLightBarView(valueForBar);
    }

    private void IncreaseVisibilityArea(InputAction.CallbackContext context)
    {
        if (GameEntryPoint._instance._managerPills.UseLightPill())
        {
            PlayEatingEffect();
            PlayEatingSound();
        }
       
    }

    private void IncreaseBulletForce(InputAction.CallbackContext context)
    {
        if (GameEntryPoint._instance._managerPills.UseAmmoPill())
        {
            PlayEatingEffect();
            PlayEatingSound();
        }
       
    }

    public void ChangeHealth(int amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        GameEntryPoint._instance._uiRoot.ChangeHealthBarView((float)_currentHealth / _maxHealth);
        Debug.Log((float)_currentHealth / _maxHealth);
        if (_currentHealth <= 0)
            Death();
    }

    [SerializeField] private float _speedOfDecreasingRangeView = 0.5f;
    private void DecreasingLightRange()
    {


        if (isDead && _currentLightRange < _minLightRange + 0.01f)
            _currentLightRange -= Time.deltaTime * _speedOfDecreasingRangeView;
        else
        {
            _currentLightRange -= Time.deltaTime * _speedOfDecreasingRangeView;
            _currentLightRange = Mathf.Clamp(_currentLightRange, _minLightRange, _maxLightRange);
        }

        _light.pointLightOuterRadius = _currentLightRange;
        _light.pointLightInnerRadius = _currentLightRange - 2.4f;


        float valueForBar = (_currentLightRange - _minLightRange) / (_maxLightRange - _minLightRange);
        GameEntryPoint._instance._uiRoot.ChangeLightBarView(valueForBar);
    }



    [SerializeField] private ParticleSystem eatingParticles;


    public void PlayEatingEffect()
    {
        if (eatingParticles != null)
        {
            eatingParticles.Play();
        }
    }

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _eatingTrak;
    private void PlayEatingSound()
    {
        _audioSource.PlayOneShot(_eatingTrak);
    }

}
