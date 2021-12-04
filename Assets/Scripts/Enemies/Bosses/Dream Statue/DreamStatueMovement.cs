using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyCombat))]
public class DreamStatueMovement : AstarAI
{
    #region Attributes
    private Vector2 currentDestination;

    private Transform groundDetection;

    [SerializeField] GameObject aoeAttack;

    private float phaseTwoTriggerHealth;
    private float currentTimeBetweenCharges;
    private float chargesLeft;
    [SerializeField] float phaseOneTimeBetweenCharges = 5f;
    [SerializeField] float phaseTwoTimeBetweenCharges = 3f;
    [SerializeField] float phaseOneChargeFrequency = 3f;
    [SerializeField] float phaseTwoChargeFrequency = 1.5f;
    [SerializeField] float phaseOneChargeTelegraphDuration = 0.4f;
    [SerializeField] float phaseTwoChargeTelegraphDuration = 0.2f;
    [Tooltip("The duration the boss will be levitating before teleporting behind the player.")]
    [SerializeField] float teleportBuildUpTime = 0.5f;

    private int currentMaxCharges;
    [Tooltip("The number of charges the boss will chain together in one go in the first phase.")]
    [SerializeField] int phaseOneMaxCharges = 2;
    [Tooltip("The number of charges the boss will chain together in one go in the second phase.")]
    [SerializeField] int phaseTwoMaxCharges = 4;

    private bool isInPhaseTwo = false;
    private bool isInChargeCycle = false;
    #endregion

    #region MonoBehaviour Methods
    private new void Awake()
    {
        base.Awake();

        phaseTwoTriggerHealth = enemyCombat.GetPhaseTriggerHealth(1);
        
        groundDetection = transform.Find("Sprite").Find("GroundDetection");
    }

    private new void Start()
    {
        base.Start();

        isBoss = true;

        currentMaxCharges = phaseOneMaxCharges;

        chargesLeft = currentMaxCharges;

        currentTimeBetweenCharges = phaseOneTimeBetweenCharges;

        chargeFrequency = phaseOneChargeFrequency;

        chargeTelegraphDuration = phaseOneChargeTelegraphDuration;

        isChargeParriable = false;
    }

    private new void Update()
    {
        base.Update();

        //Takes care of phase switching when apllicable.
        if(enemyHealth.GetHealthPercentage() <= phaseTwoTriggerHealth && !isInPhaseTwo)
        {
            isInPhaseTwo = true;

            currentMaxCharges = phaseTwoMaxCharges;

            currentTimeBetweenCharges = phaseTwoTimeBetweenCharges;

            chargeFrequency = phaseTwoChargeFrequency;

            chargesLeft = currentMaxCharges;

            chargeTelegraphDuration = phaseTwoChargeTelegraphDuration;
            
            if(enemyState.GetState() == EnemyState.State.Charging)
            {
                chargesLeft = 0f;

                StartCoroutine(ChargeEndBehaviour());
            }
        }
    }
    
    private new void FixedUpdate()
    {
        base.FixedUpdate();
        
        GroundedCheck();

        if(enemyState.GetState() != EnemyState.State.Charging && enemyState.GetState() != EnemyState.State.Attacking)
        {
            FlipCheck();
        }

        if(enemyState.GetState() == EnemyState.State.Charging)
        {
            EnvironmentalChargeInterruptionCheck(() => StartCoroutine(ChargeEndBehaviour()));

            if(chargesLeft % 1 == 0)
            {
                if((isSpriteFacingRight && currentDestination.x - myRigidbody.position.x < 0f)
                || (!isSpriteFacingRight && currentDestination.x - myRigidbody.position.x > 0f))
                {
                    StartCoroutine(ChargeEndBehaviour());
                }
            }
        }

        if(Physics2D.CircleCast(new Vector2(myRigidbody.position.x, myRigidbody.position.y + myTriggerCollider.bounds.extents.y),
        myTriggerCollider.bounds.extents.y, Vector2.up, 15f, 1 << LayerMask.NameToLayer("Player")) && !isCurrentlyCheckingForCheese && Mathf.Abs(GetVelocity().x) < 0.1f
        && enemyState.GetState() != EnemyState.State.Charging && enemyState.GetState() != EnemyState.State.Attacking)
        {
            isCurrentlyCheckingForCheese = true;

            StartCoroutine(CheeseFromAboveCheckBehaviour(ResetChargeSequence));
        }

        SpeedControl();
    }

