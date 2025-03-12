using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public CameraManager cameraManager;

    public Animator anim;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float health;
    public float horizontal;
    public float vertical;
    public Transform groundCheck;
    public Transform attackPoint;
    public Transform grabPoint;
    public Transform grabbedPoint;
    public LayerMask groundLayer;
    public LayerMask enemyLayer;

    public bool canMove;
    public bool canAttack;
    public bool canDefend;
    public bool falled;
    bool resetCombo;

    bool isBeingThrowed;

    public bool isGrabbed;

    private Rigidbody rb;
    private bool isGrounded;
    private bool facingRight = true;
    public bool isDefending;

    private bool ThrowDirection;

    public Enemy GrabbedEnemy;
    public bool isGrabbingSomeone;

    float standTimer = 2f; // Define o tempo para levantar
    float resetComboTimer = 0.33f;
    public float resetTotalComboTimer = 15f;

    public InputAction playerControls;

    public int combo;
    public int maxCombo = 4;
    public int TotalCombo = 0;
    public int hitsTaken = 0;

    // Variáveis para o sistema de defesa
    private float defenseCooldown = 5f; // Tempo de espera para defender novamente
    private float defenseCooldownTimer = 0f; // Temporizador para a espera

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraManager = FindObjectOfType<CameraManager>();
    }

    void Update()
    {
        if(!isBeingThrowed){
            Move();
        }
        GroundCheck();

        // Atualiza o temporizador de cooldown de defesa
        if (defenseCooldownTimer > 0)
        {
            defenseCooldownTimer -= Time.deltaTime;
        }

        if(falled){
            standTimer -= Time.deltaTime;
            if (standTimer <= 0)
            {
                falled = false;
                //isGrabbed = false;
                canAttack = true;
                canMove = true;
                canDefend = true;
                isBeingThrowed = false;
            }
        }

        if(combo > 0){
            resetComboTimer -= Time.deltaTime;
            if(resetComboTimer <= 0){
                combo = 0;
                //anim.SetBool("Attacking", false);
                anim.SetInteger("combo", combo);
            }
        }

        if(TotalCombo > 0){
            resetTotalComboTimer -= Time.deltaTime;
            if(resetTotalComboTimer <= 0){
                TotalCombo = 0;
                //parar de tocar solo
                //som distorcido
                //zerar UI de combo
            }
        }

        if(isGrabbed){
            transform.position = grabbedPoint.position;
        }
        
    }

    public void GetMove(InputAction.CallbackContext context)
    {
        if (!canMove)
        {
            horizontal = 0f;
            vertical = 0f;
        }
        else
        {
            Vector2 input = context.ReadValue<Vector2>();
            horizontal = input.x;
            vertical = input.y; // Para movimentação no eixo Z
        }
    }

    void Move()
    {
        if(isGrabbingSomeone){
            //rb.velocity = Vector3.zero;
            rb.velocity = new Vector3(horizontal * (moveSpeed / 2f), rb.velocity.y, vertical * (moveSpeed / 2f));
        }else{
            rb.velocity = new Vector3(horizontal * moveSpeed, rb.velocity.y, vertical * moveSpeed);
            //anim.SetFloat("Speed", Mathf.Abs((rb.velocity.x + rb.velocity.z) / 2f));
        }
        anim.SetFloat("Speed", Mathf.Abs((rb.velocity.x + rb.velocity.z) / 2f));

        if (horizontal > 0 && !facingRight)
        {
            Flip();
        }
        else if (horizontal < 0 && facingRight)
        {
            Flip();
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded && context.performed)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }

    public void Punch(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack)
        {
            rb.velocity = Vector3.zero;
            canMove = false;
            resetComboTimer = 0.33f;
            resetTotalComboTimer = 15f;
            if(combo < maxCombo){
                combo++;
            }else{
                combo = 1;
            }
            //anim.SetBool("Attacking", true);
            anim.SetInteger("combo", combo);
            if (isGrabbingSomeone)
            {
                Debug.Log("Dano no inimigo agarrado");
                GrabbedEnemy.TakeHit();
            }
            else
            {
                Collider[] colliders = Physics.OverlapSphere(attackPoint.position, 1f, enemyLayer);
                if (colliders.Length > 0)
                {
                    foreach (var collider in colliders)
                    {
                        if (collider.gameObject.GetComponent<Enemy>() != null)
                        {
                            collider.gameObject.GetComponent<Enemy>().TakeHit();
                        }
                    }
                    TotalCombo++;
                }
            }
            canMove = true;
        }
    }

    public void Defend(InputAction.CallbackContext context)
    {
        if (!canDefend || defenseCooldownTimer > 0) // Verifica se está no cooldown
        {
            return;
        }
        if (context.performed)
        {
            hitsTaken = 0;
            isDefending = true;
        }
        if (context.canceled)
        {
            isDefending = false;
        }
    }

    public void Grab(InputAction.CallbackContext context)
    {
        if (context.performed && !isGrabbingSomeone && canAttack)
        {
            Collider[] colliders = Physics.OverlapSphere(attackPoint.position, 1f, enemyLayer);
            if (colliders.Length > 0)
            {
                foreach (var collider in colliders)
                {
                    if (collider.gameObject.GetComponent<Enemy>() != null && !isGrabbingSomeone)
                    {
                        isGrabbingSomeone = true;
                        //canMove = false;
                        GrabbedEnemy = collider.gameObject.GetComponent<Enemy>();
                        collider.gameObject.GetComponent<Enemy>().currentState = Enemy.State.Grabbed;
                    }
                }
            }
        }
    }

    public void Rewind(InputAction.CallbackContext context){
        if(context.started){
            RewindScript[] rewinders = FindObjectsOfType<RewindScript>();

            foreach(RewindScript rewind in rewinders){
                rewind.StartRewind();
            }
        }
        if(context.canceled){

            RewindScript[] rewinders = FindObjectsOfType<RewindScript>();
            
            foreach(RewindScript rewind in rewinders){
                rewind.StopRewind();
            }
        }
    }

    public void SlowMotion(InputAction.CallbackContext context){
        if(context.started){
            TimeManager time = FindObjectOfType<TimeManager>();
            time.StartSlowMotion();
            anim.speed = 2f;
            //moveSpeed = moveSpeed * 2f;
        }
        if(context.canceled){
            TimeManager time = FindObjectOfType<TimeManager>();
            time.StopSlowMotion();
            anim.speed = 1f;
           // moveSpeed = moveSpeed / 2f;
        }
    }

    public void Soltar()
    {
        canMove = true;
        isGrabbingSomeone = false;
        GrabbedEnemy = null;
    }

    void GroundCheck()
    {
        isGrounded = false;

        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, 0.5f, groundLayer);
        if (colliders.Length > 0)
        {
            isGrounded = true;

            foreach (var collider in colliders)
            {
                if (collider.CompareTag("MovingPlatform"))
                {
                    transform.parent = collider.transform;
                }
                else
                {
                    transform.parent = null;
                }
            }
        }
    }

    public void TakeDamage(float dano)
    {
        combo = 0;
        if(TotalCombo > 0){
            TotalCombo = 0;
            //parar de tocar solo se estiver tocando (se total combo for maior que 0)
            //som distorcido
        }
        hitsTaken++;
        anim.SetTrigger("Hurt");
        if (!isDefending)
        {
            health -= dano;
        }
        if (hitsTaken > 2 && isDefending)
        {
            isDefending = false;
            // Inicia o cooldown de defesa se a defesa for quebrada
            defenseCooldownTimer = defenseCooldown;
            hitsTaken = 0;
        }
        if(hitsTaken > 7){
            BeThrowed(!facingRight);
            hitsTaken = 0;
        }
    }

    public void BeGrabed(Transform enemy){
        canMove = false;
        canAttack = false;
        canDefend = false;
        rb.isKinematic = true;
        isGrabbed = true;
        grabbedPoint = enemy;
    }

    public void BeThrowed(bool right){
        standTimer = 2f;
        rb.isKinematic = false;
        isBeingThrowed = true;
        ThrowDirection = right;
        if (right)
        {
            rb.AddForce((transform.right) * 7f, ForceMode.Impulse); // Aplica uma força para trás
            Debug.Log("ARREMESSADO");
        }
        else
        {
            rb.AddForce((-transform.right) * 7f, ForceMode.Impulse); // Aplica uma força para trás
            Debug.Log("ARREMESSADO");
        }
        falled = true;
        isGrabbed = false;
    }

    private void Flip()
    {
        //if(isGrabbingSomeone){
           // return;
        //}
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CameraTriggerArea")) // Usando CompareTag para melhor desempenho
        {
            cameraManager.isInFollowCamera = false;
            other.transform.GetChild(0).gameObject.SetActive(true);
            cameraManager.playerFollowCam.gameObject.SetActive(false);
        }

        if (isBeingThrowed && other.gameObject.CompareTag("Player") && rb.velocity != Vector3.zero)
        {
            Debug.Log("colisão entre jogadores");
            PlayerController otherPlayer = other.gameObject.GetComponent<PlayerController>();
            if (otherPlayer != null && !otherPlayer.isBeingThrowed && !otherPlayer.isGrabbed && !otherPlayer.isDefending && !otherPlayer.falled)
            {
                otherPlayer.BeThrowed(ThrowDirection);
            }
        }
    }

    private void OnTriggerExit(Collider other) //trocar para um checagem de inimigos na area
    {
        if (other.CompareTag("CameraTriggerArea"))
        {
            cameraManager.isInFollowCamera = true; //camera manager
            cameraManager.playerFollowCam.gameObject.SetActive(true); //camera manager
            other.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

}