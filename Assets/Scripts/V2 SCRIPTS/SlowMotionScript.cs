using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float slowMotionFactor = 0.5f; // Fator de desaceleração
    public float slowMotionDuration = 2f; // Duração do efeito
    private float originalTimeScale = 1f;
    [SerializeField]private bool isSlowed = false;

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.S)) // Pressione 'S' para ativar o slow motion
       // {
           // StartCoroutine(ActivateSlowMotion());
        //}
    }
    public void StartSlowMotion(){
        isSlowed = true;
        Time.timeScale = slowMotionFactor;
    }

    public void StopSlowMotion(){
        Time.timeScale = originalTimeScale;
        isSlowed = false;
    }
}