    private new void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        //Cuts the charging behaviour short if the boss hits the player.
        if(other.gameObject.layer.Equals(LayerMask.NameToLayer("Player")) && enemyState.GetState() == EnemyState.State.Charging
        && !other.CompareTag("Attack") && !other.CompareTag("Parry"))
        {
            StartCoroutine(ChargeEndBehaviour());
        }
    }
    #endregion

    #region Normal Methods
    protected override void Move()
    {
        base.Move();

        if(currentDirection == null || enemyState.GetState() == EnemyState.State.Attacking)
        {
            return;
        }

        //As soon as the boss is allowed to charge, it will start the charge behaviour.
        if(enemyState.GetCanCharge() && chargeCoroutine == null)
        {
            chargeCoroutine = StartCoroutine(ChargeBehaviour());
        }
        //While the boss is not in a charge cycle and isn't levitating, the default move behaviour is active.
        else if(!isInChargeCycle&& !isInPhaseTwo)
        {
            FollowMoveBehaviour();
        }
    }
    
    protected override void Idle()
    {
        base.Idle();
    }

    //Moves the boss if it's on the ground and it isn't inside the player.
    private void FollowMoveBehaviour()
    {
        if(((Mathf.Abs(myRigidbody.position.y - player.position.y) - myTriggerCollider.bounds.extents.y - targetExtents.y > 0f
        && Mathf.Abs(myRigidbody.position.x - player.position.x) - myTriggerCollider.bounds.extents.x - targetExtents.x > 0f)
        || Mathf.Abs(myRigidbody.position.y - player.position.y) - myTriggerCollider.bounds.extents.y - targetExtents.y <= 0f)
        && enemyState.GetIsGrounded())
        {
            ApplyVelocity(new Vector2(currentDirection.Value.x, 0f));

            enemyState.SetState(EnemyState.State.Following);
        }
    }

    //This is the only place where the boss' speed is determined for each state.
    protected override void SpeedControl()
    {
        switch(enemyState.GetState())
        {
            case EnemyState.State.Following:
                ClampVelocityAxes(speed, maxVerticalSpeed);
            break;

            case EnemyState.State.Falling:
                ClampVelocityAxes(speed, maxVerticalSpeed);
            break;

            case EnemyState.State.Charging:
                ApplyVelocity(commitedDirection);

                ClampVelocityMagnitude(unparriableChargeSpeed);
            break;

            case EnemyState.State.Teleporting:
                ClampVelocityMagnitude(0f);
            break;

            case EnemyState.State.Attacking:
                ClampVelocityMagnitude(0f);
            break;

            case EnemyState.State.Stationary:
                ClampVelocityAxes(0f, maxVerticalSpeed);
            break;

            case EnemyState.State.Dead:
                ClampVelocityAxes(0f, maxVerticalSpeed);
            break;

            default:
            break;
        }
    }

    //Check if the boss is on the ground and sets the state to stationary if it's not moving.
    private void GroundedCheck()
    {
        if(Physics2D.Raycast(groundDetection.position, -transform.up, 0.3f, 1 << LayerMask.NameToLayer("Ground")))
        {
            enemyState.SetIsGrounded(true);

            if(enemyState.GetState() != EnemyState.State.Charging && enemyState.GetState() != EnemyState.State.Following
            && enemyState.GetState() != EnemyState.State.Teleporting && enemyState.GetState() != EnemyState.State.Attacking)
            {
                enemyState.SetState(EnemyState.State.Stationary);
            }
        }
        else
        {
            enemyState.SetIsGrounded(false);
            
            if(enemyState.GetState() != EnemyState.State.Charging)
            {
                enemyState.SetState(EnemyState.State.Falling);
            }
        }
    }
    
    private void ResetChargeSequence()
    {
        StopAllCoroutines();

        if(chargeCoroutine != null)
        {
            chargeCoroutine = null;
        }

        chargesLeft = currentMaxCharges;

        chargeCoroutine = StartCoroutine(ChargeBehaviour());
    }

    public bool GetIsInChargeCycle()
    {
        return isInChargeCycle;
    }
    #endregion

    #region Coroutines
     //Waits until the enemy is not attacking. Sets the state and the charge direction. Waits until the charge ends.
    private IEnumerator ChargeBehaviour()
    {
        enemyState.SetCanCharge(false);

        isInChargeCycle = true;

        if(isChargeParriable)
        {
            enemyState.SetState(EnemyState.State.Stationary);

            parriableTelegraph.SetActive(true);

            yield return new WaitForSeconds(chargeTelegraphDuration);

            parriableTelegraph.SetActive(false);
        }

        yield return new WaitUntil(() => enemyState.GetState() != EnemyState.State.Attacking && !enemyCombat.GetIsInCycleAttackInProgress());

        if(chargesLeft % 1 == 0)
        {
            float height = 50f;

            RaycastHit2D hit = Physics2D.Raycast(player.position, Vector2.up, height, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Wall"));

            if(!hit)
            {
                currentDestination = new Vector2(player.position.x, player.position.y + height);
            }
            else
            {
                currentDestination = new Vector2(hit.point.x, hit.point.y - 2f);
            }

            if(!isInPhaseTwo)
            {
                commitedDirection = currentDestination - myRigidbody.position;
            
                enemyState.SetState(EnemyState.State.Charging);
            }
            else
            {
                StartCoroutine(TeleportBehaviour(currentDestination));
            }
        }
        else
        {
            commitedDirection = GetPlayerNormal();
        
            enemyState.SetState(EnemyState.State.Charging);
        }

        if(enemyState.GetState() == EnemyState.State.Charging)
        {
            SetIsRotatingConstantly(true);

            if(chargesLeft % 1 == 0)
            {
                FlipCheck(true);
            }
            else
            {
                FlipCheck();
            }

            SetIsRotatingConstantly(false);

            yield return new WaitForSeconds(chargeDuration);
        }

        StartCoroutine(ChargeEndBehaviour());
    }

    private new IEnumerator ChargeEndBehaviour()
    {
        //If the boss is still charging it stops it.
        if(chargeCoroutine != null)
        {
            StopCoroutine(chargeCoroutine);

            chargeCoroutine = null;
        }

        myRigidbody.rotation = 0f;
        
        yield return new WaitUntil(() => enemyState.SetState(EnemyState.State.Stationary));

        if(chargesLeft % 1 != 0)
        {
            yield return new WaitUntil(() => enemyState.GetIsGrounded());

            GameObject aoe = Instantiate(aoeAttack, new Vector2(transform.position.x, transform.position.y - 2f), aoeAttack.transform.rotation);

            aoe.GetComponent<EnemyWeaponController>().ProjectileSetUp(20f);

            GameObject.Destroy(aoe, 0.5f);
        }

        //Keeps track of how many times the boss charges in a row.
        chargesLeft = chargesLeft - 0.5f;

        //If there are more charges left in the sequence, it starts the next one.
        if(chargesLeft > 0f)
        {
            if(chargesLeft % 1 == 0)
            {
                yield return new WaitForSeconds(currentTimeBetweenCharges);
            }
            
            chargeCoroutine = StartCoroutine(ChargeBehaviour());
        }
        else
        {
            isInChargeCycle = false;

            yield return new WaitForSeconds(chargeFrequency);

            chargesLeft = currentMaxCharges;

            enemyState.SetCanCharge(true);
        }
    }

    //Teleports the boss behind the player (or in front if it can't fit behind), then starts an attack straight away.
    public IEnumerator TeleportBehaviour(Vector2? destination = null)
    {
        yield return new WaitForSeconds(teleportBuildUpTime);

        enemyState.SetState(EnemyState.State.Teleporting);
        
        if(destination == null)
        {
            if(PlayerState.GetIsFacingRight())
            {
                if(!Physics2D.OverlapArea(new Vector2(player.position.x - 9f - targetExtents.x, player.position.y - targetExtents.y),
                new Vector2(player.position.x - 6f + targetExtents.x, player.position.y + targetExtents.y), (1 << LayerMask.NameToLayer("Wall") | (1 << LayerMask.NameToLayer("Ground")))))
                {
                    myRigidbody.position = new Vector2(player.position.x - 9f, player.position.y);
                }
                else
                {
                    myRigidbody.position = new Vector2(player.position.x + 9f, player.position.y);
                }
            }
            else
            {
                if(!Physics2D.OverlapArea(new Vector2(player.position.x + 9f - targetExtents.x, player.position.y - targetExtents.y),
                new Vector2(player.position.x + 6f + targetExtents.x, player.position.y + targetExtents.y), (1 << LayerMask.NameToLayer("Wall") | (1 << LayerMask.NameToLayer("Ground")))))
                {
                    myRigidbody.position = new Vector2(player.position.x + 9f, player.position.y);
                }
                else
                {
                    myRigidbody.position = new Vector2(player.position.x - 9f, player.position.y);
                }
            }
        }
        else
        {
            myRigidbody.position = destination.Value;
        }

        currentDirection = null;

        yield return new WaitForEndOfFrame();
        
        enemyState.SetState(EnemyState.State.Stationary);

        FlipCheck();

        if(destination == null)
        {
            enemyCombat.FinishAuxiliaryBehaviour();
        }
    }
    #endregion
}