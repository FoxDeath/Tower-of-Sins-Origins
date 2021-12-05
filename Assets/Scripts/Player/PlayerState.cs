using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(PlayerAnimationController))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerEdgeInteractions))]
[RequireComponent(typeof(PlayerSlide))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(PlayerRuneController))]
[RequireComponent(typeof(PlayerRecovery))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(Teleport))]
[RequireComponent(typeof(PlayerParticlesController))]
public class PlayerState : MonoBehaviour
{
    #region Attributes
    static private PlayerAnimationController playerAnimationController;
    static private PlayerMovement playerMovement;
    static private PlayerWallInteraction playerWallInteraction;
    static private PlayerEdgeInteractions playerEdgeInteractions;
    static private PlayerSlide playerSlide;
    static private PlayerInput playerInput;
    static private PlayerCombat playerCombat;
    static private PlayerRuneController playerRuneController;
    static private PlayerRecovery playerRecovery;
    static private PlayerHealth playerHealth;
    static private AudioManager audioManager;
    static private Teleport playerPortalController;
    static private PlayerParticlesController playerParticlesController;

    static private State state;

    public enum State
    {
        Idle,
        Moving,
        Jumping,
        WallJumping,
        WallJumpFalling,
        Falling,
        WallSliding,
        Hanging,
        Climbing,
        Sliding,
        Dashing,
        Casting,
        Attacking,
        Parrying,
        Dialoguing,
        Stunned,
        Staggered,
        Teleporting,
        Dead
    }

    static private bool shouldWallStuck;
    static private bool canSlide;
    static private bool canDash;
    static private bool canParry;
    static private bool canAttack;
    static private bool canMoveWallJump;
    static private bool isKnockedBack;
    static private bool isRecovering;
    static private bool isWalking;
    static private bool isLanded;
    static private bool isOnMovingPlatform; 
    static private bool isGrounded;
    static private bool isFacingRight;
    static private bool isWallStuck;
    static private bool isLookingAround;
    #endregion

    #region MonoBehaviour Methods
    private void Awake() 
    {
        playerAnimationController = GetComponent<PlayerAnimationController>();
        
        playerMovement = GetComponent<PlayerMovement>();

        playerWallInteraction = GetComponent<PlayerWallInteraction>();

        playerEdgeInteractions = GetComponent<PlayerEdgeInteractions>();

        playerSlide = GetComponent<PlayerSlide>();

        playerInput = GetComponent<PlayerInput>();

        playerCombat = GetComponent<PlayerCombat>();

        playerRuneController = GetComponent<PlayerRuneController>();

        playerRecovery = GetComponent<PlayerRecovery>();

        playerHealth = GetComponent<PlayerHealth>();

        audioManager = FindObjectOfType<AudioManager>();

        playerPortalController = GetComponent<Teleport>();

        playerParticlesController = GetComponent<PlayerParticlesController>();
    }

    private void Start() 
    {
        state = State.Idle;

        isFacingRight = true;
        
        isWallStuck = false;
        
        shouldWallStuck = true;
        
        canSlide = true;
        
        canDash = true;
        
        canParry = true;
        
        canAttack = true;
        
        canMoveWallJump = false;
        
        isWalking = false;
        
        isLanded = false;
        
        isLookingAround = false;
    }

