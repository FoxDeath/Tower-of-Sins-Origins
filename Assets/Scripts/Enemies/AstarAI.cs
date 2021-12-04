using UnityEngine;
using Pathfinding;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SimpleSmoothModifier))]
[RequireComponent(typeof(EnemyState))]
[RequireComponent(typeof(EnemyHealth))]
public abstract class AstarAI : MonoBehaviour
{
    #region Attributes
    protected Transform player;

    protected Coroutine chargeCoroutine;
    protected Coroutine moveToPriorityDestinationCoroutine;
    
    private Path path;

    private Seeker seeker;
    
    protected GameObject parriableTelegraph;

    protected Rigidbody2D myRigidbody;

    protected Collider2D myTriggerCollider;
    protected Collider2D awarenessBaseline;

    private RaycastHit2D[] hits;

    private ContactFilter2D chargeCheckContactFilter = new ContactFilter2D();

    protected dynamic enemyState;

    protected EnemyHealth enemyHealth;

    protected EnemyCombat enemyCombat;

    protected PlayerHealth playerHealth;

    protected PlayerState playerState;

    protected Vector2? priorityDestination;
    protected Vector2? currentDirection;
    protected Vector2 commitedDirection = Vector2.zero;
    protected Vector2 gravityDirection = Vector2.down;
    protected Vector2 targetExtents;

    private float currentChargeCheckAngle;
    private float chargeCheckAngleOffset = 65f;
    private float gravityScale = 100f;
    private float nextWayPointDistance = 3f;
    private float pathUpdateFrequency = 1.2f;
    protected float maxVerticalSpeed = 50f;
    protected float parriableChargeSpeed = 60f;
    protected float unparriableChargeSpeed = 90f;
    [Tooltip("The duration the player has to be above the enemy for it to take counter action.")]
    [SerializeField] float cheeseFromAboveMaxTime = 2f;
    [Tooltip("How far the enemy has to be from the side of the screen/warenessBaseline for it to be aware of the player. Setting this to 0 means infinite range.")]
    [SerializeField] float pathUpdateDistance = 20f;
    [Tooltip("if the sprite has to be rotated in order to face sideways. Used only for when an enemy is being rotated 360 degrees.")]
    [SerializeField] float constantRotationDefault = 0f;
    [Tooltip("What fraction of the enemy's normal speed will be used while backing up from the player.")]
    [SerializeField] protected float backwardsSpeedModifier = 0.5f;
    [Tooltip("If the enemy is closer than this to the player, it will start backing up.")]
    [SerializeField] protected float distanceToKeep = 10f;
    [Tooltip("General speed the enemy moves with.")]
    [SerializeField] protected float speed = 10f;
    [Tooltip("The speed the enemy patrols with.")]
    [SerializeField] protected float patrolSpeed = 10f;
    [Tooltip("The distance the enemy moves in one direction before turning back during patrol. (patrol length)")]
    [SerializeField] protected float patrolOffset = 20f;
    [Tooltip("How long the enemy can charge without interruption.")]
    [SerializeField] protected float chargeDuration = 2.5f;
    [Tooltip("The time between two charges.")]
    [SerializeField] protected float chargeFrequency = 2f;
    [Tooltip("How long the telegraphing lasts before the charge behaviour starts.")]
    [SerializeField] protected float chargeTelegraphDuration = 0.3f;
    [Tooltip("How long the enemy has to be undisturbed for it to fall asleep.")]
    [SerializeField] protected float sleepTimer = 3f;
    
    private int currentWaypoint;
    [Tooltip("How much damage is inflicted on the palyer if they touch the enemy.")]
    [SerializeField] protected int collisionDamage = 10;

