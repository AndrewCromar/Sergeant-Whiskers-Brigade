using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform graphics;
    private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Dash")]
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

    [Header("Debug")]
    [SerializeField] private GameObject parryableObject;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private bool isWalking = false;
    [SerializeField] private bool isCrouching = false;
    [SerializeField] private bool didParry = false;
    [SerializeField] private bool canParry = false;
    [SerializeField] private float moveDirection = 1;
    private float moveInput;

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
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        CheckCanParry();

        if(isGrounded && !isCrouching){ canDash = true; }

        isWalking = !isCrouching && !isDashing ? (Mathf.Abs(moveInput) > 0 ? true : false) : false;

        moveDirection = moveInput > 0 ? 1 : moveInput < 0 ? -1 : moveDirection;

        graphics.rotation = Quaternion.Euler(0, moveDirection == 1 ? 0 : moveInput == -1 ? 180 : graphics.rotation.eulerAngles.y, 0);

        if (!isDashing)
        {
            float calculatedMoveDirection = Mathf.Abs(moveInput) > 0 ? moveDirection : 0;
            rb.velocity = new Vector2(isWalking ? calculatedMoveDirection * moveSpeed : 0, rb.velocity.y);
        }

        CheckJumpBuffer();
        UpdateAnimator();
    }

    private void CheckJumpBuffer()
    {
        buffered_jumps.Add(false);

        if(buffered_jumps.Any(c=> c == true)) onJump();

        buffered_jumps.RemoveAt(0);
    }

    private void UpdateAnimator(){
        animator.SetBool("isDashing", isDashing);
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isCrouching", isCrouching);

        animator.SetBool("didParry", didParry);
        didParry = false;
    }

    private void CheckCanParry(){
        GameObject[] allParryableObjects = GameObject.FindGameObjectsWithTag(parryTag);
        List<GameObject> validParryableObject = new List<GameObject>();

        foreach(GameObject testParryableObject in allParryableObjects){
            if(Vector2.Distance(transform.position, testParryableObject.transform.position) <= parryDistance){
                validParryableObject.Add(testParryableObject);
            }
        }

        parryableObject = validParryableObject.Count > 0 ? validParryableObject[0] : null;

        canParry = !isGrounded && (validParryableObject.Count > 0) && !isDashing;
    }

    public void onMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<float>();
    }

    public void onJumpInput(InputAction.CallbackContext ctx)
    {
        if(!ctx.performed) return;

        onJump();

        if(!isGrounded && !isDashing && !didParry)
        {
            buffered_jumps[frame_buffer] = true;
        }
    }

    private void onJump(){
        // Check if parry.
        if(canParry){
            rb.velocity = new Vector2(rb.velocity.x, parryBoost);

            Destroy(parryableObject);
            parryableObject = null;

            didParry = true;

            // Reset dash
            canDash = !isCrouching ? true : false;
        }

        // Check if jump.
        if (isGrounded && !isCrouching && !isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    public void onDash(InputAction.CallbackContext ctx)
    {
        if (!isDashing && canDash)
        {
            canDash = false;
            isDashing = true;
            StartCoroutine(DashCoroutine());
        }
    }

    public void onCrouch(InputAction.CallbackContext ctx){
        if (ctx.started){
            isCrouching = true;
            canDash = false;
        }else if (ctx.canceled){
            isCrouching = false;
        }
    }

    IEnumerator DashCoroutine()
    {
        rb.gravityScale = 0f;
        float dashDirection = moveDirection == 1 ? 1 : -1;
        rb.velocity = new Vector2(dashDirection * dashForce, 0);
        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = 1f;
        isDashing = false;
    }
}