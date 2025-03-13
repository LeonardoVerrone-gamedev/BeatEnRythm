using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGrabAndTrhowAttackBehaviour : MonoBehaviour
{
    private EnemyMain enemyMain;
    [SerializeField] Animator animator;
    [SerializeField] Transform GrabPoint;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Rigidbody rb;

    private Transform[] players;
    private Transform player;
    private Transform otherPlayer;

    private PlayerControl grabbedPlayer;
    private bool SearchForOtherPlayer;
    bool ThrowDirection_right;

    void Awake()
    {
        enemyMain = GetComponent<EnemyMain>();
    }

    void OnEnable()
    {
        // Código que você deseja executar sempre que o componente for ativado
        grabbedPlayer = null;
        players = enemyMain.players;
        otherPlayer = enemyMain.otherPlayer;
        player = enemyMain.player;
        SearchForOtherPlayer = false;
        StartCoroutine(PerformGrabAttack());
    }

    void OnDisable(){
        StopAllCoroutines();
    }

    void Update(){
        if(SearchForOtherPlayer){
            Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, otherPlayer.position.z);
            rb.MovePosition(Vector3.MoveTowards(transform.position, targetPos, (2.5f) * Time.deltaTime));
            
            if(otherPlayer.position.x < transform.position.x){
                ThrowDirection_right = false;
            }else{
                ThrowDirection_right = true;
            }
        }else{
            ThrowDirection_right = false;
        }
    }


    private IEnumerator PerformGrabAttack()
    {
        Debug.Log("Jogador estava incapacitado?" + IsPlayerIncapacitated());
        if(IsPlayerIncapacitated()){
            enemyMain.ChangeCurrentState(EnemyMain.EnemyState.Idle);
            yield break;
        }
        enemyMain.SetInvulnerable(true);
        animator.SetTrigger("Attack");
        AttackCollision();
        while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f){
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        enemyMain.SetInvulnerable(false);
        if(grabbedPlayer != null){
            StartCoroutine(PerformThrow());
        }else{
            enemyMain.ChangeCurrentState(EnemyMain.EnemyState.Idle);
        }
    }

    private IEnumerator PerformThrow(){
        animator.SetTrigger("Attack");
        grabbedPlayer.ThrowPlayer(ThrowDirection_right);
        while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f){
            yield return null;
        }
        enemyMain.ChangeCurrentState(EnemyMain.EnemyState.Idle);
    }

    public void AttackCollision(){
        Collider[] colliders = Physics.OverlapSphere(GrabPoint.position, 1f, playerLayer);
        if(colliders.Length > 0){
            foreach (var collider in colliders)
            {
                if (collider.gameObject.GetComponent<PlayerControl>() != null)
                {
                    collider.gameObject .GetComponent<PlayerControl>().GrabPlayer(GrabPoint);
                    grabbedPlayer = collider.gameObject.GetComponent<PlayerControl>();
                    if(otherPlayer != null){
                        SearchForOtherPlayer = true;
                    }
                    return; // Saia do loop após agarrar um jogador
                }
            }
        }
    }

    private bool IsPlayerIncapacitated()
    {
        var playerControl = player.gameObject.GetComponent<PlayerControl>();
        return !playerControl.isCapacitated();
    }
}