    protected bool interruptPriorityMovement = false;
    protected bool isInIdleBehaviour = true;
    protected bool isCurrentlyCheckingForCheese = false;
    protected bool isUsingGravity = true;
    protected bool shouldNotMove = false;
    protected bool isChargeContactFilterSetUp = false;
    [Tooltip("If the enemy should face the player directly or strictly face left and right.")]
    [SerializeField] bool isRotatingConstantly = false;
    [Tooltip("If the enemy should stop at a certain distance from the player, or just move as close as possible.")]
    [SerializeField] protected bool isKeepingDistance = false;
    [Tooltip("If the enemy should retreat when it's too close to the player or not.")]
    [SerializeField] protected bool isRetreatingWhenTooClose = false;
    [Tooltip("If the enemy should take countermeasures when the player is above them for a certain period or not.")]
    [SerializeField] protected bool isCheckingForCheeseFromAbove = true;
    [Tooltip("If this enemy is a boss or not.")]
    [SerializeField] protected bool isBoss = false;
    [Tooltip("This determines if the enemy gets knocked back or only stuck in place when hit or parried correctly.")]
    [SerializeField] protected bool isBiggerThanPlayer = false;
    [Tooltip("If the original sprite faces right or not. Needed for the flipping of the enemy.")]
    [SerializeField] protected bool isSpriteFacingRight = true;
    [Tooltip("If the enemy should go after the palyer between it's 'Move' behaviours or not.")]
    [SerializeField] protected bool isFollowingBetweenMoves = true;
    [Tooltip("If the charge of the enemy can be parried or not. It also changes the speed of the charge. Unparriable charging is faster than parriable.")]
    [SerializeField] protected bool isChargeParriable = true;
    [Tooltip("If the patrol behaviour of the enemy is resctricted or not. For air enemies it's a circle with a given radius, for ground enemies it's the platform they're on.")]
    [SerializeField] protected bool isRestrictedToArea = true;
    #endregion

    #region MonoBehaviour Methods
    protected void Awake()
    {
        seeker = GetComponent<Seeker>();

        myRigidbody = GetComponent<Rigidbody2D>();

        enemyState = GetComponent<EnemyState>();

        enemyHealth = GetComponent<EnemyHealth>();

        enemyCombat = GetComponent<EnemyCombat>();

        parriableTelegraph = transform.Find("Sprite").Find("ParriableTelegraph").gameObject;

        myTriggerCollider = transform.Find("TriggerCollider").GetComponent<Collider2D>();

        chargeCheckContactFilter.SetLayerMask(1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Wall"));
        
        if(GameObject.FindGameObjectWithTag("Player"))
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            
            playerHealth = player.gameObject.GetComponent<PlayerHealth>();

            playerState = player.GetComponent<PlayerState>();
            
            targetExtents = player.GetComponent<CapsuleCollider2D>().bounds.extents;

            awarenessBaseline = player.transform.Find("EnemyAwarenessBaseline").GetComponent<Collider2D>();
        }
    }

    protected void Start()
    {
        InvokeRepeating("UpdatePath", 0f, pathUpdateFrequency);
    }

    protected void Update()
    {
        //If there is no target specified, it will try and find the player and all it's necessary attributes.
        if((player == null || playerHealth == null
        || playerState == null || targetExtents == null)
        && GameObject.FindGameObjectWithTag("Player"))
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            
            playerHealth = player.gameObject.GetComponent<PlayerHealth>();

            playerState = player.GetComponent<PlayerState>();
            
            targetExtents = player.GetComponent<CapsuleCollider2D>().bounds.extents;

            awarenessBaseline = player.transform.Find("EnemyBaselineAwareness").GetComponent<Collider2D>();
        }

