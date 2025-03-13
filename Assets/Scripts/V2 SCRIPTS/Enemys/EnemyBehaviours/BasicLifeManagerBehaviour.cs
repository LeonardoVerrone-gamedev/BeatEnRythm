using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLifeManagerBehaviour : MonoBehaviour
{
    private EnemyMain enemyMain;

    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rb;
    
    [SerializeField] int health;

    [SerializeField] Transform attackPoint;

    [SerializeField] Transform _grabPoint;

    private Transform[] players;
    private Transform player;

    private PlayerCombat grabbing;

    [SerializeField]private bool isGrabbed;
    bool facingRight;

    [SerializeField] int hitsTaken = 0;
    [SerializeField] int MaxHitBeforeFall;

    void Awake()
    {
        enemyMain = GetComponent<EnemyMain>();
    }

    void Update()
    {
        if(isGrabbed && _grabPoint != null){
            transform.position = _grabPoint.position;
        }
    }

    public void beGrabbed(Transform grabPoint, PlayerCombat _playerCombat){
        isGrabbed = true;
        enemyMain.ChangeCurrentState(EnemyMain.EnemyState.Hurt);
        //animator.SetTrigger("Grabbed");
        rb.isKinematic = true;
        _grabPoint = grabPoint;
        grabbing = _playerCombat;
    }

    public void _TakeHit(){
        StartCoroutine(TakeHit());
    }

    public IEnumerator TakeHit(){
        enemyMain.ChangeCurrentState(EnemyMain.EnemyState.Hurt);
        hitsTaken++;

        if(isGrabbed){
            animator.SetTrigger("Hurt"); //animação de levar dano estando agarrado
        }else{
            animator.SetTrigger("Hurt"); //animação de levar dano de pé
        }
        health--;
        while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f){
            yield return null;
        }
        if(hitsTaken > MaxHitBeforeFall || health <= 0){
            hitsTaken = 0;
            StartCoroutine(Fall());
        }else{
            if(!isGrabbed){
                enemyMain.ChangeCurrentState(EnemyMain.EnemyState.Idle);
            }
        }
    }


    private IEnumerator Fall(){
        hitsTaken = 0;
        if (isGrabbed)
        {
            grabbing.Soltar();
            rb.isKinematic = false;
            isGrabbed = false;
        }
        animator.SetTrigger("Fall");
        CheckDirection();
        rb.AddForce(facingRight ? (-transform.right) * 7f : (transform.right) * 7f, ForceMode.Impulse);
        while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f){
            yield return null;
        }
        if(health <= 0){
            Die();
        }else{
            if(!isGrabbed){
                enemyMain.ChangeCurrentState(EnemyMain.EnemyState.Idle);
            }
        }
    }

    void Die(){
        //animação de morte
        //while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f){
            //yield return null;
        //}

        //soma pontos aos jogadores
        Destroy(this.gameObject);
    }

    void CheckDirection(){
        if(attackPoint.position.x < transform.position.x && facingRight){
            //Flip
            facingRight = false;
        }else{
            //Flip
            facingRight = true;
        }
    }
}
