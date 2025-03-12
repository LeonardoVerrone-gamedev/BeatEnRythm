using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Rigidbody rb;

    [SerializeField] public PlayerControl control;

    private bool isGrounded;
    public bool facingRight = true;
    //private bool falled;
    private bool isBeingThrowed;

    private float standTimer = 3f;

    public bool isGrabbed;
    private Transform grabPoint;

    void Update()
    {
        GroundCheck();
        HandleFalling();

        if(isGrabbed){
            transform.position = grabPoint.position;
        }
    }

    public void Jump(float jumpForce)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void Move(float horizontal, float vertical)
    {
        if (isBeingThrowed || control.falled) return;

        rb.velocity = new Vector3(horizontal, rb.velocity.y, vertical);
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.magnitude));

        if (horizontal > 0 && !facingRight) Flip();
        else if (horizontal < 0 && facingRight) Flip();
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.5f, groundLayer);
    }

    public void BeGrabbed(Transform point){
        isGrabbed = true;
        grabPoint = point;
    }

    public void BeThrowed(bool right)
    {
        isGrabbed = false;
        grabPoint = null;
        standTimer = 2f;
        rb.isKinematic = false;
        isBeingThrowed = true;

        Vector3 throwDirection = right ? transform.right : -transform.right;
        rb.AddForce(throwDirection * 7f, ForceMode.Impulse);
        control.falled = true;
    }

    private void HandleFalling()
    {
        if (control.falled)
        {
            standTimer -= Time.deltaTime;
            if (standTimer <= 0)
            {
                ResetAfterFall();
            }
        }
    }

    private void ResetAfterFall()
    {
        control.falled = false;
        control.CanAttack = true; // Use a propriedade
        control.CanMove = true; // Use a propriedade
        isBeingThrowed = false;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}