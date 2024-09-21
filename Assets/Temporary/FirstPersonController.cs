using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    #region
    public static Transform instance;
    #endregion

    public bool CanMove { get; private set; } = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && characterController.isGrounded;

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Crouching Parameters")]
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float crouchHeight = 1.5f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching = false;

    private Camera playerCamera;
    private CharacterController characterController;
    private PlayerStats stats;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    [Header("Animations")]
    private Animator anim;

    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        instance = this.transform;

        GetReferences();
    }

    void Update()
    {
        if(!stats.IsDead())
        {
            if (CanMove)
            {
                HandleMovementInput();
                HandleMouseLook();

                if (canJump)
                {
                    HandleJump();
                }

                if (canCrouch)
                {
                    HandleCrouch();
                }

                ApplyFinalMovement();

                HandleAnimations();
            }
        }
        else
        {
            //unlocking cursor to navigate menus
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void HandleMovementInput()
    {
        float targetSpeed = isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed;
        float moveDirectionY = moveDirection.y;

        if (characterController.isGrounded)
        {
            currentInput = new Vector2(targetSpeed * Input.GetAxisRaw("Vertical"), targetSpeed * Input.GetAxisRaw("Horizontal"));
            moveDirectionY = -1;
        }
        else
        {
            currentInput = new Vector2(targetSpeed * Input.GetAxis("Vertical"), targetSpeed * Input.GetAxis("Horizontal"));
        }

        Vector3 targetMoveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        if (targetMoveDirection.sqrMagnitude > 1)
        {
            targetMoveDirection.Normalize();
        }

        targetMoveDirection *= targetSpeed;

        if (characterController.isGrounded)
        {
            moveDirection = targetMoveDirection;
        }
        else
        {
            moveDirection = Vector3.Lerp(moveDirection, targetMoveDirection, Time.deltaTime * 5);
        }

        moveDirection.y = moveDirectionY;
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void HandleJump()
    {
        if(ShouldJump)
        {
            moveDirection.y = jumpForce;
        }
    }
    
    private void HandleCrouch()
    {
        if (ShouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }

    private void ApplyFinalMovement()
    {
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f)) {
            yield break;
        }

        float timeElapsed = 0f;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 currentCenter = characterController.center;
        Vector3 targetCenter = currentCenter;

        // Calculate the difference in height
        float heightDifference = currentHeight - targetHeight;
        float centerOffset = heightDifference / 2f;

        // Calculate the new center so that the bottom of the collider remains fixed
        targetCenter.y -= centerOffset;

        isCrouching = !isCrouching;

        float initialCameraYPos = playerCamera.transform.localPosition.y;
        float targetCameraYPos = isCrouching ? (1.5f - crouchHeight) : ((standingHeight/2) + 0.25f);
        //float targetCameraYPos = isCrouching ? (1.5f - crouchHeight) : ((standingHeight / 2) - 0.5f);

        while (timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);

            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, Mathf.Lerp(initialCameraYPos, targetCameraYPos, timeElapsed / timeToCrouch), playerCamera.transform.localPosition.z);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Finalize height and center
        characterController.height = targetHeight;
        characterController.center = targetCenter;
        playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, targetCameraYPos, playerCamera.transform.localPosition.z);
    }

    private void GetReferences()
    {
        characterController = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        stats = GetComponent<PlayerStats>();
    }

    private void HandleAnimations()
    {
       if(moveDirection == Vector3.zero)
       {
           anim.SetFloat("Speed", 0f, 0.2f, Time.deltaTime);
       }
       else if(moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
       {
           anim.SetFloat("Speed", 0.5f, 0.2f, Time.deltaTime);
       }
       else if(moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
       {
           anim.SetFloat("Speed", 1f, 0.2f, Time.deltaTime);
       }
    }
}
