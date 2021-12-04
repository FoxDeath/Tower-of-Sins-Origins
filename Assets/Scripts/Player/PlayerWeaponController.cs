using UnityEngine;
public class PlayerWeaponController : MonoBehaviour
{
    #region Attributes
    private PlayerCombat playerCombat;

    [SerializeField] LayerMask layerMask;
    #endregion
    
    #region MonoBehaviour Methods
    private void Awake() 
    {
        playerCombat = GetComponentInParent<PlayerCombat>();    
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer.Equals(LayerMask.NameToLayer("EnemyTrigger")))
        {
            AstarAI astarAI = other.GetComponentInParent<AstarAI>();
            
            EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
            
            if(PlayerState.GetState() == PlayerState.State.Parrying)
            {
                if(astarAI.Parried())
                {
                    playerCombat.SuccessfulParry();

                    enemyHealth.Hurt(playerCombat.GetDamage(), transform.position);
                }

                return;
            }

            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.parent.position, (other.transform.position - transform.parent.position).normalized,
            Vector3.Distance(other.transform.position , transform.parent.position) , layerMask);

            if(!raycastHit2D.collider && !other.gameObject.tag.Equals("EnemyAttack"))
            {
                bool attackFromAbove = false;

                bool startOfCombo = false;

                enemyHealth.Hurt(playerCombat.GetDamage(), transform.position, attackFromAbove, startOfCombo);
            }
        }

        if(PlayerState.GetState() == PlayerState.State.Sliding || PlayerState.GetState() == PlayerState.State.Dashing || PlayerState.GetState() == PlayerState.State.Attacking)
        {
            //To Do: Add sound for hitting wall
            if(other.gameObject.tag.Equals("Wall"))
            {
                playerCombat.SuccessfulAttack(other.ClosestPoint(transform.position), true);
            }

            if(other.gameObject.CompareTag("DestructibleObject") && !gameObject.CompareTag("Parry"))
            {
                other.GetComponent<ObjectDestructible>().DestroyObject();
            }
        }
    }
    #endregion
}