    private void FixedUpdate()
    {
        if(PlayerState.state == State.Dead || isLookingAround)
        {
            return;
        }

        switch(state)
        {
            case State.Moving:
                playerMovement.Movement();

                playerMovement.Gravity();

                playerMovement.VelocityClamp();
            break;

            case State.Jumping:
                playerMovement.Movement();

                PlayerMovement.StopMoving();

                playerMovement.VelocityClamp();
            break;
            
            case State.WallJumping:
                Vector2 wallJumpSpeed = playerWallInteraction.GetWallJumpSpeed();

                if(canMoveWallJump)
                {
                    playerMovement.Movement();
                }

                playerMovement.VelocityClamp(wallJumpSpeed.x, wallJumpSpeed.y);
            break;

            case State.WallJumpFalling:
                playerMovement.Movement();
                
                playerMovement.Gravity();

                playerMovement.VelocityClamp();
            break;

            case State.Falling:
                playerMovement.Movement();

                playerMovement.Gravity();

                PlayerMovement.StopMoving();

                playerMovement.VelocityClamp(0f, playerMovement.GetMaxFallSpeed());
            break;

            case State.WallSliding:
                playerMovement.Movement();

                playerWallInteraction.WallSlide();

                playerMovement.VelocityClamp();
            break;

            case State.Hanging:
                playerMovement.Movement();

                playerMovement.StopGravity();

                playerMovement.VelocityClamp();
            break;

            case State.Climbing:                
                playerMovement.StopGravity();

                playerMovement.VelocityClamp();
            break;

            case State.Sliding:
                float slideSpeed = playerSlide.GetSlideSpeed();

                playerMovement.Gravity();

                playerMovement.VelocityClamp(slideSpeed, slideSpeed);
            break;

            case State.Dashing:
                float dashSpeed = playerSlide.GetSlideSpeed();

                playerMovement.StopGravity();

                playerMovement.VelocityClamp(dashSpeed, dashSpeed);
            break;

            case State.Attacking:
                playerMovement.Gravity();

                if(!GetIsKnockedBack())
                {
                    playerMovement.Movement();

                    PlayerMovement.StopMoving();
                }

                playerMovement.VelocityClamp();
            break;

            case State.Casting:
                playerMovement.Gravity();

                PlayerMovement.StopMoving();

                playerMovement.VelocityClamp();
            break;

            case State.Parrying:
                playerMovement.Movement();

                playerMovement.Gravity();

                PlayerMovement.StopMoving();

                playerMovement.VelocityClamp();
            break;

            case State.Stunned:
                playerMovement.Movement();

                playerMovement.Gravity();

                playerMovement.VelocityClamp();
            break;

            case State.Staggered:
                playerMovement.Gravity();

                playerMovement.VelocityClamp();
            break;

            case State.Dead:
                playerMovement.Gravity();

                playerMovement.VelocityClamp();
            break;

            case State.Idle:
                playerMovement.Movement();

                playerMovement.Gravity();

                playerMovement.VelocityClamp();
            break;

            case State.Teleporting:
                playerMovement.Movement();

                playerMovement.Gravity();

                playerMovement.VelocityClamp();
            break;

            case State.Dialoguing:
                playerMovement.Movement();

                playerMovement.Gravity();

                playerMovement.VelocityClamp();
            break;

            default:
            return;
        }
    }

    private void Update() 
    {
        if(PlayerState.state == State.Dead || isLookingAround)
        {
            return;
        }
        
        if(audioManager.IsPlaying("Run") && state != State.Moving)
        {
            audioManager.Stop("Run");
        }

        if(isWallStuck && PlayerState.state != State.WallSliding)
        {
            playerWallInteraction.StopWallStuck();
        }

        switch(state)
        {
            case State.Moving:
                playerInput.MovePerformedGameplay();
            break;

            case State.Jumping:
                playerInput.MovePerformedGameplay();

                if(GetIsGrounded() && PlayerMovement.GetVelocity().y <= 0f)
                {
                    SetState(State.Idle);
                }
            break;
            
            case State.WallJumping:
                if(canMoveWallJump)
                {
                    playerInput.MovePerformedGameplay();
                }

                if(GetIsGrounded())
                {
                    SetState(State.Idle);
                }
            break;

            case State.WallJumpFalling:
            break;

            case State.Falling:
            break;

            case State.WallSliding:
                playerInput.MovePerformedGameplay();

                if(GetIsGrounded())
                {
                    SetState(State.Idle);
                }
            break;

            case State.Hanging:
                playerInput.MovePerformedGameplay();
            break;

            case State.Climbing:
            break;

            case State.Sliding:
            break;

            case State.Dashing:
            break;

            case State.Casting:
            break;

            case State.Attacking:
            break;

            case State.Parrying:
            break;

            case State.Stunned:
            break;

            case State.Staggered:
            break;

            case State.Teleporting:
            break;

            case State.Dead:
            break;

            case State.Idle:
                playerInput.MovePerformedGameplay();
            break;

            case State.Dialoguing:
                //playerInput.InteractPerformedGameplay();
            break;

            default:
            return;
        }
    }
    #endregion

