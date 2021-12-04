using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemyMovement : AstarAI
{
    #region Attributes
    public enum MoveOptions
    {
        Stationary,
        KeepIdling,
        Follow,
        Charge,
        Jump
    }
    public enum IdleOptions
    {
        Stationary,
        Patrol,
        Asleep,
        CirclePlatform
    }
    
    [Tooltip("The behaviour that the enemy will do while the player is in it's range.")]
    [SerializeField] MoveOptions currentMoveOption = MoveOptions.Follow;
    [Tooltip("The behaviour that the enemy will do while the player is out of it's range.")]
    [SerializeField] IdleOptions currentIdleOption = IdleOptions.Patrol;

    private Coroutine waitingToSleepCoroutine;

    private Transform edgeDetection;
    private Transform wallDetection;
    private Transform groundDetection;

    private Vector2? basePoint;

    [Tooltip("What speed will the enemy be moved upwards during the jump.")]
    [SerializeField] float jumpSpeed = 30f;
    [Tooltip("How long the jumping behaviour lasts. The jump speed and direction will be applied to the enemy for this duration.")]
    [SerializeField] float jumpDuration = 0.2f;
    [Tooltip("The duration between jumps.")]
    [SerializeField] float jumpFrequency = 2f;
    
    private bool toBase = false;
    private bool isBlockedByPlatform = false;
    private bool canResetPatrol = true;
    private bool isTouchingPlatform = false;
    [Tooltip("If the platform circling enemy goes clockwise (right) or the opposite.")]
    [SerializeField] bool isGoingRight = true;
    #endregion

    #region MonoBehaviour Methods
    private new void Awake()
    {
        base.Awake();

        //Creating the spacial detectors.
        Transform sprite = transform.Find("Sprite");

        edgeDetection = new GameObject("EdgeDetection").transform;

        edgeDetection.parent = sprite;

        wallDetection = new GameObject("WallDetection").transform;

        wallDetection.parent = sprite;

        groundDetection = new GameObject("GroundDetection").transform;

        groundDetection.parent = sprite;

        //Setting up the positions of the spacial detectors.
        Collider2D collider = GetComponent<Collider2D>();

        if(isSpriteFacingRight)
        {
            edgeDetection.position = new Vector2(collider.bounds.max.x, collider.bounds.min.y);

            wallDetection.position = new Vector2(collider.bounds.max.x, collider.bounds.center.y);
        }
        else
        {
            edgeDetection.position = new Vector2(collider.bounds.min.x, collider.bounds.min.y);

            wallDetection.position = new Vector2(collider.bounds.min.x, collider.bounds.center.y);
        }

        groundDetection.position = new Vector2(collider.bounds.center.x, collider.bounds.min.y);
    }

    private new void Update()
    {
        base.Update();

        //Rotates the enemy except when it's not allowed to.
        if(enemyState.GetState() != EnemyState.State.Charging && enemyState.GetState() != EnemyState.State.Jumping
        && enemyState.GetState() != EnemyState.State.Sleeping && enemyState.GetState() != EnemyState.State.CirclingPlatrform
        && enemyState.GetState() != EnemyState.State.Attacking && enemyState.GetState() != EnemyState.State.Attacking)
        {
            FlipCheck();
        }
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();

        //Edge detection ray. Update this if raycast changes.
        Debug.DrawRay(edgeDetection.position, -transform.up * 2f, Color.red);

        if(enemyState.GetState() != EnemyState.State.CirclingPlatrform)
        {
            //This makes sure that the direction of the gravity is correct if behaviour changes on runtime.
            if(gravityDirection != Vector2.down)
            {
                gravityDirection = Vector2.down;
            }

            //This makes sure that the rotation is correct if behaviour changes on runtime.
            if(transform.rotation != Quaternion.identity)
            {
                transform.rotation = Quaternion.identity;
            }
        }

        GroundedCheck();

        BlockedByPlatformCheck();

        if(enemyState.GetState() == EnemyState.State.Charging)
        {
            EnvironmentalChargeInterruptionCheck(StationaryBehaviour);
        }

        if(isCheckingForCheeseFromAbove && Physics2D.CircleCast(new Vector2(myRigidbody.position.x, myRigidbody.position.y + myTriggerCollider.bounds.extents.y),
        myTriggerCollider.bounds.extents.y, Vector2.up, 15f, 1 << LayerMask.NameToLayer("Player")) && !isCurrentlyCheckingForCheese && Mathf.Abs(GetVelocity().x) < 0.1f
        && enemyState.GetState() != EnemyState.State.Retreating && enemyState.GetState() != EnemyState.State.Staggered
        && enemyState.GetState() != EnemyState.State.KnockedBack)
        {
            isCurrentlyCheckingForCheese = true;

            StartCoroutine(CheeseFromAboveCheckBehaviour(() => StartCoroutine(SeizureAgainstCheeseFromAbove())));
        }

        SpeedControl();
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        //This stops the jumping behaviour prematurely if it hits another platform.
        if((other.gameObject.tag.Equals("Ground") || other.gameObject.tag.Equals("Wall"))
        && enemyState.GetState() == EnemyState.State.Jumping)
        {
            enemyState.SetState(EnemyState.State.Falling);
        }
    }
    
    private new void OnCollisionStay2D(Collision2D other)
    {
        base.OnCollisionStay2D(other);

        //This bool is used for checking if the Platform Circling enemy should rotate or not.
        if((other.gameObject.tag.Equals("Ground") || other.gameObject.tag.Equals("Wall"))
        && !isTouchingPlatform)
        {
            isTouchingPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        //This bool is used for checking if the Platform Circling enemy should rotate or not.
        if((other.gameObject.tag.Equals("Ground") || other.gameObject.tag.Equals("Wall"))
        && isTouchingPlatform)
        {
            isTouchingPlatform = false;
        }
    }
    #endregion

    #region Normal Methods
    protected override void Move()
    {
        base.Move();

        //The enemy won't run movement behaviours while it's attacking, staggered, retreating(responding to cheese from above) or when it has no path.
        if(enemyState.GetState() == EnemyState.State.Attacking || currentDirection == null
        || enemyState.GetState() == EnemyState.State.KnockedBack || enemyState.GetState() == EnemyState.State.Retreating
        || enemyState.GetState() == EnemyState.State.Staggered)
        {
            return;
        }

        //The enemy won't start it's movement behaviour when it's asleep or if it's circling, even when in range.
        if(currentIdleOption == IdleOptions.CirclePlatform || enemyState.GetState() == EnemyState.State.Sleeping)
        {
            Idle();

            return;
        }

        //If the player is in recovery and the enemy is inside them, it won't start bugging out.
        if(shouldNotMove && enemyState.GetState() != EnemyState.State.Charging
        && enemyState.GetState() != EnemyState.State.CirclingPlatrform)
        {
            StationaryBehaviour();

            return;
        }

        if(currentIdleOption == GroundEnemyMovement.IdleOptions.CirclePlatform || currentMoveOption == GroundEnemyMovement.MoveOptions.Jump)
        {
            isCheckingForCheeseFromAbove = false;
        }

        //The timer that puts enemies to sleep is stopped when a movement behaviour is started.
        if(enemyState.GetIsWaitingToSleep())
        {
            enemyState.SetIsWaitingToSleep(false);

            StopCoroutine(waitingToSleepCoroutine);
        }

        //Resets the base point used for patrolling if needed.
        if(currentMoveOption != MoveOptions.KeepIdling && basePoint != null)
        {
            basePoint = null;
        }
        
        if(enemyState.GetIsGrounded())
        {
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

                case MoveOptions.Jump:
                    JumpMoveBehaviour();
                break;

                default:
                break;
            }
        }
    }

    protected override void Idle()
    {
        base.Idle();

        //The enemy won't run it's idle behaviour while it's attacking or retreating(responding to cheese from above).
        if(enemyState.GetState() == EnemyState.State.Attacking || enemyState.GetState() == EnemyState.State.Retreating
        || enemyState.GetState() == EnemyState.State.Staggered || priorityDestination.HasValue)
        {
            return;
        }

        //When the enemy stops the move behaviour or it's instantiated, a new base point is set for patrolling.
        if(basePoint == null)
        {
            basePoint = myRigidbody.position;
            
            toBase = false;
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

            case IdleOptions.CirclePlatform:
                CirclePlatformIdleBehaviour();
            break;

            default:
            break;
        }
    }

    private void StationaryBehaviour()
    {
        if(enemyState.GetIsGrounded())
        {
            if(enemyState.GetState() != EnemyState.State.KnockedBack && enemyState.GetState() != EnemyState.State.Staggered)
            {
                if((IsEndOfPlatform() || isBlockedByPlatform)
                && isInIdleBehaviour && enemyState.GetPreviousState() == EnemyState.State.Following)
                {
                    Vector2 destination;

                    bool ogIsSpriteFacingRight = isSpriteFacingRight;

                    if(isSpriteFacingRight)
                    {
                        destination = new Vector2(myRigidbody.position.x - 30f, myRigidbody.position.y);
                    }
                    else
                    {
                        destination = new Vector2(myRigidbody.position.x + 30f, myRigidbody.position.y);
                    }
                    
                    if(patrolSpeed == 0f)
                    {
                        patrolSpeed = speed;
                    }

                    moveToPriorityDestinationCoroutine = StartCoroutine(MoveToPriorityDestinationBehaviour(destination, () => !isInIdleBehaviour || (isSpriteFacingRight == !ogIsSpriteFacingRight
                    && (IsEndOfPlatform() || isBlockedByPlatform)), EnemyState.State.Patroling));
                }
                else
                {
                    enemyState.SetState(EnemyState.State.Stationary);
                }
            }
        }
        else
        {
            enemyState.SetState(EnemyState.State.Falling);
        }
    }

    //Moves the enemy towards the target if it's not restricted to an area, at the edge of a platform or inside the target.
    private void FollowMoveBehaviour()
    {
        if((!isRestrictedToArea || !IsEndOfPlatform())
        && Mathf.Abs(myRigidbody.position.x - player.position.x) - (myTriggerCollider.bounds.extents.x / 2f) - (targetExtents.x / 2f) > 0f)
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
                ApplyVelocity(new Vector2(-currentDirection.Value.x, 0f));
                
                enemyState.SetState(EnemyState.State.Retreating);
            }
            //Moves towards target if it's not close enough.
            else if((isKeepingDistance && Vector2.Distance(myRigidbody.position, player.position) > distanceToKeep + 1) || !isKeepingDistance)
            {
                ApplyVelocity(new Vector2(currentDirection.Value.x, 0f));
                
                enemyState.SetState(EnemyState.State.Following);
            }
        }
        else
        {
            StationaryBehaviour();
        }
    }

    private void ChargeMoveBehaviour()
    {
        if(enemyState.GetCanCharge() && chargeCoroutine == null)
        {
            chargeCoroutine = StartCoroutine(ChargeBehaviour());
        }
        else if(isFollowingBetweenMoves && enemyState.GetState() != EnemyState.State.Charging)
        {
            FollowMoveBehaviour();
        }
    }

    private void JumpMoveBehaviour()
    {
        if(enemyState.GetCanJump())
        {
            StartCoroutine(JumpBehaviour());
        }
        else if(isFollowingBetweenMoves)
        {
            FollowMoveBehaviour();
        }
    }

    private void PatrolIdleBehaviour()
    {
        if(enemyState.GetIsGrounded())
        {
            enemyState.SetState(EnemyState.State.Patroling);

            //the toBase bool determines which direction the enemy should moving.
            if(toBase)
            {
                //Moves the enemy towards the base point.
                if(enemyState.GetIsGrounded())
                {
                    ApplyVelocity(new Vector2((basePoint.Value - new Vector2(basePoint.Value.x + patrolOffset, basePoint.Value.y)).x, 0f));
                }

                //Changes direction when reached base point.
                if(basePoint.Value.x - myRigidbody.position.x > 0f)
                {
                    toBase = false;
                }
            }
            else
            {
                //Moves the enemy away from the base point.
                if(enemyState.GetIsGrounded())
                {
                    ApplyVelocity(new Vector2((new Vector2(basePoint.Value.x + patrolOffset, basePoint.Value.y) - basePoint).Value.x, 0f));
                }
                
                //Changes direction when too far from base point.
                if((basePoint.Value.x + patrolOffset) - myRigidbody.position.x < 0f)
                {
                    toBase = true;
                }
            }

            //Changes patrol direction when applicable.
            if(((IsEndOfPlatform() && isRestrictedToArea) || isBlockedByPlatform) && canResetPatrol)
            {
                StartCoroutine(ResetPatrolBehaviour());
            }
        }
    }

    private void AsleepIdleBehaviour()
    {
        if(enemyState.GetIsGrounded())
        {
            //When the enemy is supposed to sleep, it starts the timer.
            if(currentDirection == null)
            {
                if(!enemyState.GetIsWaitingToSleep() && enemyState.GetState() != EnemyState.State.Sleeping)
                {
                    StationaryBehaviour();

                    waitingToSleepCoroutine = StartCoroutine(WaitingToSleepBehaviour());
                }
            }
            else
            //Wakes up the enemy if the player is making noise around it.
            {
                if(playerState.IsMakingNoise())
                {
                    enemyState.SetState(EnemyState.State.Stationary);
                }
            }
        }
    }
    
    private void CirclePlatformIdleBehaviour()
    {
        if(!isBiggerThanPlayer)
        {
            isBiggerThanPlayer = true;
        }

        enemyState.SetState(EnemyState.State.CirclingPlatrform);
        
        //Sets the right direction and facing for the enemy.
        if(isGoingRight)
        {
            if(commitedDirection != (Vector2)transform.right)
            {
                commitedDirection = transform.right;
            }

            if(!isSpriteFacingRight)
            {
                Flip();
            }
        }
        else
        {
            if(commitedDirection != (Vector2)(-transform.right))
            {
                commitedDirection = -transform.right;
            }

            if(isSpriteFacingRight)
            {
                Flip();
            }
        }
        
        //If the path is clear, velocity is applied.
        if(enemyState.GetIsGrounded() && !CirclingHasToRotate() && !isBlockedByPlatform)
        {
            ApplyVelocity(commitedDirection);
        }
        //If the enemy reaches the end of a platform, it stops and turns accordingly.
        else if(CirclingHasToRotate())
        {
            ZeroOutVelocity();

            if(isGoingRight)
            {
                transform.RotateAround(groundDetection.position, Vector3.forward, -90f);
            }
            else
            {
                transform.RotateAround(groundDetection.position, Vector3.forward, 90f);
            }
            
            gravityDirection = -transform.up;
        }
        //If the enemy gets blocked by a wall, it stops and turns accordingly.
        else if(isBlockedByPlatform)
        {
            ZeroOutVelocity();

            if(isGoingRight)
            {
                transform.Rotate(0f, 0f, 90f);
            }
            else
            {
                transform.Rotate(0f, 0f, -90f);
            }
            
            gravityDirection = -transform.up;
        }
    }
    
    //This is where the enemy's speed is set for each state.
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
            
            case EnemyState.State.CirclingPlatrform:ClampVelocityAxes(speed, speed);
            break;
            
            case EnemyState.State.Jumping:
                ApplyVelocity(new Vector2(commitedDirection.x, 1f));

                ClampVelocityAxes(jumpSpeed / 2f,  jumpSpeed);
            break;
            
            case EnemyState.State.Patroling:
                ClampVelocityAxes(patrolSpeed, maxVerticalSpeed);
            break;
            
            case EnemyState.State.Charging:
                if(enemyState.GetIsGrounded())
                {
                    ApplyVelocity(new Vector2(commitedDirection.x, 0f));
                }

                if(isChargeParriable)
                {
                    ClampVelocityAxes(parriableChargeSpeed, maxVerticalSpeed);
                }
                else
                {
                    ClampVelocityAxes(unparriableChargeSpeed, maxVerticalSpeed);
                }
            break;
            
            case EnemyState.State.Stationary:
                ClampVelocityAxes(0f, maxVerticalSpeed);
            break;
            
            case EnemyState.State.Staggered:
                ClampVelocityAxes(0f, maxVerticalSpeed);
            break;
            
            case EnemyState.State.Dead:
                ClampVelocityAxes(0f, maxVerticalSpeed);
            break;
            
            case EnemyState.State.Sleeping:
                ClampVelocityAxes(0f, maxVerticalSpeed);
            break;
            
            case EnemyState.State.Attacking:
                ClampVelocityAxes(0f, maxVerticalSpeed);
            break;
            
            case EnemyState.State.KnockedBack:
                ApplyVelocity(commitedDirection);

                ClampVelocityAxes(45f, maxVerticalSpeed);
            break;
            
            case EnemyState.State.Retreating:
                ClampVelocityAxes(speed * backwardsSpeedModifier, maxVerticalSpeed);
            break;

            default:
            break;
        }
    }

    //This is what determines if the enemy is grounded or not, based on raycasts and states.
    private void GroundedCheck()
    {
        if(enemyState.GetState() != EnemyState.State.CirclingPlatrform)
        {
            if(Physics2D.Raycast(groundDetection.position, -transform.up, 0.3f, 1 << LayerMask.NameToLayer("Ground")))
            {
                enemyState.SetIsGrounded(true);

                if(enemyState.GetState() != EnemyState.State.Charging && enemyState.GetState() != EnemyState.State.Following
                && enemyState.GetState() != EnemyState.State.Patroling && enemyState.GetState() != EnemyState.State.Sleeping
                && enemyState.GetState() != EnemyState.State.CirclingPlatrform && enemyState.GetState() != EnemyState.State.Attacking
                && enemyState.GetState() != EnemyState.State.Retreating && enemyState.GetState() != EnemyState.State.Staggered
                && enemyState.GetState() != EnemyState.State.KnockedBack)
                {
                    enemyState.SetState(EnemyState.State.Stationary);
                }
            }
            else
            {
                enemyState.SetIsGrounded(false);
                
                if(enemyState.GetState() != EnemyState.State.Jumping && enemyState.GetState() != EnemyState.State.KnockedBack
                && enemyState.GetState() != EnemyState.State.Charging)
                {
                    enemyState.SetState(EnemyState.State.Falling);
                }
            }
        }
        //Platform Circling enemies need circle casts beacuse of the distance between vertexes in the composite colldiers. At least I think thats why this works.
        else
        {
            if(Physics2D.CircleCast(groundDetection.position, 0.1f, -transform.up, 0.1f, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Wall")))
            {
                enemyState.SetIsGrounded(true);
            }
            else
            {
                enemyState.SetIsGrounded(false);
                
                if(enemyState.GetState() != EnemyState.State.Jumping && enemyState.GetState() != EnemyState.State.KnockedBack
                && enemyState.GetState() != EnemyState.State.Charging)
                {
                    enemyState.SetState(EnemyState.State.Falling);
                }
            }
        }
    }

    //This determines if the enemies movement is blocked by another platform or not.
    private void BlockedByPlatformCheck()
    {
        if(isSpriteFacingRight)
        {
            if(Physics2D.BoxCast(wallDetection.position, new Vector2(myTriggerCollider.bounds.extents.x, myTriggerCollider.bounds.extents.y), myRigidbody.rotation, transform.right, 0.1f, 1 << LayerMask.NameToLayer("Wall"))
            || Physics2D.BoxCast(wallDetection.position, new Vector2(myTriggerCollider.bounds.extents.x, myTriggerCollider.bounds.extents.y), myRigidbody.rotation, transform.right, 0.1f, 1 << LayerMask.NameToLayer("Ground")))
            {
                isBlockedByPlatform = true;
            }
            else
            {
                isBlockedByPlatform = false;
            }
        }
        else
        {
            if(Physics2D.BoxCast(wallDetection.position, new Vector2(myTriggerCollider.bounds.extents.x, myTriggerCollider.bounds.extents.y), myRigidbody.rotation, -transform.right, 0.1f, 1 << LayerMask.NameToLayer("Wall"))
            || Physics2D.BoxCast(wallDetection.position, new Vector2(myTriggerCollider.bounds.extents.x, myTriggerCollider.bounds.extents.y), myRigidbody.rotation, -transform.right, 0.1f, 1 << LayerMask.NameToLayer("Ground")))
            {
                isBlockedByPlatform = true;
            }
            else
            {
                isBlockedByPlatform = false;
            }
        }
    }

    private bool IsEndOfPlatform()
    {
        //Update DrawRay() in FixedUpdate if you change the length of this.
        return !Physics2D.Raycast(edgeDetection.position, Vector2.down, 2f, 1 << LayerMask.NameToLayer("Ground"));
    }
    
    //Checks if the circling enemy reaches the edge of a platform.
    private bool CirclingHasToRotate()
    {
        //Update DrawRay()s in FixedUpdate if you change the length of this.
        if(!Physics2D.CircleCast(groundDetection.position, 0.1f, -transform.up, 0.1f, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Wall"))
        && isTouchingPlatform)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region Getters and Setters
    public float GetJumpSpeed()
    {
        return jumpSpeed;
    }

    public float GetJumpDuration()
    {
        return jumpDuration;
    }

    public float GetJumpFrequency()
    {
        return jumpFrequency;
    }

    public void SetJumpSpeed(float jumpSpeed)
    {
        if(this.jumpSpeed != jumpSpeed)
        {
            this.jumpSpeed = jumpSpeed;
        }
    }

    public void SetJumpDuration(float jumpDuration)
    {
        if(this.jumpDuration != jumpDuration)
        {
            this.jumpDuration = jumpDuration;
        }
    }

    public void SetJumpFrequency(float jumpFrequency)
    {
        if(this.jumpFrequency != jumpFrequency)
        {
            this.jumpFrequency = jumpFrequency;
        }
    }
    #endregion

    #region Coroutines
    //What the enemy does when the player was above them for too long.
    private IEnumerator SeizureAgainstCheeseFromAbove()
    {
        if(enemyState.GetState() == EnemyState.State.Retreating)
        {
            yield break;
        }

        float ogPatrolOffset = patrolOffset;

        patrolOffset = 10f;

        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        ContactFilter2D filter = new ContactFilter2D();

        filter.SetLayerMask(1 << LayerMask.NameToLayer("Player"));

        bool isLooping = true;

        while(isLooping)
        {
            Physics2D.CircleCast(new Vector2(myRigidbody.position.x, myRigidbody.position.y + myTriggerCollider.bounds.extents.y),
            myTriggerCollider.bounds.extents.y + 3f, Vector2.up, filter, hits, 15f);

            if(hits.Count == 0)
            {
                isLooping = false;

                break;
            }
            
            if(enemyState.GetIsGrounded() && hits[0].collider.name != "EnemyAwarenessBaseline")
            {
                enemyState.SetState(EnemyState.State.Retreating);

                if(basePoint == null)
                {
                    basePoint = myRigidbody.position;
                }

                PatrolIdleBehaviour();
            }

            yield return null;
        }
        
        basePoint = null;

        patrolOffset = ogPatrolOffset;

        enemyState.SetState(EnemyState.State.Stationary);
    }

    //Dictates the charge behaviours events. The movement is done up in the Move().
    private IEnumerator ChargeBehaviour()
    {
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
        
        if(GetPlayerNormal().x < 0f)
        {
            commitedDirection = Vector2.left;
        }
        else
        {
            commitedDirection = Vector2.right;
        }

        FlipCheck(true);

        yield return new WaitForSeconds(chargeDuration);
        
        StartCoroutine(ChargeEndBehaviour());
    }

    //Dictates the jump behaviours events. The movement is done up in the Move().
    private IEnumerator JumpBehaviour()
    {
        if(moveToPriorityDestinationCoroutine != null)
        {
            interruptPriorityMovement = true;
        }

        enemyState.SetState(EnemyState.State.Jumping);

        enemyState.SetCanJump(false);
        
        FlipCheck();

        commitedDirection = GetPlayerNormal();

        yield return new WaitForSeconds(jumpDuration);

        enemyState.SetState(EnemyState.State.Falling);

        yield return new WaitUntil(() => enemyState.GetIsGrounded());

        ZeroOutVelocity();

        yield return new WaitForSeconds(jumpFrequency);

        enemyState.SetCanJump(true);
    }

    //If, while patroling the enemy has to change direction prematurely, this takes care of it.
    private IEnumerator ResetPatrolBehaviour()
    {
        canResetPatrol = false;

        ZeroOutVelocity();

        toBase = !toBase;

        if(toBase)
        {
            basePoint = new Vector2(myRigidbody.position.x - patrolOffset, 0f);
        }
        else
        {
            basePoint = myRigidbody.position;
        }

        yield return new WaitUntil(() => !IsEndOfPlatform() && !isBlockedByPlatform);
        
        canResetPatrol = true;
    }
    #endregion
}