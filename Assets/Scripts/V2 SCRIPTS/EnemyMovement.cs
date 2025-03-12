using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Variables
    public enum State
    {
        Idle,
        Approaching,
        Attack,
        Grab,
        Throw,
        CoolDown,
        Grabbed,
        Fall,
        Stand,
        Die,
        Waiting,
        Stunned
    }

    public State currentState = State.Idle;

    public int MaxHitBeforeFall;
    public float stunDuration = 2f;

    public Transform player;
    public Transform otherPlayer;
    public Transform[] players;

    public Transform attackPoint;
    public Transform GrabPoint;
    public LayerMask playerLayer;
    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    public float cooldownTime = 1f;
    public float grabTime = 2f;
    public float fallTime = 3f;
    public int health = 5;
    public float dano;

    private float cooldownTimer;
    private float grabTimer;
    private float stunTimer;
    private float ChangeTargetTimer;
    public int hitsTaken = 0;
    public int attackingEnemiesCount = 0;
    public int maxAttackingEnemies = 3;

    private Rigidbody rb;
    public Animator animator;

    private bool hasFallen = false;
    private float standTimer;
    private bool facingRight = true;
    private bool isGrabing;
    private bool isGrabbed;
    private bool TargetChanged;

    bool ThrowDirection_right;

    public PlayerControl grabbedPlayer;

    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameObject[] _players = GameObject.FindGameObjectsWithTag("Player");
        players = new Transform[_players.Length];
        for (int i = 0; i < _players.Length; i++)
        {
            players[i] = _players[i].transform;
        }
        currentState = State.Idle;
        //EscolheJogador();
    }

    void Update()
    {
        #region State SwitchCase
        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Approaching:
                HandleApproaching();
                break;
            case State.Attack:
                HandleAttack();
                break;
            case State.Grab:
                HandleGrab();
                break;
            case State.Throw:
                HandleThrow();
                break;
            case State.CoolDown:
                HandleCoolDown();
                break;
            case State.Grabbed:
                HandleGrabbed();
                break;
            case State.Fall:
                HandleFall();
                break;
            case State.Stand:
                HandleStand();
                break;
            case State.Die:
                HandleDie();
                break;
            case State.Waiting:
                HandleWaiting();
                break;
            case State.Stunned:
                HandleStunned();
                break;
        }
        #endregion

        FlipCheck();

        if(isGrabbed){
            transform.position = player.GetComponent<PlayerController>().attackPoint.position;
        }
    }
    #region Idle
    private void HandleIdle()
    {
        if(players.Length > 1){
            if(Vector3.Distance(players[0].position, transform.position) < Vector3.Distance(players[1].position, transform.position)){
                player = players[0];
                otherPlayer = players[1];
            }else{
                player = players[1];
                otherPlayer = players[0];
            }
        }else{
            player = players[0];
        }
        animator.SetFloat("Speed", 0f);
        if (CanApproach())
        {
            currentState = State.Approaching;
        }
        else
        {
            currentState = State.Waiting;
        }
    }
    #endregion

    #region Approaching
    private void HandleApproaching()
    {
        if (IsPlayerIncapacitated())
        {
            currentState = State.Waiting;
            return;
        }

        Vector3 targetPosition = GetTargetPosition();
        rb.MovePosition(Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime));
        animator.SetFloat("Speed", 1f);

        if (IsInAttackRange())
        {
            if (CanAttack())
            {
                currentState = State.Attack;
            }
            else
            {
                currentState = State.Waiting;
            }
        }
    }
    #endregion

    #region attack and grab
    private void HandleAttack()
    {
        Debug.Log("Ataque inimigo");
        rb.velocity = Vector3.zero;
        animator.SetFloat("Speed", 0f);

        if (CanAttack())
        {
            if (Random.Range(0, 100) < 80) // 80% de chance para ataque normal
            {
                PerformNormalAttack();
                Debug.Log("Atacou");
            }
            else
            {
                if(player.GetComponent<PlayerCombat>().isGrabbingSomeone == false){
                    PerformGrabAttack();
                }else{
                    PerformNormalAttack();
                    Debug.Log("Atacou");
                }
            }
        }
        else
        {
            Debug.Log("Não pode atacar o player");
            currentState = State.CoolDown;
            cooldownTimer = cooldownTime;
        }
    }

    private void HandleGrab()
    {
        isGrabing = true;
        //grabbedPlayer.facingRight = !facingRight;
        if(players.Length >1){
            Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, otherPlayer.position.z);
            rb.MovePosition(Vector3.MoveTowards(transform.position, targetPos, (moveSpeed / 2f) * Time.deltaTime));
            
            if(otherPlayer.position.x < transform.position.x){
                ThrowDirection_right = false;
            }else{
                ThrowDirection_right = true;
            }
        }else{
            ThrowDirection_right = false;
        }
        grabTimer -= Time.deltaTime;
        if (grabTimer <= 0)
        {
            currentState = State.Throw;
        }
    }

    private void HandleThrow()
    {
        isGrabing = false;
        grabbedPlayer.ThrowPlayer(ThrowDirection_right);
        currentState = State.CoolDown;
    }

    private void PerformNormalAttack()
    {
        animator.SetTrigger("Attack");
        Collider[] colliders = Physics.OverlapSphere(attackPoint.position, 1f, playerLayer);
        if(colliders.Length > 0){
            foreach (var collider in colliders)
            {
                if (collider.gameObject.GetComponent<PlayerControl>() != null)
                {
                    collider.gameObject.GetComponent<PlayerControl>().TakeDamage(dano);
                }
            }
        }
        currentState = State.CoolDown;
        cooldownTimer = cooldownTime;
    }

    private void PerformGrabAttack()
    {
        Collider[] colliders = Physics.OverlapSphere(attackPoint.position, 1f, playerLayer);
        if(colliders.Length > 0){
            foreach (var collider in colliders)
            {
                if (collider.gameObject.GetComponent<PlayerControl>() != null)
                {
                    isGrabing = true;
                    collider.gameObject .GetComponent<PlayerControl>().GrabPlayer(GrabPoint);
                    grabbedPlayer = collider.gameObject.GetComponent<PlayerControl>();
                    currentState = State.Grab;
                    grabTimer = grabTime;
                    return; // Saia do loop após agarrar um jogador
                }
            }
        }
        currentState = State.CoolDown;
        cooldownTimer = cooldownTime;
    }

    #endregion

    #region cool down

    private void HandleCoolDown()
    {
        if (grabbedPlayer != null)
        {
            grabbedPlayer = null;
        }
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0)
        {
            currentState = State.Idle;
        }
    }

    private void HandleWaiting()
    {
        rb.velocity = Vector3.zero;
        animator.SetFloat("Speed", 0f);
        Debug.Log("Inimigo esperando!");
        
        if (attackingEnemiesCount < maxAttackingEnemies)
        {
            currentState = State.Idle;
        }
    }

    #endregion

    #region be Grabbed, Be throwed, take demage and get stunned

    private void HandleGrabbed()
    {
        animator.SetFloat("Speed", 0f);
        Debug.Log("Inimigo agarrado!");
        rb.isKinematic = true;
        isGrabbed = true;
        transform.position = player.GetComponent<PlayerController>().attackPoint.position;
    }

    private void HandleFall()
    {
        hitsTaken = 0;
        if (isGrabbed)
        {
            player.GetComponent<PlayerCombat>().isGrabbingSomeone = false;
            player.GetComponent<PlayerCombat>().GrabbedEnemy = null;
            rb.isKinematic = false;
            isGrabbed = false;
        }
        if (!hasFallen)
        {
            hasFallen = true;
            rb.AddForce(facingRight ? (-transform.right) * 7f : (transform.right) * 7f, ForceMode.Impulse);
            //se o inimigo atingir outro inimigo durante esta tragetoria de ser arremessado, o outro inimigo tambem deve levar um dano e tambem deve cair
            //standTimer = Random.Range(3f, 5f);
            standTimer = 3f;
            animator.SetTrigger("Fall");
        }
        else
        {
            standTimer -= Time.deltaTime;
            if (standTimer <= 0)
            {
                currentState = health <= 0 ? State.Die : State.Stand;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (hasFallen && collision.gameObject.CompareTag("Enemy") && rb.velocity != Vector3.zero)
        {
            Debug.Log("colisão entre inimigos");
            Enemy otherEnemy = collision.gameObject.GetComponent<Enemy>();
            if (otherEnemy != null && (otherEnemy.currentState != State.Stunned || otherEnemy.currentState != State.Fall))
            {
                otherEnemy.TakeHit(); // Aplica dano ao inimigo atingido
                otherEnemy.currentState = State.Fall;
            }
        }
    }

    private void HandleStand()
    {
        animator.SetFloat("Speed", 0f);
        rb.velocity = Vector3.zero;
        hasFallen = false;
        currentState = State.Idle;
    }

    private void HandleDie()
    {
        Debug.Log("Inimigo morreu!");
        Destroy(gameObject);
    }

    private void HandleStunned()
    {
        rb.velocity = Vector3.zero;
        animator.SetFloat("Speed", 0f);
        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0)
        {
            currentState = State.Idle;
        }
    }

    public void TakeHit()
    {
        hitsTaken++;
        health--;
        animator.SetTrigger("Hurt");
        if (hitsTaken >= MaxHitBeforeFall || health <= 0)
        {
            currentState = State.Fall;
        }
        else
        {
            currentState = State.Stunned;
            stunTimer = stunDuration;
        }
    }
    #endregion

    #region other

    private void FlipCheck()
    {
        float directionX;
        if(players.Length >1){
            if(!isGrabing ){
                directionX = player.position.x - transform.position.x;
            }else{
                directionX = otherPlayer.position.x - transform.position.x;
            }
        }else{
            directionX = player.position.x - transform.position.x;
        }
        if (directionX > 0 && !facingRight)
        {
            Flip();
        }
        else if (directionX < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private bool CanApproach()
    {
        return attackingEnemiesCount < maxAttackingEnemies && !IsPlayerIncapacitated();
    }

    private bool IsPlayerIncapacitated()
    {
        var playerControl = player.GetComponent<PlayerControl>();
        return isGrabbed || playerControl.falled; // || playerController.isGrabbingSomeone;
    }

    private Vector3 GetTargetPosition()
    {
        Vector3 possibleTargetPosition1 = new Vector3(player.position.x + 1f, transform.position.y, player.position.z);
        Vector3 possibleTargetPosition2 = new Vector3(player.position.x - 1f, transform.position.y, player.position.z);
        return Vector3.Distance(transform.position, possibleTargetPosition1) < Vector3.Distance(transform.position, possibleTargetPosition2) ? possibleTargetPosition1 : possibleTargetPosition2;
    }

    private bool IsInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) < attackRange && transform.position.z == player.position.z;
    }

    private bool CanAttack()
    {
        return attackingEnemiesCount < maxAttackingEnemies && currentState != State.Stunned && !IsPlayerIncapacitated();
    }
    #endregion
}
