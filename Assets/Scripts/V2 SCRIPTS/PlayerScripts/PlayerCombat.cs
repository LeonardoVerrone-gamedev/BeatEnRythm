using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;

    public bool isGrabbingSomeone;
    public Enemy GrabbedEnemy;

    public void PerformAttack()
    {
        if (isGrabbingSomeone)
        {
            Debug.Log("Dano no inimigo agarrado");
            GrabbedEnemy.TakeHit();
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
            if (collider.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.TakeHit();
            }
        }
    }

    public void PerformGrab()
    {
        Collider[] colliders = Physics.OverlapSphere(attackPoint.position, 1f, enemyLayer);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<Enemy>(out var enemy) && !isGrabbingSomeone)
            {
                isGrabbingSomeone = true;
                GrabbedEnemy = enemy;
                enemy.currentState = Enemy.State.Grabbed;
                break; // Saia do loop após agarrar um inimigo
            }
        }
    }
}