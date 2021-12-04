using UnityEngine;

public class DreamRuneLightning : MonoBehaviour
{
    #region Attributes
    [SerializeField] float damage = 10f;
    [SerializeField] float lifeTime = 1f;
    #endregion

    #region  MonoBehaviour Methods
    private void Start() 
    {
        FindObjectOfType<AudioManager>().Play("DreamRuneLightning");

        Destroy(gameObject, lifeTime);    
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.layer.Equals(LayerMask.NameToLayer("EnemyTrigger")) && !other.gameObject.tag.Equals("EnemyAttack"))
        {
            EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
            
            enemyHealth.Hurt(damage, transform.position);
        }

        if(other.gameObject.CompareTag("DestructibleObject"))
        {
            other.GetComponent<ObjectDestructible>().DestroyObject();
        }
    }
    #endregion
}
