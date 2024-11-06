using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform graphics;
    [SerializeField] private BoxCollider2D col;
    private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float variableJumpForce = 4f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Dash")]
    [SerializeField] private Vector2 dashHitboxSize;
    [SerializeField] private float dashForce = 5f;
    [SerializeField] private float dashDuration = 0.3f;
    private bool canDash;

    [Header("Parry")]
    [SerializeField] private float parryBoost = 4f;
    [SerializeField] private string parryTag = "parryable";
    [SerializeField] private float parryDistance = 0.5f;

    [Header("Jump Buffer")]
    [SerializeField] private int frame_buffer;
    [SerializeField] private List<bool> buffered_jumps = new List<bool>();

    [Header("Crouch")]
    [SerializeField] private float defaultHeight = 0.9f;
    [SerializeField] private float crouchHeight = 0.45f;

    [Header("Layers")]
    [SerializeField] private string defaultLayerName = "Player";
    [SerializeField] private string softDropLayerName = "SoftDrop";
    [SerializeField] public string onewayTag = "oneway";

    [Header("Debug")]
    [SerializeField] private GameObject parryableObject;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private bool isWalking = false;
    [SerializeField] private bool isCrouching = false;
    [SerializeField] private bool didParry = false;
    [SerializeField] private bool canParry = false;
    [SerializeField] private float moveDirection = 1;
    [SerializeField] private float moveInput;
    [SerializeField] private Vector2 crouchInput;
    [SerializeField] private Vector2 lastCrouchInput;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        CameraController.instance.AddTarget(gameObject);

        buffered_jumps = new List<bool>(new bool[frame_buffer + 1]);
    }

    void Update()
    {
        Debug.Log(moveInput);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        CheckCanParry();

        if (isGrounded && !isCrouching) { canDash = true; }

        isWalking = !isCrouching && !isDashing ? (Mathf.Abs(moveInput) > 0 ? true : false) : false;

        moveDirection = moveInput > 0 ? 1 : moveInput < 0 ? -1 : moveDirection;

        graphics.rotation = Quaternion.Euler(0, moveDirection == 1 ? 0 : moveInput == -1 ? 180 : graphics.rotation.eulerAngles.y, 0);

        if (!isDashing)
        {
            float calculatedMoveDirection = Mathf.Abs(moveInput) > 0 ? moveDirection : 0;
            rb.linearVelocity = new Vector2(isWalking ? calculatedMoveDirection * moveSpeed : 0, rb.linearVelocity.y);
        }

        CheckJumpBuffer();
        SetHitboxSize();
        UpdateAnimator();
    }

    private void SetHitboxSize()
    {
        if (isDashing) col.size = dashHitboxSize;
        else if (isCrouching) col.size = new Vector2(col.size.x, crouchHeight);
        else col.size = new Vector2(col.size.x, defaultHeight);
    }

    private void CheckJumpBuffer()
    {
        buffered_jumps.Add(false);

        if (buffered_jumps.Any(c => c == true)) AttemptJump();

        buffered_jumps.RemoveAt(0);
    }

    private void UpdateAnimator()
    {
        animator.SetBool("isDashing", isDashing);
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isCrouching", isCrouching);

        animator.SetBool("didParry", didParry);
        didParry = false;
    }

    #region Jump
    public void OnJumpInput(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        AttemptJump();

        if (!isGrounded && !isDashing && !didParry)
        {
            buffered_jumps[frame_buffer] = true;
        }
    }
    private void AttemptJump()
    {
        // Check if parry.
        if (canParry)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, parryBoost);

            Destroy(parryableObject);
            parryableObject = null;

            didParry = true;

            // Reset dash
            canDash = !isCrouching ? true : false;
        }

        // Check if jump.
        if (isGrounded && !isCrouching && !isDashing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Check if soft drop.
        if (isGrounded && isCrouching)
        {
            gameObject.layer = LayerMask.NameToLayer(softDropLayerName);
        }
    }
    private void CheckCanParry()
    {
        GameObject[] allParryableObjects = GameObject.FindGameObjectsWithTag(parryTag);
        List<GameObject> validParryableObject = new List<GameObject>();

        foreach (GameObject testParryableObject in allParryableObjects)
        {
            if (Vector2.Distance(transform.position, testParryableObject.transform.position) <= parryDistance)
            {
                validParryableObject.Add(testParryableObject);
            }
        }

        parryableObject = validParryableObject.Count > 0 ? validParryableObject[0] : null;

        canParry = !isGrounded && (validParryableObject.Count > 0) && !isDashing;
    }
    #endregion

    #region Move
    public void OnMoveInput(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<float>();
    }
    #endregion

    #region Crouch
    public void OnCrouchInput(InputAction.CallbackContext ctx)
    {
        lastCrouchInput = crouchInput;
        crouchInput = ctx.ReadValue<Vector2>();

        if (CheckCrouchAngle(crouchInput) && !CheckCrouchAngle(lastCrouchInput))
        {
            OnCrouchInputDown();
        }
        else if (!CheckCrouchAngle(crouchInput) && CheckCrouchAngle(lastCrouchInput))
        {
            OnCrouchInputUp();
        }
    }
    private bool CheckCrouchAngle(Vector2 _moveInput)
    {
        return _moveInput.y < 0 && Mathf.Abs(_moveInput.x) < 0.5f;
    }
    private void OnCrouchInputDown()
    {
        transform.position = (Vector2)transform.position - new Vector2(0, (defaultHeight / 2) - (crouchHeight / 2));

        isCrouching = true;
        canDash = false;
    }
    private void OnCrouchInputUp()
    {
        transform.position = (Vector2)transform.position + new Vector2(0, (defaultHeight / 2) - (crouchHeight / 2));

        isCrouching = false;
    }
    #endregion

    public void OnDashInput(InputAction.CallbackContext ctx)
    {
        if (!isDashing && canDash)
        {
            canDash = false;
            isDashing = true;
            StartCoroutine(DashCoroutine());
        }
    }

    public void OnSoftDropExit()
    {
        gameObject.layer = LayerMask.NameToLayer(defaultLayerName);
    }

    IEnumerator DashCoroutine()
    {
        rb.gravityScale = 0f;
        float dashDirection = moveDirection == 1 ? 1 : -1;
        rb.linearVelocity = new Vector2(dashDirection * dashForce, 0);
        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = 1f;
        isDashing = false;
    }
}