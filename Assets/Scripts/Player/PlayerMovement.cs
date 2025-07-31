using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public PlayerManager manager;
    private InputSystem_Actions playerInputs; // Use if using the C# Class
    // private PlayerInput playerInput; // Use if using the Unity Interface

    [Header("Character Dimensions")]
    [SerializeField] public float playerRadius = 0.5f;
    [SerializeField] public float standingHeight = 1.7f;
    [SerializeField] public float crouchingHeight = 1f;

    [Header("Ground Checks")]
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] public float groundDistance = 0.03f;
    [SerializeField] public float maxSlope;
    [SerializeField] public Vector3 groundNormal = Vector3.up;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 3.5f;
    [SerializeField] private int airJumpsTotal = 1;
    [SerializeField] private int airJumpsLeft = 1;

    [Header("Movement")]
    [SerializeField] public Vector3 movement;
    [SerializeField] public PlayerMovementState movementState;
    [SerializeField] public PlayerMovementState lastMovementState = PlayerMovementState.idle;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Vector3 surfaceVelocity;
    [SerializeField] private Vector2 inputDirection;
    [SerializeField] private float baseMovementAcceleration = 100f;

    [Header("Crouching")]
    [SerializeField] private bool lastHoldCrouchState = false;
    [SerializeField] private bool holdCrouch = false;
    [SerializeField] private bool toggleCrouch = false;

    [Header("Speed Caps")]
    [SerializeField] private float maxCrouchSpeed = 4.5f;
    [SerializeField] private float maxWalkSpeed = 7.5f;
    [SerializeField] private float maxSprintSpeed = 15f;

    [Header("Objects")]
    [SerializeField] private GameObject body;
    [SerializeField] private Rigidbody player;
    [SerializeField] private CapsuleCollider playerCollider;
    const float minSpeed = 1e-4f;

    public enum PlayerMovementState
    {
        idle,
        crouching,
        walking,
        sprinting
    }
    private void Start()
    {
        manager = PlayerManager.Instance;
        playerLayer = LayerMask.GetMask("Standable");

        player = body.GetComponent<Rigidbody>();
        playerCollider = body.GetComponent<CapsuleCollider>();
        SetPlayerDimensions(standingHeight);

        playerInputs = manager.inputs;

        playerInputs.Player.Jump.performed += Jump;

        movementState = PlayerMovementState.idle;
        airJumpsLeft = airJumpsTotal;
    }
    private void Update()
    {
        velocity = player.linearVelocity; // Store the current velocity for ease of use
        surfaceVelocity = Vector3.ProjectOnPlane(velocity, groundNormal); // project the velocity on the floor
        inputDirection = playerInputs.Player.Move.ReadValue<Vector2>().normalized; // Get the input for ease of use
        bool crouching = CrouchControlState(); // Get the current crouching state

        SetMovementState(crouching); // Set the current movement state
        Crouch(); // Decide if the player should change size
        Movement(); // Generic movement function
        if (inputDirection.magnitude == 0 && surfaceVelocity.magnitude < 0.1f)
        { // If moving very slowly horizontally and with no input
            player.linearVelocity = new Vector3(0, velocity.y, 0); // Reset the horizontal movement
        }
        lastMovementState = movementState; // Store the current movement state
    }
    private void FixedUpdate()
    {
        //movement -= new Vector3(player.linearVelocity.x, 0, player.linearVelocity.z);
        player.AddForce(movement, ForceMode.VelocityChange); // Apply total movement
        movement = Vector3.zero; // Reset movement
    }
    private void SetMovementState(bool isCrouched)
    {
        if (isCrouched)
        { // If crouching
            movementState = PlayerMovementState.crouching;
        }
        else if (inputDirection.magnitude != 0 && playerInputs.Player.Sprint.IsInProgress())
        { // If the player is walking, based on if they aren't supposed to be boosting or sprinting
            movementState = PlayerMovementState.sprinting;
        }
        else if (inputDirection.magnitude != 0)
        { // If the player is walking based on if they aren't supposed to be boosting and are currently holding down the sprint key
            movementState = PlayerMovementState.walking;
        }
        else
        { // If no input is held and not in any of the other states
            movementState = PlayerMovementState.idle;
        }
    }
    private void SetPlayerDimensions(float height)
    {
        float previousHeight = playerCollider.height; // Store the current height
        playerCollider.height = height; // Set the new height
        player.transform.position -= new Vector3(0, (previousHeight - height) / 2, 0); // Work out the movement distance
    }
    private bool CrouchControlState()
    {
        holdCrouch = playerInputs.Player.HoldCrouch.inProgress; // Get the status of the hold crouch
        if (playerInputs.Player.ToggleCrouch.triggered)
        { // If the player just pressed the toggle crouch
            toggleCrouch = !toggleCrouch; // Invert the toggle crouch
        }
        if (holdCrouch)
        { // If the player is holding crouch
            lastHoldCrouchState = true;
            return true;
        }
        else
        {
            if (lastHoldCrouchState)
            { // If the player just let go of crouching
                toggleCrouch = false;
            }
            lastHoldCrouchState = false; // Store for the next iteration
            return toggleCrouch;
        }
    }
    private void Crouch()
    {
        if (lastMovementState != movementState)
        { // If the player wasn't previously crouching
            if (movementState == PlayerMovementState.crouching)
            { // If the player is supposed to be short
                SetPlayerDimensions(crouchingHeight);
            }
            else
            {
                SetPlayerDimensions(standingHeight);
            }
        }
    }
    private float MaxSpeed()
    {
        switch (movementState)
        { // Return the correct max speed based on the current movement state
            case PlayerMovementState.crouching:
                return maxCrouchSpeed;
            case PlayerMovementState.walking:
                return maxWalkSpeed;
            case PlayerMovementState.sprinting:
                return maxSprintSpeed;
            case PlayerMovementState.idle:
                return maxWalkSpeed;
            default:
                return 0;
        }
    }

    private void Movement()
    {
        float maxSpeed = MaxSpeed(); // Get the maximum speed for that state
        Vector3 target = player.rotation * new Vector3(inputDirection.x, 0, inputDirection.y); // Get the target
        float acceleration = baseMovementAcceleration;
        float alignment = 0;
        if (inputDirection.magnitude != 0)
        {
            alignment = Vector3.Dot(surfaceVelocity.normalized, target.normalized);
        }        

        if (surfaceVelocity.magnitude > maxSpeed)
        { // If moving faster that the maximum speed
            acceleration *= surfaceVelocity.magnitude / maxSpeed; // Reduce the acceleration to counteract the speed overflow
        }
        target = target * maxSpeed - surfaceVelocity; // Work out how much extra acceleration is needed

        if (target.magnitude < 0.5f) // If moving very slowly
        {
            acceleration *= target.magnitude / 0.5f;
        }
        if (alignment < 0)
        { // If attempting to move in a wildly opposite direction
            acceleration *= 2;
        }

        target = Vector3.ProjectOnPlane(target, groundNormal).normalized; // Project target on the ground
        target *= acceleration; // Work out the next movement
        movement += target * Time.deltaTime; // add it to the movement for the next physics timestep
    }
    private void Jump(InputAction.CallbackContext inputType)
    {
        Vector3 direction = Vector3.up * jumpForce;
        if (isGrounded)
        { // If on the ground
            Debug.Log("Jumped");
            movement += direction; // Jump normally
        }
    }
    public void CollisionDetected(Collision collision)
    { // This function is called externally by the body
        if (collision.contacts.Length > 0)
        { // If the player is touching something
            isGrounded = false; // Track if the player is on a floor
            foreach (ContactPoint contact in collision.contacts)
            {
                float slopeAngle = Vector3.Angle(contact.normal, Vector3.up); // Calculate the angle of the slope from the vertical

                airJumpsLeft = airJumpsTotal; // Resets the airjumps
                if (slopeAngle <= maxSlope)
                { // If on the ground
                    isGrounded = true;
                    groundNormal = contact.normal;
                    break;
                }
            }
        }
        else
        { // Not touching anything
            isGrounded = false; // In the air
            movementState = PlayerMovementState.walking; // apply normal walking state
            groundNormal = Vector3.up; // Reset groundNormal to default
        }
    }
}
