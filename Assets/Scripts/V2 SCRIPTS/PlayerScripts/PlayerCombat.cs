using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;

    public bool isGrabbingSomeone;
    public BasicLifeManagerBehaviour GrabbedEnemy;

    public void PerformAttack()
    {
        if (isGrabbingSomeone)
        {
            Debug.Log("Dano no inimigo agarrado");
            GrabbedEnemy._TakeHit();
        }
        else
        {
            ColliderArea();
        }
    }

    private void ColliderArea()
    {
        Collider[] colliders = Physics.OverlapSphere(attackPoint.position, 1f, enemyLayer);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<BasicLifeManagerBehaviour>(out var enemy))
            {
                enemy._TakeHit();
            }
        }
    }

    public void PerformGrab()
    {
        Collider[] colliders = Physics.OverlapSphere(attackPoint.position, 1f, enemyLayer);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<BasicLifeManagerBehaviour>(out var enemy) && !isGrabbingSomeone)
            {
                isGrabbingSomeone = true;
                GrabbedEnemy = enemy;
                enemy.beGrabbed(attackPoint, this);
                break; // Saia do loop após agarrar um inimigo
            }
        }
    }

    public void Soltar(){
        isGrabbingSomeone = false;
        GrabbedEnemy = null;
    }
}