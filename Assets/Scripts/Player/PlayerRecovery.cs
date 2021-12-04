using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerRecovery : MonoBehaviour
{
    #region  Attributes
    private GameObject currentRecoveryPoint;

    private PlayerHealth playerHealth;

    [SerializeField] float recoveryDamage = 10f;
    #endregion

    #region  MonoBehaviour Methods
    private void Awake() 
    {
        playerHealth = GetComponent<PlayerHealth>();    
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("RecoveryPoint"))
        {
            currentRecoveryPoint = other.gameObject;
        }
    }
    #endregion

    #region  Normal Methods
    public void Recover()
    {
        playerHealth.Hurt(recoveryDamage, false);

        gameObject.transform.position = currentRecoveryPoint.transform.position;
    }
    #endregion
}
