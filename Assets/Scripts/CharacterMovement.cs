using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour {

    [Header("Movement")]
    public float walkSpeed = 1f;

    [Header("Jumping")]
    public float jumpForce = 5f;
    public bool isGrounded = true;

    public Rigidbody2D rb;
    Animator animator;

    //Raycasts
    private Vector3 rightOrigin;
    private Vector3 leftOrigin;
    public float width;
    public float heigth;
    LayerMask Ground;
    public float RayLength = 0.1f;
    private bool IsDying;

    // Use this for initialization
    void Start ()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        IsDying = false;
        Ground = 1 << LayerMask.NameToLayer("Ground");
    }


    // Update is called once per frame
    void Update ()
    {
        if (IsDying) return;
        IsGrounded();
    }

    /*
     *  LateUpdate is called at the end of Update(). 
     *  Useful for camera scripts, animations, etc, which need to keep tracks of other elements.
    */
    private void LateUpdate()
    {
        if  (rb.velocity.y == 0)
        {
            animator.SetBool("IsJumping", false);
        }
    }

    private void FixedUpdate()
    {
        if (IsDying) return;
        Move();
        if (Input.GetButton("Jump")) Jump();
    }

    void Move()
    {
        float movement = Input.GetAxisRaw("Horizontal");
        if (movement > 0)
        {
            transform.Translate(new Vector3(walkSpeed * Time.deltaTime * movement, 0, 0));
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
            animator.SetBool("IsMoving", true);
        }
        else if (movement < 0)
        {
            transform.Translate(new Vector3(walkSpeed * Time.deltaTime * movement, 0, 0));
            transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    void MoveForces()
    {
        float movement = Input.GetAxisRaw("Horizontal");
        if (movement > 0)
        {
            rb.velocity  = new Vector2(walkSpeed * movement, rb.velocity.y);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
            animator.SetBool("IsMoving", true);
        }
        else if (movement < 0)
        {
            rb.velocity = new Vector2(walkSpeed * movement, rb.velocity.y);
            transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            animator.SetBool("IsJumping", true);
            isGrounded = false;
        }
    }

    /*
     * Is the character grounded?
     * Uses raycasts at each side of the character width.
     */
    void IsGrounded()
    {
        rightOrigin = transform.position + new Vector3(width, -heigth / 2, 0);
        leftOrigin = transform.position + new Vector3(-width, -heigth / 2, 0);

        RaycastHit2D rightRay = Physics2D.Raycast(rightOrigin, -Vector3.up, RayLength, Ground);
        RaycastHit2D leftRay = Physics2D.Raycast(leftOrigin, -Vector3.up, RayLength, Ground);

        Debug.DrawLine(rightOrigin, rightOrigin + -Vector3.up * RayLength, Color.red);
        Debug.DrawLine(leftOrigin, leftOrigin + -Vector3.up * RayLength, Color.red);


        isGrounded = rightRay.collider != null || leftRay.collider != null;
    }

    void KillPlayer()
    {
        IsDying = true;
        animator.SetBool("IsDead", true);
        Invoke("Respawn", 1f);
    }

    void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Auch!!");
            KillPlayer();
        } 
    }
}
