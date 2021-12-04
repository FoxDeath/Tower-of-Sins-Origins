using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
//todo: SetCollisionPosition(Vector2 collisionPos) should get the contact point of the collision, not the pos of the whole enemy collider
public class PlayerHealth : MonoBehaviour
{
    #region  Attributes
    [SerializeField] GameObject loadDeathScreen;
    
    [SerializeField] Object hubSceneObject;

    private SpriteRenderer spriteRenderer;

    private PlayerCombat playerCombat;

    private PlayerStats playerStats;

    private UIController uIController;

    private Vector2 collisionPos;
    
    private float knockBackStrenght;
    private float recoveryTime;
    private float maxEnergyPoints;
    private float energyPoints;

    [Header("Knockback Stats")]
    [SerializeField] float xKnockbackMultiplier = 3f;
    [SerializeField] float yKnockbackMultiplier = 5f;

    [Header("Regeneration Stats")]
    [Tooltip("How fast the regen ammount goes down")]
    [SerializeField] float regenDecreaseMultiplier = 1;
    
    [Tooltip("How much the regeneration is split")]
    [SerializeField] int regenIncrement;
    [Tooltip("How many health increments you regenerate when the enemy dies due to the envirovment")]
    [SerializeField] int environmentalDeathRegenMultiplier = 1;

    //How much you regenerate per hit after the first hit (maxEnergyPoints / regenIncrement) * regenMultiplier which comes from the combo attacks)
    private float regenAmount;
    
    //If true the regeneration decreeses every frame, if false it doesn't
    private bool decreaseRegen;
    #endregion

    #region  MonoBehaviour Methods
    private void Awake()
    {
        playerCombat = GetComponent<PlayerCombat>();

        playerStats = GetComponent<PlayerStats>();

        uIController = GetComponentInChildren<UIController>();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        SetPlayerStats();

        energyPoints = maxEnergyPoints;
    }

    private void Update() 
    {
        if(decreaseRegen && regenAmount > 0f)
        {
            regenAmount -= Time.deltaTime * regenDecreaseMultiplier;
        }

        uIController.SetRegenAmmount(energyPoints/maxEnergyPoints + regenAmount/maxEnergyPoints);
    }
    #endregion

    #region  Normal Methods
    public void Hurt(float damage, bool isKnockedBack = true, bool isStaggering = true)
    {
        if(PlayerState.GetIsRecovering())
        {
            return;
        }

        playerCombat.StopCombo(true);

        SetEnergyPoints(energyPoints - damage);
        
        if(energyPoints > 0f)
        {
            if(PlayerState.GetState() != PlayerState.State.Staggered && isStaggering)
            {
                StartCoroutine(StaggerBehaviour(isKnockedBack));
            }
        }
        else
        {
            PlayerState.SetState(PlayerState.State.Dead);
        }
    }

    public void SuccessfulAttack(Vector2 collisionPossition, int attackDirection, int regenMultiplier, bool isWall = false)
    {
        SetCollisionPosition(collisionPossition);

        if(attackDirection == -1 && !isWall)
        {
            Knockback(0f, yKnockbackMultiplier);
        }
        else if(attackDirection == 0 && isWall)
        {
            Knockback(xKnockbackMultiplier, 0f);

            return;
        }

        if(regenAmount > 0)
        {
            Regenerate();
        }

        CalculateRegenAmmount(regenMultiplier);
    }

    private void CalculateRegenAmmount(int regenMultiplier)
    {
        regenAmount = (((int)maxEnergyPoints / regenIncrement) * regenMultiplier);

        if(regenAmount + energyPoints > maxEnergyPoints)
        {
            regenAmount = maxEnergyPoints - energyPoints;
        }

        decreaseRegen = false;

        if(regenAmount > 0)
        {
            StopCoroutine(RegenDecreaseCooldown());

            StartCoroutine(RegenDecreaseCooldown());
        }
    }

    public void Regenerate(bool isParry = false, bool isEnvirovment = false)
    {
        float regenAmount = this.regenAmount;

        if(isParry)
        {
            regenAmount = (int)maxEnergyPoints / regenIncrement;
        }

        if(isEnvirovment)
        {
            regenAmount = (int)maxEnergyPoints / regenIncrement * environmentalDeathRegenMultiplier;
        }

        if(energyPoints + regenAmount < maxEnergyPoints)
        {
            SetEnergyPoints(energyPoints + regenAmount);
        }
        else
        {
            SetEnergyPoints(maxEnergyPoints);
        }
    }

    public void Knockback(float xMultiplier = 1f, float yMultiplier = 1f)
    {
        if(collisionPos.x - transform.position.x < 0f)
        {
            StartCoroutine(KnockbackBehaviour(xMultiplier, yMultiplier, 1));
        }
        else if(collisionPos.x - transform.position.x > 0f)
        {
            StartCoroutine(KnockbackBehaviour(xMultiplier, yMultiplier, -1));
        }
    }

    public void OnDeath()
    {
        LoadDeathMenu();
    }

    public void LoadDeathMenu()
    {
        loadDeathScreen.SetActive(true);
    }

    public void LoadHub()
    {
        SceneManager.LoadScene(hubSceneObject.name);
    }

    public float GetEnergyPoints()
    {
        return energyPoints;
    }

    public void SetCollisionPosition(Vector2 collisionPos)
    {
        if(!PlayerState.GetIsRecovering())
        {
            this.collisionPos = collisionPos;
        }
    }

    private void SetEnergyPoints(float energyPoints)
    {
        this.energyPoints = energyPoints;

        uIController.SetEnergy(energyPoints/maxEnergyPoints);
    }

    public void SetPlayerStats()
    {
        knockBackStrenght = playerStats.GetKnockBackStrenght();

        recoveryTime = playerStats.GetRecoveryTime();

        maxEnergyPoints = playerStats.GetMaxEnergyPoints();
    }
    #endregion

    #region  Coroutines
    private IEnumerator KnockbackBehaviour(float xMultiplier, float yMultiplier, int direction)
    {
        PlayerState.SetIsKnockedBack(true);

        PlayerMovement.AddVelocity(new Vector2(knockBackStrenght * xMultiplier * direction, knockBackStrenght * yMultiplier));

        yield return new WaitForSeconds(0.2f);

        PlayerState.SetIsKnockedBack(false);

        PlayerMovement.StopJumping();
    }

    private IEnumerator StaggerBehaviour(bool isKnockedBack)
    {
        PlayerInput.SetIsRecievingInput(false);

        PlayerMovement.StopMoving(true);

        PlayerState.SetState(PlayerState.State.Staggered);

        if(isKnockedBack)
        {
            Knockback();
        }

        StartCoroutine(RecoveryBehaviour());

        yield return new WaitForSeconds(0.3f);

        PlayerInput.SetIsRecievingInput(true);

        PlayerState.SetState(PlayerState.State.Idle);
    }

    private IEnumerator RecoveryBehaviour()
    {
        PlayerState.SetIsRecovering(true);

        StartCoroutine(SpriteBlinkBehaviour());

        yield return new WaitForSeconds(recoveryTime);

        PlayerState.SetIsRecovering(false);
    }

    private IEnumerator SpriteBlinkBehaviour()
    {
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds (0.1f);

        spriteRenderer.enabled = true;

        yield return new WaitForSeconds (0.1f);

        if(PlayerState.GetIsRecovering())
        {
            StartCoroutine(SpriteBlinkBehaviour());
        }
    }

    private IEnumerator RegenDecreaseCooldown()
    {
        yield return new WaitForSeconds(1);

        decreaseRegen = true;
    }
    #endregion
}