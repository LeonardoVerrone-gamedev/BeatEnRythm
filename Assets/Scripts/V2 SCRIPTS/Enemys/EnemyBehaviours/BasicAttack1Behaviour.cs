using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack1Behaviour : MonoBehaviour
{
    private EnemyMain enemyMain;

    [SerializeField] Transform attackPoint;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float dano;

    [SerializeField]Animator animator;

    void Awake()
    {
        enemyMain = GetComponent<EnemyMain>();
    }

    void OnEnable()
    {
        // Código que você deseja executar sempre que o componente for ativado
        StartCoroutine(PerformNormalAttack());
    }

    void OnDisable(){
        StopAllCoroutines();
    }


    private IEnumerator PerformNormalAttack()
    {
        enemyMain.SetInvulnerable(true);
        animator.SetTrigger("Attack");
        while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f){
            yield return null;
        }
        enemyMain.SetInvulnerable(false);
        StartCoroutine(returnToIdle());
    }

    private IEnumerator returnToIdle(){
        yield return new WaitForSeconds(1f);
        enemyMain.ChangeCurrentState(EnemyMain.EnemyState.Idle);
    }

    public void AttackCollision(){
        Collider[] colliders = Physics.OverlapSphere(attackPoint.position, 1f, playerLayer);
        if(colliders.Length > 0){
            foreach (var collider in colliders)
            {
                if (collider.gameObject.GetComponent<PlayerControl>() != null)
                {
                    collider.gameObject.GetComponent<PlayerControl>().TakeDamage(dano * enemyMain.players.Length);
                }
            }
        }
    }
}
