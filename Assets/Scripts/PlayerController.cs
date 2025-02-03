using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private InputSystem_Actions inputActions; // Reference to Input System Actions
    private Animator animatorController;
    [SerializeField] private bool groundedPlayer;
    private Vector2 moveInput;
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;

    private void Awake()
    {
        inputActions = new InputSystem_Actions(); // Initialize the input system
        controller = gameObject.GetComponent<CharacterController>();
        animatorController=gameObject.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove; // Reset movement when no input
        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Attack.performed += OnAttack;
        inputActions.Player.Look.performed += OnLook;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.Look.performed -= OnLook;
    }

    private void Update()
    {
        // Check if player is on the ground
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Apply movement
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move; // Rotate player in movement direction
        }

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        animatorController.SetFloat("Speed",playerSpeed);
        Debug.Log("Move Input: " + moveInput);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            animatorController.SetBool("Jump",true);
            Debug.Log("Jumped!");
        }
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        Debug.Log("Attack!");
        // Add attack logic here
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();
        Debug.Log("Look Input: " + lookInput);
        // Implement camera movement or aiming logic here
    }
}
