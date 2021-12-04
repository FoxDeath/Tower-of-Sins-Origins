using UnityEngine;

[RequireComponent(typeof(AstarAI))]
[RequireComponent(typeof(EnemyState))]
public class EnemyHealth : MonoBehaviour
{
    #region  Attributes
    private dynamic astarAI;

    private dynamic enemyState;

    private EnemyCombat enemyCombat;

    private AudioManager audioManager;

    private UIController uiController;

    [SerializeField] float maxHealth = 100f;
    private float health;

    private GameObject triggerColliderObject;
    [SerializeField] GameObject bloodExplosionObject;

    [SerializeField] ParticleSystem bloodHit;
    #endregion

    #region  Monobehaviour Methods
    private void Awake()
    {
        astarAI = GetComponent<AstarAI>();

        enemyState = GetComponent<EnemyState>();

        enemyCombat = GetComponent<EnemyCombat>();

        audioManager = FindObjectOfType<AudioManager>();

        uiController = FindObjectOfType<UIController>();

        triggerColliderObject = transform.Find("TriggerCollider").gameObject;
    }

    private void Start()
    {
        health = maxHealth;

        if(astarAI.GetIsBoss() && uiController != null)
        {
            uiController.SetBossEnergyBarState(true);

            uiController.SetBossEnergy(health / maxHealth);
        }
    }
    #endregion

    #region Normal Methods
    //Retracts health and kills the enemy if it has no hp left.
    public void Hurt(float damage, Vector3 playerPosition, bool attackFromAbove = false, bool startOfCombo = true)
    {
        if(!startOfCombo && !attackFromAbove)
        {
            if(!astarAI.GetIsBiggerThanPlayer())
            {
                StartCoroutine(astarAI.KnockedBackBehaviour());
            }
            else
            {
                StartCoroutine(astarAI.StaggeredBehaviour());
            }
        }
        

        health -= damage;

        uiController.SetBossEnergy(health / maxHealth);

        if(health <= 0f)
        {
            Die();
        }

        PlayerHitParticles(playerPosition);

        audioManager.Play("EnemyHurt");
    }

    //Takes care of the enemy's death.
    public void Die()
    {
        enemyState.SetState(EnemyState.State.Dead);

        audioManager.Play("Die");
        
        triggerColliderObject.SetActive(false);

        GameObject bloodExplosionInstantiatedObject = Instantiate(bloodExplosionObject, transform.position, Quaternion.identity);

        Destroy(bloodExplosionInstantiatedObject, 3f);

        uiController.SetBossEnergyBarState(false);

        Destroy(gameObject, 2f);
    }

    private void PlayerHitParticles(Vector2 playerPosition)
    {
        bloodHit.transform.rotation = Quaternion.LookRotation(playerPosition = bloodHit.transform.position);

        bloodHit.Play();
    }

    #region Getters and Setters
    public float GetCurrentHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealthPercentage()
    {
        return health / maxHealth * 100f;
    }

    public void SetMaxHealth(float maxHealth)
    {
        if(this.maxHealth != maxHealth)
        {
            this.maxHealth = maxHealth;

            if(maxHealth < health)
            {
                health = maxHealth;
            }

            if(health <= 0f)
            {
                Die();
            }
        }
    }
    #endregion
    #endregion
}
