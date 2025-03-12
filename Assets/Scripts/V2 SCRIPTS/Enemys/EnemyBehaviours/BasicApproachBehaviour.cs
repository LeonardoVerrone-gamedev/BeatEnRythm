using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicApproachBehaviour : MonoBehaviour
{
    private EnemyMain enemyMain;

    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rb;

    private Transform[] players;
    private Transform player;
    private Transform otherPlayer;

    [SerializeField] float attackRange = 2f;
    [SerializeField] float moveSpeed = 5f;

    bool facingRight = true;

    void Awake(){
        enemyMain = GetComponent<EnemyMain>();
    }
    void Start()
    {
        players = enemyMain.players;
        player = enemyMain.player;
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null){
            HandleApproaching();
            FlipCheck();
        }
    }

    private void HandleApproaching()
    {
        Vector3 targetPosition = GetTargetPosition();
        rb.MovePosition(Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime));
        animator.SetFloat("Speed", 1f);

        if (IsInAttackRange())
        {
            enemyMain.ChangeCurrentState(EnemyMain.EnemyState.Attack);
        }
    }

    private Vector3 GetTargetPosition()
    {
        Vector3 possibleTargetPosition1 = new Vector3(player.position.x + 1f, transform.position.y, player.position.z);
        Vector3 possibleTargetPosition2 = new Vector3(player.position.x - 1f, transform.position.y, player.position.z);
        return Vector3.Distance(transform.position, possibleTargetPosition1) < Vector3.Distance(transform.position, possibleTargetPosition2) ? possibleTargetPosition1 : possibleTargetPosition2;
    }

    private bool IsInAttackRange()
    {
        float zDifference = Mathf.Abs(transform.position.z - player.position.z);
        return Vector3.Distance(transform.position, player.position) < attackRange && zDifference <= 0.5f;
    }

    private void FlipCheck()
    {
        
        float directionX = player.position.x - transform.position.x;
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
}
