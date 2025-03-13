using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicIdleBehaviour : MonoBehaviour
{
    private EnemyMain enemyMain;
    [SerializeField] Animator animator;

    private Transform[] players;
    private Transform player;
    private Transform otherPlayer;

    void Awake()
    {
        enemyMain = GetComponent<EnemyMain>();
        players = enemyMain.players;
        otherPlayer = enemyMain.otherPlayer;
        player = enemyMain.player;
    }

    void OnEnable(){
        SetTarget();
        StartCoroutine(Idle());
    }

    private IEnumerator Idle(){
        animator.SetFloat("Speed", 0f);
        yield return new WaitForSeconds(Random.Range(2f, 3f));
        if(enemyMain.player != null){
            if (canApproach())
            {
                enemyMain.ChangeCurrentState(EnemyMain.EnemyState.Approach);
            }
        }else{
            StartCoroutine(TrySetTarget());
            yield break;
        }
    }

    IEnumerator TrySetTarget() {
        while (true) {
            SetTarget();
            if (enemyMain.player != null) {
                // Se um jogador foi encontrado, pare a Coroutine
                StartCoroutine(Idle());
                yield break; // Isso sai da Coroutine
            }
            yield return new WaitForSeconds(1f); // Espera 1 segundo antes de tentar novamente
        }
    }

    void SetTarget() {
        if (enemyMain.players.Length > 1) {
            if (Vector3.Distance(enemyMain.players[0].position, transform.position) < Vector3.Distance(enemyMain.players[1].position, transform.position)) {
                enemyMain.player = enemyMain.players[0];
                enemyMain.otherPlayer = enemyMain.players[1];
            } else {
                enemyMain.player = enemyMain.players[1];
                enemyMain.otherPlayer = enemyMain.players[0];
            }
        } else if (enemyMain.players.Length == 1) {
            enemyMain.player = enemyMain.players[0];
            enemyMain.otherPlayer = null; // Não há outro jogador
        } else {
            // Se não houver jogadores, não fazemos nada e tentamos novamente na próxima iteração
        }
    }

    public bool canApproach(){
        var playerControl = enemyMain.player.GetComponent<PlayerControl>();
        return playerControl.isCapacitated();
    }
}
