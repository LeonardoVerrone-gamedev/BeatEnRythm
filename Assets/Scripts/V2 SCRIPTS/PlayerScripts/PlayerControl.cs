using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerCombat combat;

    [SerializeField] private float playerMoveSpeed;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float playerJumpForce;

    private bool canMove = true;
    private bool canAttack = true;
    private bool isAttacking;
    public bool falled;

    private int hitsTaken;
    private float health;

    private float horizontal;
    private float vertical;

    public bool CanMove
    {
        get { return canMove; }
        set { canMove = value; }
    }

    public bool CanAttack
    {
        get { return canAttack; }
        set { canAttack = value; }
    }

    void Start()
    {
        currentSpeed = playerMoveSpeed;
        movement.control = this;
    }

    void Update()
    {
        UpdateMovementState();
        if (canMove) movement.Move(horizontal, vertical);
    }

    public void GetMove(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            Vector2 input = context.ReadValue<Vector2>();
            horizontal = input.x * currentSpeed;
            vertical = input.y * currentSpeed; // Para movimentação no eixo Z
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && canMove)
        {
            movement.Jump(playerJumpForce);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack)
        {
            isAttacking = true;
            combat.PerformAttack();
            isAttacking = false;
        }
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack)
        {
            isAttacking = true;
            combat.PerformGrab();
            isAttacking = false;
        }
    }

    public void GrabPlayer(Transform enemy)
    {
        canMove = false;
        canAttack = false;
        movement.BeGrabbed(enemy);
    }

    public void ThrowPlayer(bool right)
    {
        movement.BeThrowed(right);
    }

    public void TakeDamage(float dano)
    {
        health -= dano;
        hitsTaken++;

        if (hitsTaken > 7)
        {
            ThrowPlayer(!movement.facingRight);
            hitsTaken = 0;
        }
    }

    private void UpdateMovementState()
    {
        canMove = !isAttacking && !movement.isGrabbed;
        currentSpeed = combat.isGrabbingSomeone ? playerMoveSpeed / 2f : playerMoveSpeed;
    }

    public bool isCapacitated(){
        return !movement.isGrabbed || !falled; // || playerController.isGrabbingSomeone;
    }
}