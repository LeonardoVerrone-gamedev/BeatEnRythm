using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindScript : MonoBehaviour
{
    public float _delay;
    public Component[] toDisable;
    public Animator anim;
    public Rigidbody rb;
    public Transform transform;
    public SpriteRenderer spriteRenderer;

    public float gravity;

    public bool Recording;
    public bool Rewinding;

    public List<Vector3> lastPositions = new List<Vector3>();
    public List<string> lastPlayedAnimations = new List<string>();

    public List<Sprite> currentSprite = new List<Sprite>();

    void Start()
    {
        
    }

    
    void FixedUpdate()
    {
        if(!Rewinding){
            Recording = true;
        }else{
            Recording = false;

            OnRewind2();
        }
    }

    void LateUpdate(){
        if(Recording){
            if(anim != null){
                GetLastAnimation();
            }
            if(transform != null){
                GetLastPosition();
            }
        }
    }

    void GetLastAnimation(){
        //AnimatorStateInfo animState = anim.GetCurrentAnimatorStateInfo(0);
        //lastPlayedAnimations.Insert(0, animState.shortNameHash.ToString());

        currentSprite.Insert(0, spriteRenderer.sprite);

        //deleta um item após 5 segundos (se 60FPS)
        if(currentSprite.Count > 150){
            //lastPlayedAnimations.RemoveAt(lastPlayedAnimations.Count - 1);
            currentSprite.RemoveAt(currentSprite.Count -1);
        }
    }

    void GetLastPosition(){
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        lastPositions.Insert(0, pos);

    //deleta um item após 30 segundos (se 60FPS)
        if(lastPositions.Count > 150){
            lastPositions.RemoveAt(lastPositions.Count - 1);
        }
    }

    public void StartRewind(){
        foreach(Component component in toDisable){
            var enabledProperty = component.GetType().GetProperty("enabled");

            if(enabledProperty != null && enabledProperty.CanWrite){
                enabledProperty.SetValue(component, false);
            }
        }

        if(rb != null){
            rb.isKinematic = true;
        }
        anim.Play("Move");

        Recording = false;
        anim.enabled = false;
        Rewinding = true;
    }

    public void StopRewind(){
        Recording = true;
        //anim.speed = 1f;
        Rewinding = false;

        foreach(Component component in toDisable){
            var enabledProperty = component.GetType().GetProperty("enabled");

            if(enabledProperty != null && enabledProperty.CanWrite){
                enabledProperty.SetValue(component, true);
            }
        }

        if(rb != null){
            rb.isKinematic = false;
        }
        //anim.SetTrigger("");
        anim.enabled = true;
        //anim.speed = 1f;
        //anim.Play(lastPlayedAnimations[0]);
    }

    private void OnRewind2(){
        if(spriteRenderer != null){
            spriteRenderer.sprite = currentSprite[0];
            currentSprite.RemoveAt(0);
        }

        if(transform != null){
            transform.position = lastPositions[0];
            lastPositions.RemoveAt(0);
        }
    }
}
