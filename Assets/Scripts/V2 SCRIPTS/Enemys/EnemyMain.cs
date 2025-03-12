using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMain : MonoBehaviour
{
    #region EnemyState variables
    public enum EnemyState{Idle, Approach, Attack, Hurt, Die}
    [SerializeField] private EnemyState currentState;

    [SerializeField]Component IdleComponent;
    [SerializeField]Component ApproachComponent;
    [SerializeField]Component[] AttackComponents;
    [SerializeField]Component LifeManagerComponent; //cuidado pra desativar esse, so em momentos de invulnerabilidade

    Behaviour idleBehaviour;
    Behaviour approachBehaviour;
    List <Behaviour> attackBehaviours = new List<Behaviour>();
    Behaviour lifeBehaviour;
    #endregion

    #region player variables
    public Transform[] players;
    public Transform player;
    public Transform otherPlayer;
    #endregion

    #region EnemyStateControl

    void Awake(){
        GameObject[] _players = GameObject.FindGameObjectsWithTag("Player");
        players = new Transform[_players.Length];
        for (int i = 0; i < _players.Length; i++)
        {
            players[i] = _players[i].transform;
        }
    }
    void Start()
    {
        idleBehaviour = IdleComponent as Behaviour;
        approachBehaviour = ApproachComponent as Behaviour;
        lifeBehaviour = LifeManagerComponent as Behaviour;
        
        foreach (Component attack in AttackComponents){
            attackBehaviours.Add(attack as Behaviour);
        }

        ChangeCurrentState(EnemyState.Idle);
    }


    public void ChangeCurrentState(EnemyState newCurrentState){
        currentState = newCurrentState;

        switch (currentState)
        {
            case EnemyState.Idle:
                SwitchToIdleBehaviour();
                break;
            case EnemyState.Approach:
                SwitchToApproachBehaviour();
                break;
            case EnemyState.Attack:
                SwitchToAttackBehaviour();
                break;
        }
    }

    void SwitchToIdleBehaviour(){
        idleBehaviour.enabled = true;

        approachBehaviour.enabled = false;
        foreach(Behaviour attack in attackBehaviours){
            attack.enabled = false;
        }
    }

    void SwitchToApproachBehaviour(){
        approachBehaviour.enabled = true;

        idleBehaviour.enabled = false;
        foreach(Behaviour attack in attackBehaviours){
            attack.enabled = false;
        }
    }

    void SwitchToAttackBehaviour(){
        Behaviour thisAttack = attackBehaviours[Random.Range(0, attackBehaviours.Count)];
        thisAttack.enabled = true;

        foreach(Behaviour attack in attackBehaviours){
            if(attack != thisAttack){
                attack.enabled = false;
            }
        }

        approachBehaviour.enabled = false;
        idleBehaviour.enabled = false;
    }

    public void SetInvulnerable(bool invulnerable){
        if(invulnerable){
            lifeBehaviour.enabled = false;
        }else{
            lifeBehaviour.enabled = true;
        }
    }
    #endregion
}