    #region  Normal Methods
    public bool IsMakingNoise()
    {   
        if((!isWalking && state == State.Moving)
        || state == State.Attacking || state == State.Parrying
        || state == State.Sliding || isLanded)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #region  Setters
    static public void SetState(State state)
    {
        if(PlayerState.state == State.Dead || isLookingAround)
        {
            return;
        }

        switch(state)
        {
            case State.Moving:
                if(GetIsGrounded() && PlayerState.state == State.Falling)
                {
                    playerMovement.StartCoroutine(playerMovement.LandingTimerBehaviour());
                }

                if(GetIsGrounded() && PlayerState.state != State.Moving 
                && (PlayerState.state == State.Idle || PlayerState.state == State.Falling
                || PlayerState.state == State.Sliding || PlayerState.state == State.WallJumpFalling
                || PlayerState.state == State.WallJumping || PlayerState.state == State.Parrying
                || PlayerState.state == State.Climbing || PlayerState.state == State.Staggered
                || PlayerState.state == State.Teleporting))
                {
                    playerAnimationController.SetTrigger("Run");

                    audioManager.Play("Run");

                    PlayerState.state = state;
                }
                else if(!GetIsGrounded() && PlayerState.state == State.Sliding)
                {
                    playerAnimationController.SetTrigger("Run");

                    audioManager.Play("Run");

                    PlayerState.state = state;
                }
            break;

            case State.Jumping:
                if(PlayerState.state != State.Jumping 
                && (PlayerState.state == State.Idle || PlayerState.state == State.Moving
                || PlayerState.state == State.Dashing || PlayerState.state == State.Teleporting 
                || PlayerState.state == State.Attacking))
                {
                    playerAnimationController.SetTrigger("Jump");

                    audioManager.Play("Jump");

                    PlayerState.state = state;
                    
                    if(GetIsGrounded())
                    {
                        playerMovement.Jump();
                    }
                }
            break;
            
            case State.WallJumping:
                if(PlayerState.state != State.WallJumping 
                && (PlayerState.state == State.WallSliding))
                {
                    playerAnimationController.SetTrigger("WallJump");

                    PlayerState.state = state;

                    SetShouldWallStuck(true);

                    playerWallInteraction.WallJump();

                    playerParticlesController.WallJumpParticles();
                }
            break;

            case State.WallJumpFalling:
                if(PlayerState.state != State.WallJumpFalling && PlayerState.state != State.Climbing
                && PlayerMovement.GetVelocity().y <= 0f)
                {
                    playerAnimationController.SetTrigger("Fall");

                    PlayerState.state = state;

                    playerInput.MovePerformedGameplay();
                }
            break;

            case State.Falling:
                if(PlayerState.state == State.WallJumping || PlayerState.state == State.WallJumpFalling)
                {
                    SetState(State.WallJumpFalling);
                    
                    return;
                }

                if(PlayerState.state != State.Falling && PlayerState.state != State.Climbing
                && (PlayerMovement.GetVelocity().y <= 0f || PlayerState.state == State.Dashing))
                {
                    playerAnimationController.SetTrigger("Fall");

                    PlayerState.state = state;
                }
            break;

            case State.WallSliding:
                if(PlayerState.state != State.WallSliding && !GetIsGrounded()
                && (PlayerState.state == State.Falling || PlayerState.state == State.WallJumping
                || PlayerState.state == State.WallJumpFalling || PlayerState.state == State.Dashing 
                || PlayerState.state == State.Attacking || PlayerState.state == State.Parrying))
                {
                    State oldState = PlayerState.state;

                    playerAnimationController.SetTrigger("WallSlide");

                    if(oldState != State.Attacking && oldState != State.Parrying)
                    {
                        audioManager.Play("WallJump");
                    }

                    PlayerState.state = state;

                    SetCanDash(true);

                    if(GetShouldWallStuck() && !GetIsWallStuck())
                    {
                        playerWallInteraction.WallStuck(oldState == State.Attacking || oldState == State.Parrying);
                    }
                }
            break;

            case State.Hanging:
                if(PlayerState.state != State.Hanging && !GetIsGrounded()
                && (PlayerState.state == State.Jumping || PlayerState.state == State.Moving
                || PlayerState.state == State.WallJumping || PlayerState.state == State.Falling
                || PlayerState.state == State.WallSliding || PlayerState.state == State.WallJumpFalling))
                {
                    playerAnimationController.SetTrigger("Hang");

                    PlayerState.state = state;

                    playerEdgeInteractions.EdgeHang();
                }
            break;

            case State.Climbing:
                if(PlayerState.state != State.Climbing 
                && (PlayerState.state == State.Hanging))
                {
                    playerAnimationController.SetTrigger("Climb");

                    PlayerState.state = state;

                    playerEdgeInteractions.EdgeClimb();
                }
            break;

            case State.Sliding:
                if(PlayerState.state != State.Sliding && GetCanSlide()
                && !GetIsWalking() && (PlayerState.state == State.Idle 
                || PlayerState.state == State.Moving || PlayerState.state == State.Attacking
                || PlayerState.state == State.Parrying || PlayerState.state == State.Teleporting
                || PlayerState.state == State.Dialoguing))
                {
                    playerAnimationController.SetTrigger("Slide");

                    audioManager.Play("Slide");

                    PlayerState.state = state;

                    playerSlide.Slide();
                }
            break;

            case State.Dashing:
                if(PlayerState.state != State.Dashing && GetCanDash() 
                && (PlayerState.state == State.Jumping || PlayerState.state == State.Falling
                || PlayerState.state == State.WallJumping || PlayerState.state == State.WallJumpFalling
                || PlayerState.state == State.WallSliding || PlayerState.state == State.Attacking
                || PlayerState.state == State.Parrying))
                {
                    playerAnimationController.SetTrigger("Dash");

                    audioManager.Play("Dash");

                    State oldState = PlayerState.state;

                    PlayerState.state = state;

                    playerSlide.Dash(oldState == State.WallSliding);
                }
            break;

            case State.Teleporting:
                if(PlayerState.state != State.Teleporting && PlayerState.state == State.Idle)
                {
                    PlayerState.state = state;

                    playerPortalController.Teleporting();
                }
            break;

            case State.Casting:
                if(GetIsGrounded() && PlayerState.state == State.Falling)
                {
                    playerMovement.StartCoroutine(playerMovement.LandingTimerBehaviour());
                }

                if(PlayerState.state == State.Idle && playerRuneController.GetCanUseSpecialAbility())
                {
                    playerAnimationController.SetTrigger("Cast");

                    audioManager.Play("Cast");

                    PlayerState.state = state;

                    PlayerMovement.StopMoving();

                    playerRuneController.SpecialAbility();
                }
            break;

            case State.Attacking:
                if(GetIsGrounded() && PlayerState.state == State.Falling)
                {
                    playerMovement.StartCoroutine(playerMovement.LandingTimerBehaviour());
                }

                if((PlayerState.state == State.Jumping || PlayerState.state == State.Moving
                || PlayerState.state == State.Falling || PlayerState.state == State.Idle
                || PlayerState.state == State.WallJumpFalling || PlayerState.state == State.Attacking
                || PlayerState.state == State.WallSliding || PlayerState.state == State.Teleporting))
                {
                    State oldState = PlayerState.state;

                    PlayerState.state = state;

                    playerCombat.Attack(oldState == State.WallSliding);
                }
            break;

            case State.Parrying:
                if(GetIsGrounded() && PlayerState.state == State.Falling)
                {
                    playerMovement.StartCoroutine(playerMovement.LandingTimerBehaviour());
                }
                
                if(PlayerState.state != State.Parrying && canParry 
                && (PlayerState.state == State.Jumping || PlayerState.state == State.Moving
                || PlayerState.state == State.Attacking || PlayerState.state == State.Falling
                || PlayerState.state == State.Idle || PlayerState.state == State.WallJumpFalling
                || PlayerState.state == State.WallSliding || PlayerState.state == State.Teleporting))
                {
                    State oldState = PlayerState.state;

                    PlayerState.state = state;

                    playerCombat.Parry(oldState == State.WallSliding);
                }
            break;

            case State.Stunned:
                playerAnimationController.SetTrigger("Stun");

                PlayerState.state = state;
            break;

            case State.Staggered:
                playerAnimationController.SetTrigger("Stagger");

                audioManager.Play("Hurt");

                PlayerState.state = state;
            break;

            case State.Dead:
                playerAnimationController.SetTrigger("Die");

                audioManager.Play("Die");

                playerHealth.OnDeath();

                PlayerState.state = state;
            break;

            case State.Dialoguing:
                // if(PlayerState.state != State.Dialoguing && GetIsGrounded())
                // {
                    PlayerState.state = state;
                // }
            break;

            case State.Idle:
                if(GetIsGrounded() && PlayerState.state == State.Falling)
                {
                    playerMovement.StartCoroutine(playerMovement.LandingTimerBehaviour());
                }

                if(PlayerState.state == State.Attacking || PlayerState.state == State.Parrying
                || PlayerState.state == State.Casting)
                {
                    if(!GetIsGrounded() && PlayerMovement.GetVelocity().y <= 0f)
                    {
                        SetState(State.Falling);
                    }
                    else if(!GetIsGrounded() && PlayerMovement.GetVelocity().y > 0f)
                    {
                        PlayerState.state = state;

                        SetState(State.Jumping);
                    }
                    else if(PlayerInput.GetMoveInput() != 0f)
                    {
                        PlayerState.state = state;
                        
                        SetState(State.Moving);
                    }
                    else
                    {
                        playerAnimationController.SetTrigger("Idle");

                        PlayerState.state = state;

                        PlayerMovement.StopMoving(true);
                    }
                }
                else if(GetIsGrounded() && PlayerState.state != State.Idle
                && PlayerState.state != State.Jumping)
                {
                    playerAnimationController.SetTrigger("Idle");

                    PlayerState.state = state;

                    PlayerMovement.StopMoving();
                }
                else if(GetIsGrounded() && (PlayerState.state == State.Jumping || PlayerState.state == State.WallJumping))
                {
                    SetState(State.Falling);
                }
                else if(!GetIsGrounded() && PlayerState.state != State.WallSliding)
                {
                    SetState(State.Falling);
                }
            break;

            default:
            return;
        }
    }

    static public void SetIsGrounded(bool isGrounded)
    {
        if(PlayerState.isGrounded != isGrounded)
        {
            PlayerState.isGrounded = isGrounded;

            if(isGrounded)
            {

                SetCanDash(true);

                audioManager.Play("Land");
            }
        }
    }

    static public void SetIsKnockedBack(bool isKnockedBack)
    {
        if(PlayerState.isKnockedBack != isKnockedBack)
        {
            PlayerState.isKnockedBack = isKnockedBack;
        }
    }

    static public void SetIsFacingRight(bool isFacingRight)
    {
        if(PlayerState.isFacingRight != isFacingRight)
        {
            PlayerState.isFacingRight = isFacingRight;

            //Placeholder while we have no player art
            if(isFacingRight)
            {
                playerAnimationController.SetFloat("Facing", 1f);
            }
            else
            {
                playerAnimationController.SetFloat("Facing", 0f);
            }
        }
    }

    static public void SetIsWallStuck(bool isWallStuck)
    {
        if(PlayerState.isWallStuck != isWallStuck)
        {
            PlayerState.isWallStuck = isWallStuck;
        }
    }

    static public void SetShouldWallStuck(bool shouldWallStuck)
    {
        if(PlayerState.shouldWallStuck != shouldWallStuck)
        {
            PlayerState.shouldWallStuck = shouldWallStuck;
        }
    }

    static public void SetIsOnMovingPlatform(bool isOnMovingPlatform)
    {
        if(PlayerState.isOnMovingPlatform != isOnMovingPlatform)
        {
            PlayerState.isOnMovingPlatform = isOnMovingPlatform;
        }
    }
    
    static public void SetCanSlide(bool canSlide)
    {
        if(PlayerState.canSlide != canSlide)
        {
            PlayerState.canSlide = canSlide;
        }
    }

    static public void SetCanDash(bool canDash)
    {
        if(PlayerState.canDash != canDash)
        {
            PlayerState.canDash = canDash;
        }
    }

    static public void SetCanParry(bool canParry)
    {
        if(PlayerState.canParry != canParry)
        {
            PlayerState.canParry = canParry;
        }
    }

    static public void SetCanAttack(bool canAttack)
    {
        if(PlayerState.canAttack != canAttack)
        {
            PlayerState.canAttack = canAttack;
        }
    }
    
    static public void SetCanMoveWallJump(bool canMoveWallJump)
    {
        if(PlayerState.canMoveWallJump != canMoveWallJump)
        {
            PlayerState.canMoveWallJump = canMoveWallJump;
        }
    }

    static public void SetIsRecovering(bool isRecovering)
    {
        if(PlayerState.isRecovering != isRecovering)
        {
            PlayerState.isRecovering = isRecovering;
        }
    }

    static public void SetIsWalking(bool isWalking)
    {
        if(PlayerState.isWalking != isWalking)
        {
            if(isWalking)
            {
                audioManager.SetPitch("Run", 0.4f);
            }
            else
            {
                audioManager.SetPitch("Run", 1f);
            }
            
            PlayerState.isWalking = isWalking;
        }
    }

    static public void SetIsLanded(bool isLanded)
    {
        if(PlayerState.isLanded != isLanded)
        {
            PlayerState.isLanded = isLanded;
        }
    }

    static public void SetIsLookingAround(bool isLookingAround)
    {
        if(PlayerState.isLookingAround != isLookingAround)
        {
            PlayerState.isLookingAround = isLookingAround;
        }
    }
    #endregion

    #region Getters
    static public State GetState()
    {
        return state;
    }

    static public bool GetIsGrounded()
    {
        return isGrounded;
    }

    static public bool GetIsKnockedBack()
    {
        return isKnockedBack;
    }

    static public bool GetIsFacingRight()
    {
        return isFacingRight;
    }

    static public bool GetIsWallStuck()
    {
        return isWallStuck;
    }

    static public bool GetShouldWallStuck()
    {
        return shouldWallStuck;
    }

    static public bool GetIsOnMovingPlatform()
    {
        return isOnMovingPlatform;
    }

    static public bool GetIsLanded()
    {
        return isLanded;
    }

    static public bool GetCanSlide()
    {
        return canSlide;
    }

    static public bool GetCanDash()
    {
        return canDash;
    }

    static public bool GetCanParry()
    {
        return canParry;
    }
    
    static public bool GetCanAttack()
    {
        return canAttack;
    }

    static public bool GetIsRecovering()
    {
        return isRecovering;
    }

    static public bool GetIsWalking()
    {
        return isWalking;
    }

    static public bool GetIsLookingAround()
    {
        return isLookingAround;
    }
    #endregion
    #endregion
}