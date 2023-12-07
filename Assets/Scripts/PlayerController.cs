using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 10f;
    [SerializeField] float _jumpPower = 500f;
    [SerializeField] bool _canClimb = false;
    [SerializeField] GameObject _bulletPrefab;

    private PlayerControl playerControl = null;
    private float moveAxisX = 0f;
    private float moveAxisY = 0f;
    private Rigidbody2D rb = null;
    private void Awake()
    {
        playerControl = new PlayerControl();
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        playerControl.Enable();
        playerControl.Player.Movement.performed += OnMovementPerformed;
        playerControl.Player.Movement.canceled += OnMovementCanceled;
        playerControl.Player.Jump.performed += OnJumpPerformed;
        playerControl.Player.Climb.performed += OnClimbPerformed;
        playerControl.Player.Climb.canceled += OnClimbCanceled;
        playerControl.Player.Shoot.performed += OnShootPerformed;
    }
    private void OnDisable()
    {
        playerControl.Disable();
        playerControl.Player.Movement.performed -= OnMovementPerformed;
        playerControl.Player.Movement.canceled -= OnMovementCanceled;
        playerControl.Player.Jump.performed -= OnJumpPerformed;
        playerControl.Player.Climb.performed -= OnClimbPerformed;
        playerControl.Player.Climb.canceled -= OnClimbCanceled;
        playerControl.Player.Shoot.performed -= OnShootPerformed;
    }
    private void FixedUpdate()
    {        if (_canClimb)
        {
            rb.velocity = new Vector2(moveAxisX * _moveSpeed * Time.fixedDeltaTime / 2, moveAxisY * _moveSpeed * Time.fixedDeltaTime / 2);
        }
        else
        {
            rb.velocity = new Vector2(moveAxisX * _moveSpeed * Time.fixedDeltaTime, rb.velocity.y);
        }
    }
    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        moveAxisX = value.ReadValue<float>();
    }
    private void OnMovementCanceled(InputAction.CallbackContext value)
    {
        moveAxisX = 0f;
    }
    private void OnJumpPerformed(InputAction.CallbackContext value)
    {
        if (IsGrounded() && !_canClimb)
        {
            rb.AddForce(Vector2.up * _jumpPower);
        }
    }
    private bool IsGrounded()
    {
        if (Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 2 + 0.1f, 1 << 3))
        {
            return true;
        }
        return false;
    }
    private void OnClimbPerformed(InputAction.CallbackContext value)
    {
        moveAxisY = value.ReadValue<float>();
    }
    private void OnClimbCanceled(InputAction.CallbackContext value)
    {
        moveAxisY = 0;
    }
    private void OnShootPerformed(InputAction.CallbackContext value)
    {
        Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            _canClimb = true;
            rb.gravityScale = 0;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            _canClimb = false;
            rb.gravityScale = 1;
        }
    }
}