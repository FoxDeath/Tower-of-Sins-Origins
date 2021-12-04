using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirEnemyMovement : AstarAI
{
    #region Attributes
    public enum MoveOptions
    {
        Stationary,
        KeepIdling,
        Follow,
        Charge
    }
    public enum IdleOptions
    {
        Stationary,
        Patrol,
        Asleep
    }

    public enum AngleOfAttack
    {
        Vertical,
        Horizontal,
        Unrestricted
    }
    
    [Tooltip("The behaviour that the enemy will do while the player is in it's range.")]
    [SerializeField] MoveOptions currentMoveOption = MoveOptions.Follow;
    [Tooltip("The behaviour that the enemy will do while the player is out of it's range.")]
    [SerializeField] IdleOptions currentIdleOption = IdleOptions.Patrol;
    [Tooltip("The direction the enemy will approach the player from.")]
    [SerializeField] AngleOfAttack currentAngleOfAttack = AngleOfAttack.Unrestricted;

    private Coroutine waitingToSleepCoroutine;
    private Coroutine patrolDirectionChangeCoroutine;

    private Vector2? basePoint;
    
    [Tooltip("If clearWayForCharge is true, this is the distance the enemy moves away from the player, before charging.")]
    [SerializeField] float distanceToClear = 20f;
    [Tooltip("Before the charge behaviour starts, the enemy moves a certain distance away from the player.")]
    [SerializeField] bool isClearingWayForCharge = true;
    #endregion

    #region MonoBehaviour Methods
    private new void Start()
    {
        base.Start();
        
        isUsingGravity = false;
    }

    private new void Update()
    {
        base.Update();

        //Rotates the enemy except when it's not allowed to.
        if(enemyState.GetState() != EnemyState.State.Charging && enemyState.GetState() != EnemyState.State.Sleeping
        && enemyState.GetState() != EnemyState.State.Attacking && enemyState.GetState() != EnemyState.State.Dead)
        {
            FlipCheck();
        }
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        
        if(enemyState.GetState() == EnemyState.State.Charging)
        {
            EnvironmentalChargeInterruptionCheck(StationaryBehaviour);
        }

        SpeedControl();
    }

    private new void OnCollisionStay2D(Collision2D other)
    {
        base.OnCollisionStay2D(other);

        if(other.gameObject.tag.Equals("Ground") || other.gameObject.tag.Equals("Wall"))
        {
            //Initiates the patrol direction change if the enemy hits a platform.
            if(enemyState.GetState() == EnemyState.State.Patroling && patrolDirectionChangeCoroutine == null)
            {
                patrolDirectionChangeCoroutine = StartCoroutine(PatrolDirectionChangeBehaviour(other.GetContact(0).normal));
            }
        }
    }
    #endregion

    #region Normal Methods
    protected override void Move()
    {
        base.Move();

        //The enemy won't run movement behaviours while it's attacking or knocked back.
        if(enemyState.GetState() == EnemyState.State.Attacking || currentDirection == null
        || enemyState.GetState() == EnemyState.State.KnockedBack || enemyState.GetState() == EnemyState.State.Staggered)
        {
            return;
        }

        //If the player is in recovery and the enemy is inside them, it won't start bugging out.
        if(shouldNotMove && enemyState.GetState() != EnemyState.State.Charging)
        {
            StationaryBehaviour();

            return;
        }

        //If the enemy should be asleep, it won't run the other behaviours.
        if(enemyState.GetState() == EnemyState.State.Sleeping)
        {
            AsleepIdleBehaviour();

            return;
        }

        //The timer that puts enemies to sleep is stopped when a movement behaviour is started.
        if(enemyState.GetIsWaitingToSleep())
        {
            enemyState.SetIsWaitingToSleep(false);

            StopCoroutine(waitingToSleepCoroutine);
        }

        //Resets the base point used for patrolling if needed.
        if(currentMoveOption != MoveOptions.KeepIdling)
        {   
            if(basePoint != null)
            {
                basePoint = null;
            }
        }

        if(currentMoveOption != MoveOptions.Charge && currentMoveOption != MoveOptions.KeepIdling
        && commitedDirection != Vector2.zero)
        {
            commitedDirection = Vector2.zero;
        }

        //Determines which move behaviour is used.
        switch(currentMoveOption)
        {
            case MoveOptions.Stationary:
                StationaryBehaviour();
            break;

            case MoveOptions.KeepIdling:
                Idle();
            break;

            case MoveOptions.Follow:
                FollowMoveBehaviour();
            break;

            case MoveOptions.Charge:
                ChargeMoveBehaviour();
            break;

            default:
            break;
        }
    }

    protected override void Idle()
    {
        base.Idle();

        //The enemy won't run it's idle behaviour while it's attacking.
        if(enemyState.GetState() == EnemyState.State.Attacking || enemyState.GetState() == EnemyState.State.KnockedBack
        || enemyState.GetState() == EnemyState.State.Staggered)
        {
            return;
        }

        //When enemy stops the move behaviour or it's instantiated, a new base point is set for patrolling.
        if(basePoint == null)
        {
            basePoint = myRigidbody.position;
        }

        //Determines which idle behaviour is used.
        switch(currentIdleOption)
        {
            case IdleOptions.Stationary:
                StationaryBehaviour();
            break;

            case IdleOptions.Patrol:
                PatrolIdleBehaviour();
            break;

            case IdleOptions.Asleep:
                AsleepIdleBehaviour();
            break;

            default:
            break;
        }
    }

    private void StationaryBehaviour()
    {
        if(enemyState.GetState() != EnemyState.State.KnockedBack && enemyState.GetState() != EnemyState.State.Staggered)
        {
            enemyState.SetState(EnemyState.State.Stationary);
        }
    }

    private void FollowMoveBehaviour()
    {
        //Stops the enemy if it's supposed to keep a distance from the target .
        if(isKeepingDistance && Vector2.Distance(myRigidbody.position, player.position) > distanceToKeep - 1
        && Vector2.Distance(myRigidbody.position, player.position) < distanceToKeep + 1)
        {
            StationaryBehaviour();
        }
        //If the enemy is supposed to retreat when too close and it's too close to the target, it moves backwards.
        else if(isRetreatingWhenTooClose && Vector2.Distance(myRigidbody.position, player.position) < distanceToKeep - 1)
        {
            ApplyVelocity(-currentDirection.Value);
            
            enemyState.SetState(EnemyState.State.Retreating);
        }
        //Moves towards target if it's not close enough.
        else if((!TargetOutOfPathUpdateDistance() && isKeepingDistance
        && Vector2.Distance(myRigidbody.position, player.position) > distanceToKeep + 1) || !isKeepingDistance)
        {
            if(currentAngleOfAttack == AngleOfAttack.Vertical && Mathf.Abs(player.position.x - myRigidbody.position.x) > targetExtents.x / 2f + 3f)
            {
                if(!priorityDestination.HasValue)
                {
                    moveToPriorityDestinationCoroutine = StartCoroutine(MoveToPriorityDestinationBehaviour(
                    new Vector2(0f, 15f), () => isInIdleBehaviour || Mathf.Abs(player.position.x - myRigidbody.position.x) <= targetExtents.x / 2f + 2f, EnemyState.State.Following, player));
                }
            }
            else if(currentAngleOfAttack == AngleOfAttack.Horizontal && Mathf.Abs(player.position.y - myRigidbody.position.y) > targetExtents.y / 2f + 1f)
            {
                if(!priorityDestination.HasValue)
                {
                    FlipCheck();

                    bool ogIsSpriteFacingRight = isSpriteFacingRight;

                    if(isSpriteFacingRight)
                    {
                        moveToPriorityDestinationCoroutine = StartCoroutine(MoveToPriorityDestinationBehaviour(
                        new Vector2(-15f, 0f), () => isInIdleBehaviour || (isSpriteFacingRight == !ogIsSpriteFacingRight && Mathf.Abs(player.position.x - myRigidbody.position.x) > 4f)
                        || Mathf.Abs(player.position.y - myRigidbody.position.y) <= targetExtents.y / 2f, EnemyState.State.Following, player));
                    }
                    else
                    {
                        moveToPriorityDestinationCoroutine = StartCoroutine(MoveToPriorityDestinationBehaviour(
                        new Vector2(15f, 0f), () => isInIdleBehaviour || (isSpriteFacingRight == !ogIsSpriteFacingRight && Mathf.Abs(player.position.x - myRigidbody.position.x) > 4f)
                        || Mathf.Abs(player.position.y - myRigidbody.position.y) <= targetExtents.y / 2f, EnemyState.State.Following, player));
                    }
                }
            }
            else
            {
                ApplyVelocity(currentDirection.Value);
            
                enemyState.SetState(EnemyState.State.Following);
            }
        }
    }

    private void ChargeMoveBehaviour()
    {
        if(enemyState.GetCanCharge() && chargeCoroutine == null)
        {
            chargeCoroutine = StartCoroutine(ChargeBehaviour());
        }
        else if(enemyState.GetState() != EnemyState.State.Charging && enemyState.GetState() != EnemyState.State.Retreating)
        {
            if(isFollowingBetweenMoves)
            {
                FollowMoveBehaviour();
            }
            else
            {
                StationaryBehaviour();
            }
        }
    }

    private void PatrolIdleBehaviour()
    {
        enemyState.SetState(EnemyState.State.Patroling);

        //Sets a direction in which it should patrol if it doesn't have one.
        if(commitedDirection == Vector2.zero)
        {
            commitedDirection = new Vector2(Random.value, Random.value).normalized;
        }
        //If it's too far from it's base point and it's confined to an area, it will look for a new direction to patrol in.
        else if(isRestrictedToArea && Vector2.Distance(basePoint.Value, myRigidbody.position) > patrolOffset
        && patrolDirectionChangeCoroutine == null)
        {
            patrolDirectionChangeCoroutine = StartCoroutine(PatrolDirectionChangeBehaviour((basePoint.Value - myRigidbody.position).normalized));
        }
        else
        {
            ApplyVelocity(commitedDirection);
        }
    }

    private void AsleepIdleBehaviour()
    {
        if(currentDirection == null)
        {
            //When the enemy is supposed to sleep, it starts the timer.
            if(!enemyState.GetIsWaitingToSleep() && enemyState.GetState() != EnemyState.State.Sleeping)
            {
                StationaryBehaviour();

                waitingToSleepCoroutine = StartCoroutine(WaitingToSleepBehaviour());
            }
        }
        else
        {
            //Wakes up the enemy if the player is making noise around it.
            if(playerState.IsMakingNoise())
            {
                StationaryBehaviour();
            }
        }
    }

    //This is where the enemy's speed is set for each state.
    protected override void SpeedControl()
    {
        switch(enemyState.GetState())
        {
            case EnemyState.State.Patroling:
                ClampVelocityMagnitude(patrolSpeed);
            break;
            
            case EnemyState.State.Following:
                ClampVelocityMagnitude(speed);
            break;
            
            case EnemyState.State.Stationary:
                ClampVelocityMagnitude(0f);
            break;
            
            case EnemyState.State.Dead:
                ClampVelocityComplexAxes(0f, 0f, -maxVerticalSpeed, 0f);
            break;
            
            case EnemyState.State.Sleeping:
                ClampVelocityMagnitude(0f);
            break;
            
            case EnemyState.State.Attacking:
                ClampVelocityMagnitude(0f);
            break;
            
            case EnemyState.State.Staggered:
                ClampVelocityMagnitude(0f);
            break;
            
            case EnemyState.State.Charging:
                ApplyVelocity(commitedDirection);

                if(isChargeParriable)
                {
                    ClampVelocityMagnitude(parriableChargeSpeed);
                }
                else
                {
                    ClampVelocityMagnitude(unparriableChargeSpeed);
                }
            break;
            
            case EnemyState.State.KnockedBack:
                ApplyVelocity(commitedDirection);

                ClampVelocityMagnitude(25f);
            break;
            
            case EnemyState.State.Retreating:
                ClampVelocityMagnitude(speed * backwardsSpeedModifier);
            break;

            default:
            break;
        }
    }

    #region Getters and Setters
    public float GetBackwardsSpeedModifier()
    {
        return backwardsSpeedModifier;
    }

    public float GetDistanceToKeep()
    {
        return distanceToKeep;
    }

    public bool GetIsKeepingDistance()
    {
        return isKeepingDistance;
    }

    public void SetBackwardsSpeedModifier(float backwardsSpeedModifier)
    {
        if(this.backwardsSpeedModifier != backwardsSpeedModifier)
        {
            this.backwardsSpeedModifier = backwardsSpeedModifier;
        }
    }

    public void SetDistanceToKeep(float distanceToKeep)
    {
        if(this.distanceToKeep != distanceToKeep)
        {
            this.distanceToKeep = distanceToKeep;
        }
    }
    #endregion
    #endregion

    #region Coroutines
    //This determines the new patrol direction when needed.
    private IEnumerator PatrolDirectionChangeBehaviour(Vector2 normal)
    {
        commitedDirection = Quaternion.AngleAxis(Random.Range(-70, 70), Vector3.forward) * normal;

        ZeroOutVelocity();

        yield return new WaitForSeconds(0.3f);

        patrolDirectionChangeCoroutine = null;
    }

    //Dictates the charge behaviours events. The movement is done up in the SpeedControl().
    private IEnumerator ChargeBehaviour()
    {
        if(isClearingWayForCharge)
        {
            while(Vector2.Distance(myRigidbody.position, player.position) < distanceToClear)
            {
                enemyState.SetState(EnemyState.State.Retreating);

                ApplyVelocity(-GetPlayerNormal());
                
                yield return null;
            }
        }

        if(isChargeParriable)
        {
            enemyState.SetState(EnemyState.State.Stationary);

            parriableTelegraph.SetActive(true);

            yield return new WaitForSeconds(chargeTelegraphDuration);

            parriableTelegraph.SetActive(false);
        }
        
        if(moveToPriorityDestinationCoroutine != null)
        {
            interruptPriorityMovement = true;
        }

        enemyState.SetState(EnemyState.State.Charging);

        enemyState.SetCanCharge(false);

        ZeroOutVelocity();

        commitedDirection = GetPlayerNormal();

        FlipCheck(true);

        yield return new WaitForSeconds(chargeDuration);
        
        StartCoroutine(ChargeEndBehaviour());
    }
    #endregion
}
