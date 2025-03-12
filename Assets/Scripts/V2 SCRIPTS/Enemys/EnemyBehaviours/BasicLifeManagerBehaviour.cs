using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLifeManagerBehaviour : MonoBehaviour
{
    private EnemyMain enemyMain;

    [SerializeField] Animator animator;

    private Transform[] players;
    private Transform player;

    void Awake()
    {
        enemyMain = GetComponent<EnemyMain>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
