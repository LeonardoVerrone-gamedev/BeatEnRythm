using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public float followSpeed;

    public CinemachineVirtualCamera playerFollowCam;

    private GameObject[] players;
    public Transform followTarget;

    public bool isInFollowCamera;

    void Start()
    {
        playerFollowCam = GameObject.FindWithTag("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        //players = GameObject.FindGameObjectsWithTag("Player"); // Inicializa o array de jogadores

    }

    public void SetPlayersArray(){
        players = GameObject.FindGameObjectsWithTag("Player"); // Inicializa o array de jogadores
    }

    void Update()
    {
        if (players.Length > 1 && isInFollowCamera){
            GameObject rightMost = players[0];
            float maxX = rightMost.transform.position.x;

            foreach (GameObject player in players)
            {
                float playerX = player.transform.position.x;
                if (playerX > maxX)
                {
                    maxX = playerX;
                    rightMost = player;
                }
            }

            // Interpola a posição para rightMost
            float targetX = rightMost.transform.position.x; // Corrigido para rightMost
            float newX = Mathf.Lerp(followTarget.position.x, targetX, followSpeed * Time.deltaTime);
            followTarget.position = new Vector3(newX, followTarget.position.y, followTarget.position.z);
        }
        else
        {
            float targetX = players[0].transform.position.x; // Corrigido para rightMost
            float newX = Mathf.Lerp(followTarget.position.x, targetX, followSpeed * Time.deltaTime);
            followTarget.position = new Vector3(newX, followTarget.position.y, followTarget.position.z);
        }
    }
}