        if(isChargeContactFilterSetUp && enemyState.GetState() != EnemyState.State.Charging)
        {
            isChargeContactFilterSetUp = false;
        }
    }

    protected void FixedUpdate()
    {
        Gravity();

        TriggerColliderStateCheck();
        
        //If there is no path, no target, the enemy is at the end of the path or the player is out of range then it will only be idle.
        //Removed this: path.GetTotalLength() > pathUpdateDistance * 3f. It might introduce a certain bug.
        if((path == null || player == null
        || TargetOutOfPathUpdateDistance() || enemyState.GetState() == EnemyState.State.CirclingPlatrform))
        {
            RefreshCurrentDirection();

            //Makes sure that if the enemy is jumping or charging and gets out of the palyer's range, the behaviour still finishes.
            if(enemyState.GetState() == EnemyState.State.Charging || enemyState.GetState() == EnemyState.State.Jumping)
            {
                Move();
            }
            else
            {
                Idle();
            }

            return;
        }

        RefreshCurrentDirection();

        Move();
    }

    protected void OnCollisionStay2D(Collision2D other)
    {
        if((other.gameObject.tag.Equals("Ground") || other.gameObject.tag.Equals("Wall"))
        && enemyState.GetState() == EnemyState.State.Dead)
        {
            Destroy(gameObject, 0.5f);
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        //Kills the enemy if it falls into any kind of environmental danger.
        if(other.CompareTag("EnvironmentalDanger"))
        {
            enemyHealth.Die();

            playerHealth.Regenerate(false, true);
        }
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if(other.CompareTag("Player"))
        {
            shouldNotMove = true;

            //Sets it's collider as the enemy collider for the player.
            playerHealth.SetCollisionPosition(myRigidbody.position);

            //Applies the predetermined damage to the player.
            playerHealth.Hurt(collisionDamage);
            
            //Stops the charge behaviour prematurely if the enemy hits the player.
            if(enemyState.GetState() == EnemyState.State.Charging && !isBoss
            && !other.CompareTag("Attack") && !other.CompareTag("Parry")
            && isChargeParriable)
            {
                enemyState.SetState(EnemyState.State.Stationary);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            shouldNotMove = false;
        }
    }
    #endregion

    #region Normal Methods
    private void RefreshCurrentDirection()
    {
        if(path == null)
        {
            return;
        }

        float distance = Vector2.Distance(myRigidbody.position, path.vectorPath[currentWaypoint]);

        //Takes the next waypoint and calculates the way there if applicable.
        if(currentWaypoint > -1 && currentWaypoint < path.vectorPath.Count - 1
        && distance < nextWayPointDistance)
        {
            currentWaypoint++;

            currentDirection = ((Vector2)path.vectorPath[currentWaypoint] - myRigidbody.position).normalized;
        }
    }

    //If we have a target, previous path calculation is done and the player is in range, a new path calculation starts.
    private void UpdatePath()
    {
        if(!seeker.IsDone())
        {
            return;
        }

        if(priorityDestination.HasValue)
        {
            //Ground enemies get a 1D path, so height differences don't make them seize.
            if(isUsingGravity)
            {
                seeker.StartPath(myRigidbody.position, new Vector2(priorityDestination.Value.x, myRigidbody.position.y), OnPathComplete);
            }
            else
            {
                seeker.StartPath(myRigidbody.position, priorityDestination.Value, OnPathComplete);
            }
        }
        else if(player != null && !TargetOutOfPathUpdateDistance())
        {
            //Ground enemies get a 1D path, so height differences don't make them seize.
            if(isUsingGravity)
            {
                seeker.StartPath(myRigidbody.position, new Vector2(player.position.x, myRigidbody.position.y), OnPathComplete);
            }
            else
            {
                seeker.StartPath(myRigidbody.position, player.position, OnPathComplete);
            }
        }
    }

    protected bool TargetOutOfPathUpdateDistance()
    {
        float distanceFromAwarenessBaseline;

        if(Mathf.Abs(player.position.x - myRigidbody.position.x) < awarenessBaseline.bounds.extents.x - 1f
        && Mathf.Abs(player.position.y - myRigidbody.position.y) < awarenessBaseline.bounds.extents.y - 1f)
        {
            distanceFromAwarenessBaseline = Vector2.Distance(myRigidbody.position, awarenessBaseline.ClosestPoint(myRigidbody.position));
        }
        else
        {
            distanceFromAwarenessBaseline = -Vector2.Distance(myRigidbody.position, awarenessBaseline.ClosestPoint(myRigidbody.position));
        }

        if(pathUpdateDistance != 0)
        {
            return distanceFromAwarenessBaseline < pathUpdateDistance;
        }
        else
        {
            return false;
        }
    }

    //When path calculation is complete and there is no error, current path changes to the new one.
    private void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;

            currentWaypoint = 0;
        }
    }

    //The gravitiy is applied in code because it's easier to work with here.
    private void Gravity()
    {
        if(isUsingGravity)
        {
            myRigidbody.velocity += gravityDirection * gravityScale;
        }
    }

    //This is used to get the direction towards the target for several move behaviours.
    protected Vector2 GetPlayerNormal()
    {
        return ((Vector2)player.position - myRigidbody.position).normalized;
    }

    public bool IsPlayerInRange(float range = 0f)
    {
        //If a range is not specified, it will check if the enemy currently has a path or not.
        if(range == 0)
        {
            if(currentDirection != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //If there is a range specified, it checks if the target is within it.
        else
        {
            if(Vector2.Distance(player.position, myRigidbody.position) < range)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    //This is probably an overcomplicated way to flip the enemy, but it works so fuck it.
    protected void FlipCheck(bool ignorePlayer = false)
    {
        //The sprite is always flipped so that it's facing the player or the direction of movement.
        if(currentDirection != null && !ignorePlayer
        && enemyState.GetState() != EnemyState.State.Retreating && enemyState.GetState() != EnemyState.State.Patroling)
        {
            if(currentDirection.Value.x > 0f && player.position.x > transform.position.x)
            {
                if(!isSpriteFacingRight)
                {
                    Flip();
                }
            }
            else if(currentDirection.Value.x < 0f && player.position.x < transform.position.x)
            {
                if(isSpriteFacingRight)
                {
                    Flip();
                }
            }
        }
        else if(GetVelocity().x != 0f)
        {
            if(GetVelocity().x > 0f)
            {
                if(!isSpriteFacingRight)
                {
                    Flip();
                }
            }
            else if(GetVelocity().x < 0f)
            {
                if(isSpriteFacingRight)
                {
                    Flip();
                }
            }
        }
        else if(commitedDirection != Vector2.zero)
        {
            if(commitedDirection.x > 0f)
            {
                if(!isSpriteFacingRight)
                {
                    Flip();
                }
            }
            else if(commitedDirection.x < 0f)
            {
                if(isSpriteFacingRight)
                {
                    Flip();
                }
            }
        }
        else if(!isInIdleBehaviour)
        {
            if(GetPlayerNormal().x > 0f)
            {
                if(!isSpriteFacingRight)
                {
                    Flip();
                }
            }
            else
            {
                if(isSpriteFacingRight)
                {
                    Flip();
                }
            }
        }

        //If we want, the enemy will be rotated so it's looking right at the player or the direction it's moving in.
        if(isRotatingConstantly)
        {
            Vector2 direction;
        
            if(currentDirection != null && !ignorePlayer
            && enemyState.GetState() != EnemyState.State.Retreating && enemyState.GetState() != EnemyState.State.Patroling)
            {
                direction = GetPlayerNormal();
            }
            else if(commitedDirection != Vector2.zero)
            {
                direction = commitedDirection;
            }
            else
            {
                direction = GetVelocity().normalized;
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion rotation = Quaternion.identity;
            
            if(isSpriteFacingRight)
            {
                transform.rotation = Quaternion.Euler(Vector3.forward * (angle + constantRotationDefault));
            }
            else
            {
                transform.rotation = Quaternion.Euler(Vector3.forward * (angle - 180f - constantRotationDefault));
            }
        }
    }

    protected void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x *- 1,transform.localScale.y, transform.localScale.z);

        isSpriteFacingRight = !isSpriteFacingRight;
    }

    //Keeps the trigger colldier off when the enemy is sleeping so the player can walk past it.
    protected void TriggerColliderStateCheck()
    {
        if(enemyState.GetState() == EnemyState.State.Sleeping)
        {
            if(myTriggerCollider.enabled)
            {
                myTriggerCollider.enabled = false;
            }
        }
        else
        {
            if(!myTriggerCollider.enabled)
            {
                myTriggerCollider.enabled = true;
            }
        }
    }

    //Casts the enemy's trigger collider in the direction of the charge. If there is Ground or Wall in it's way,
    //it calles an Action. This stops the charge behaviour prematurely in some form, depending on the enemy.
    protected void EnvironmentalChargeInterruptionCheck(Action methodName)
    {
        hits = new RaycastHit2D[1];

        //The contact filter's normal angles are only set up once per charge.
        if(!isChargeContactFilterSetUp)
        {
            isChargeContactFilterSetUp = true;

            if(Vector2.SignedAngle(Vector2.left, commitedDirection) >= 0f)
            {
                currentChargeCheckAngle = Vector2.SignedAngle(Vector2.left, commitedDirection);
            }
            else
            {
                currentChargeCheckAngle = 360f + Vector2.SignedAngle(Vector2.left, commitedDirection);
            }

            //The contact filter will ignore any collision where the angle of impact is outside the normal angles.
            if(currentChargeCheckAngle + chargeCheckAngleOffset <= 360f)
            {
                chargeCheckContactFilter.SetNormalAngle(currentChargeCheckAngle - chargeCheckAngleOffset, currentChargeCheckAngle + chargeCheckAngleOffset);
            }
            else
            {
                chargeCheckContactFilter.SetNormalAngle(-(360f - (currentChargeCheckAngle - chargeCheckAngleOffset)), (360f - (currentChargeCheckAngle + chargeCheckAngleOffset)));
            }
        }
        
        myTriggerCollider.Cast(commitedDirection, chargeCheckContactFilter, hits, 1.5f, true);

        if(hits[0])
        {
            methodName();
        }
    }

    //If the player is able to parry the current attack, it dictates what should happen to the enemy.
    public bool Parried()
    {
        if((enemyState.GetState() == EnemyState.State.Charging && GetIsChargingInterruptable())
        || (enemyState.GetState() == EnemyState.State.Attacking && enemyCombat.GetCurrentParriable()))
        {
            if(enemyState.GetState() == EnemyState.State.Charging)
            {
                if(chargeCoroutine != null)
                {
                    StopCoroutine(chargeCoroutine);
                }
            }

            StartCoroutine(ChargeEndBehaviour());
            
            if(!isBiggerThanPlayer)
            {
                StartCoroutine(KnockedBackBehaviour());
            }
            else
            {
                StartCoroutine(StaggeredBehaviour());
            }
            
            return true;
        }

        return false;
    }

    public void UnfreezeRigidbodyRotation()
    {
        myRigidbody.constraints = RigidbodyConstraints2D.None;
    }

    public void ZeroOutCommitedDirection()
    {
        commitedDirection = Vector2.zero;
    }

    public void ZeroOutVelocity()
    {
        myRigidbody.velocity = Vector2.zero;
    }

    //The only way velocity is applied to the enemy. Except when it's zeroed out.
    protected void ApplyVelocity(Vector2 dir)
    {
        myRigidbody.velocity += dir.normalized * 1000f;
    }

    protected void ClampVelocityMagnitude(float magnitude)
    {
        myRigidbody.velocity = Vector2.ClampMagnitude(myRigidbody.velocity, magnitude);
    }

    protected void ClampVelocityAxes(float xClamp, float yClamp)
    {
        myRigidbody.velocity = new Vector2(Mathf.Clamp(myRigidbody.velocity.x, -xClamp, xClamp), Mathf.Clamp(myRigidbody.velocity.y, -yClamp, yClamp));
    }

    protected void ClampVelocityComplexAxes(float minXClamp, float maxXClamp, float minYClamp, float maxYClamp)
    {
        myRigidbody.velocity = new Vector2(Mathf.Clamp(myRigidbody.velocity.x, minXClamp, maxXClamp), Mathf.Clamp(myRigidbody.velocity.y, minYClamp, maxYClamp));
    }

    //Move behaviour implemented in scripts that inherit this one.
    protected virtual void Move()
    {
        isInIdleBehaviour = false;
    }

    //Idle behaviour implemented in scripts that inherit this one.
    protected virtual void Idle()
    {
        isInIdleBehaviour = true;
    }

    //Each subclass has to implement a method here the speed is clamped for every state.
    protected abstract void SpeedControl();

    #region Getters and Setters
    public Vector2 GetVelocity()
    {
        return myRigidbody.velocity;
    }

    public Transform GetTarget()
    {
        return player;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float GetPatrolSpeed()
    {
        return patrolSpeed;
    }

    public float GetPatrolOffset()
    {
        return patrolOffset;
    }

    public float GetChargeDuration()
    {
        return chargeDuration;
    }

    public float  GetChargeFrequency()
    {
        return chargeFrequency;
    }

    public float GetSleepTimer()
    {
        return sleepTimer;
    }

    public int GetCollisionDamage()
    {
        return collisionDamage;
    }

    public bool GetIsTherePriorityDestination()
    {
        return priorityDestination.HasValue;
    }

    public bool GetIsRestrictedToArea()
    {
        return isRestrictedToArea;
    }

    public bool GetIsFollowingBetweenMoves()
    {
        return isFollowingBetweenMoves;
    }

    public bool GetIsChargingInterruptable()
    {
        return isChargeParriable;
    }

    public bool GetIsRotatingConstantly()
    {
        return isRotatingConstantly;
    }

    public bool GetIsSpriteFacingRight()
    {
        return isSpriteFacingRight;
    }

    public bool GetIsBoss()
    {
        return isBoss;
    }

    public bool GetIsBiggerThanPlayer()
    {
        return isBiggerThanPlayer;
    }

    public void SetSpeed(float speed)
    {
        if(this.speed != speed)
        {
            this.speed = speed;
        }
    }

    public void SetPatrolSpeed(float patrolSpeed)
    {
        if(this.patrolSpeed != patrolSpeed)
        {
            this.patrolSpeed = patrolSpeed;
        }
    }

    public void SetPatrolOffset(float patrolOffset)
    {
        if(this.patrolOffset != patrolOffset)
        {
            this.patrolOffset = patrolOffset;
        }
    }

    public void SetChargeDuration(float chargeDuration)
    {
        if(this.chargeDuration != chargeDuration)
        {
            this.chargeDuration = chargeDuration;
        }
    }

    public void SetChargeFrequency(float chargeFrequency)
    {
        if(this.chargeFrequency != chargeFrequency)
        {
            this.chargeFrequency = chargeFrequency;
        }
    }

    public void SetSleepTimer(float sleepTimer)
    {
        if(this.sleepTimer != sleepTimer)
        {
            this.sleepTimer = sleepTimer;
        }
    }

    public void SetCollisionDamage(int collisionDamage)
    {
        if(this.collisionDamage != collisionDamage)
        {
            this.collisionDamage = collisionDamage;
        }
    }

    public void SetIsUsingGravity(bool isUsingGravity)
    {
        if(this.isUsingGravity != isUsingGravity)
        {
            this.isUsingGravity = isUsingGravity;
        }
    }

    public void SetIsKeepingDistance(bool isKeepingDistance)
    {
        if(this.isKeepingDistance != isKeepingDistance)
        {
            this.isKeepingDistance = isKeepingDistance;
        }
    }

    public void SetIsFollowingBetweenMoves(bool isFollowingBetweenMoves)
    {
        if(this.isFollowingBetweenMoves != isFollowingBetweenMoves)
        {
            this.isFollowingBetweenMoves = isFollowingBetweenMoves;
        }
    }

    public void SetIsChargingInterruptable(bool isChargeInterruptable)
    {
        if(this.isChargeParriable != isChargeInterruptable)
        {
            this.isChargeParriable = isChargeInterruptable;
        }
    }

    public void SetIsRotatingConstantly(bool isRotatingConstantly)
    {
        if(this.isRotatingConstantly != isRotatingConstantly)
        {
            this.isRotatingConstantly = isRotatingConstantly;
        }
    }

    public void SetIsRestrictedToArea(bool isRestrictedToArea)
    {
        if(this.isRestrictedToArea != isRestrictedToArea)
        {
            this.isRestrictedToArea = isRestrictedToArea;
        }
    }
    #endregion
    #endregion

    #region Coroutine
    protected IEnumerator MoveToPriorityDestinationBehaviour(Vector2 destinationOrTargetOffset, Func<bool> breakCondition, EnemyState.State stateToBeIn, Transform targetIfItsMoving = null)
    {
        if(targetIfItsMoving != null)
        {
            priorityDestination = (Vector2)targetIfItsMoving.position + destinationOrTargetOffset;
        }
        else
        {
            priorityDestination = destinationOrTargetOffset;
        }
        
        enemyState.SetState(stateToBeIn);
        
        UpdatePath();

        yield return new WaitUntil(() => currentDirection != null);

        ApplyVelocity(currentDirection.Value);

        yield  return new WaitForFixedUpdate();

        while(Vector2.Distance(myRigidbody.position, destinationOrTargetOffset) > 0.2f)
        {
            if(breakCondition() || interruptPriorityMovement)
            {
                interruptPriorityMovement = false;

                priorityDestination = null; 

                currentDirection = null;

                if(enemyState.GetState() != EnemyState.State.Charging)
                {
                    enemyState.SetState(EnemyState.State.Stationary);
                }

                UpdatePath();

                moveToPriorityDestinationCoroutine = null;

                yield break;
            }

            if(targetIfItsMoving != null)
            {
                priorityDestination = (Vector2)targetIfItsMoving.position + destinationOrTargetOffset;
            }

            ApplyVelocity(currentDirection.Value);

            yield return null;
        }

        priorityDestination = null;

        currentDirection = null;

        if(enemyState.GetState() != EnemyState.State.Charging)
        {
            enemyState.SetState(EnemyState.State.Stationary);
        }

        UpdatePath();

        moveToPriorityDestinationCoroutine = null;

        yield return null;
    }

    //If the player stays above the enemy for a certain period, then the enemy will utilise some type of counter measure. The input method will be called.
    //
    //Certain enemy types might want to implement this functionality in different ways. To do so, check how this
    //method gets called in the FixedUpdate metod of GroundEnemyMovement and take that as the basis of your implementaiton.
    protected IEnumerator CheeseFromAboveCheckBehaviour(Action methodName)
    {
        float timer = cheeseFromAboveMaxTime;

        while(timer > 0f)
        {
            timer -= Time.deltaTime;

            if(!Physics2D.CircleCast(new Vector2(myRigidbody.position.x, myRigidbody.position.y + myTriggerCollider.bounds.extents.y), 10f, Vector2.up, 15f,
            1 << LayerMask.NameToLayer("Player")))
            {
                isCurrentlyCheckingForCheese = false;

                yield break;
            }

            yield return null;
        }

        if(timer <= 0f)
        {
            methodName();
        }
        
        isCurrentlyCheckingForCheese = false;

        yield return null;
    }

    protected IEnumerator ChargeEndBehaviour()
    {
        if(enemyState.GetState() != EnemyState.State.Staggered && enemyState.GetState() != EnemyState.State.KnockedBack)
        {
            enemyState.SetState(EnemyState.State.Stationary);
        }

        chargeCoroutine = null;

        yield return new WaitForSeconds(chargeFrequency);

        enemyState.SetCanCharge(true);
    }

    //The timer that puts the enemy to sleep when it runs out.
    protected IEnumerator WaitingToSleepBehaviour()
    {
        enemyState.SetIsWaitingToSleep(true);

        yield return new WaitForSeconds(sleepTimer);

        enemyState.SetState(EnemyState.State.Sleeping);
        
        enemyState.SetIsWaitingToSleep(false);
    }

    public IEnumerator KnockedBackBehaviour()
    {
        if(isUsingGravity)
        {
            if(GetPlayerNormal().x > 0f)
            {
                commitedDirection = new Vector2(-1f, 0.5f);
            }
            else
            {
                commitedDirection = new Vector2(1f, 0.5f);
            }
        }
        else
        {
            commitedDirection = -GetPlayerNormal();
        }

        yield return new WaitForSeconds(2f);
    }

    public IEnumerator StaggeredBehaviour()
    {
        enemyState.SetCanDealDamage(false);

        enemyState.SetState(EnemyState.State.Staggered);

        yield return new WaitForSeconds(0.5f);
        
        enemyState.SetCanDealDamage(true);
        
        priorityDestination = null;
        
        enemyState.SetState(EnemyState.State.Stationary);
    }
    #endregion
}