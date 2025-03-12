using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManagement : MonoBehaviour
{
    public Enemy[] enemies;
    public int attackingEnemies;

    void Update()
    {
        ChecaInimigosAtacando();
    }

    void ChecaInimigosAtacando(){
        attackingEnemies = 0;
        enemies = FindObjectsOfType<Enemy>();

        foreach(Enemy enemy in enemies){
            if(enemy.currentState == Enemy.State.Attack){
                attackingEnemies++;
            }
            enemy.attackingEnemiesCount = attackingEnemies;
        }
    }
}
