using UnityEngine;
using UnityEngine.InputSystem;

public class Jump : MonoBehaviour
{
    [SerializeField] private InputActionReference jumButton;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController _characterController;
    private Vector3 _playerVelocity;

    private void Awake() => _characterController = GetComponent<CharacterController>();

    private void OnEnable() => jumButton.action.performed += Jumping;

    private void OnDisable() => jumButton.action.performed -= Jumping;    private void Jumping(InputAction.CallbackContext obj)
    {
        if (!_characterController.isGrounded) return;

        _playerVelocity.y = Mathf.Sqrt(jumpHeight * -3f * gravity);
    }

    private void Update()
    {
        if (_characterController.isGrounded && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }

        _playerVelocity.y += gravity * Time.deltaTime;
        _characterController.Move(_playerVelocity * Time.deltaTime);
    }
}