using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponController : MonoBehaviour
{
    #region Attributes
    private PlayerHealth playerHealth;

    private EnemyCombat enemyCombat;
    
    private EnemyState enemyState;

    private float damage;

    [Tooltip("If the sprite of the attack should stay on screen after it collides with the player.")]
    [SerializeField] bool shouldPersistAfterCollision = false;
    private bool isProjectile = false;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        enemyCombat = GetComponentInParent<EnemyCombat>();

        enemyState = GetComponentInParent<EnemyState>();
        
        if(GameObject.FindGameObjectWithTag("Player"))
        {
            playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if(playerHealth == null && GameObject.FindGameObjectWithTag("Player"))
        {
            playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //If the enemy's weapon hits the player, it runs the on trigger behaviour.
        if(other.gameObject.layer.Equals(LayerMask.NameToLayer("Player")) && !other.gameObject.tag.Equals("Attack"))
        {
            StartCoroutine(OnTriggerBehaviour(other));
        }

        //Enemies are able to destroy objects in the environment.
        if(other.gameObject.CompareTag("DestructibleObject"))
        {
            other.GetComponent<ObjectDestructible>().DestroyObject();
        }
    }
    #endregion

    #region Normal Methods
    public void ProjectileSetUp(EnemyCombat enemyCombat)
    {
        isProjectile = true;

        this.enemyCombat = enemyCombat;
    }
    
    public void ProjectileSetUp(float damage)
    {
        isProjectile = true;

        this.damage = damage;
    }
    #endregion

    #region Coroutines
    //placing the whole behaviour in a coroutine allowed us to check the collision on the player's side first.
    private IEnumerator OnTriggerBehaviour(Collider2D other)
    {
        //It waits for the next fixed update, so the player's trigger check can run before this.
        yield return new WaitForFixedUpdate();

        if((enemyState != null && enemyState.GetState() != EnemyState.State.KnockedBack
        && enemyState.GetState() != EnemyState.State.Staggered && enemyState.GetCanDealDamage())
        || isProjectile)
        {
            playerHealth.SetCollisionPosition(transform.position);

            if(enemyCombat != null)
            {
                playerHealth.Hurt(enemyCombat.GetCurrentDamage());
            }
            else
            {
                playerHealth.Hurt(damage);
            }

            if(!shouldPersistAfterCollision)
            {
                gameObject.SetActive(false);
            }
            else
            {
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }
    #endregion
}
