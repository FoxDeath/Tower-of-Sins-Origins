using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerMovement : MonoBehaviour
{
    #region Attributes
    static private Rigidbody2D myRigidbody;

    private PlayerStats playerStats;

    private static SpriteRenderer spriteRenderer;

    private Coroutine jumpTimerCoroutine;

    static public float facing;
    [SerializeField] float gravity = 20f;
    [SerializeField] float maxFallSpeed = 70f;
    public float speed;
    private float jumpForce;
    private float jumpTime;
    #endregion
    
    #region MonoBehaviour Methods
    private void Awake() 
    {
        myRigidbody = GetComponent<Rigidbody2D>();

        playerStats = GetComponent<PlayerStats>();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    
    private void Start()
    {
        SetPlayerStats();

        facing = 0;
    }

    private void Update() 
    {
        Facing();
    }
    #endregion

    #region Normal Methods
    static public Vector2 GetVelocity()
    {
        return myRigidbody.velocity;
    }

    static private void SetVelocityX(float x)
    {
        myRigidbody.velocity = new Vector2(x, myRigidbody.velocity.y);
    }

    static private void SetVelocityY(float y)
    {
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, y);
    }

    private void Facing()
    {
        if(PlayerState.GetIsFacingRight())
        {
            facing = 1;
        }
        else
        {
            facing = -1;
        }
    }

    public void Gravity()
    {
        myRigidbody.velocity += Vector2.down * gravity;
    }

    public void StopGravity()
    {
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, 0f);
    }

    public void Movement()
    {
        if(PlayerState.GetState() != PlayerState.State.Sliding)
        {
            FlipPlayer();
        }

        myRigidbody.velocity = new Vector2(PlayerInput.GetMoveInput() * speed, myRigidbody.velocity.y);
    }

    public void VelocityClamp(float clampX = 0f, float clampY = 0f)
    {
        if(clampX == 0f)
        {
            clampX = speed;
        }

        if(clampY == 0f)
        {
            clampY = jumpForce;
        }

        if(PlayerState.GetIsWalking())
        {
            myRigidbody.velocity = new Vector2(Mathf.Clamp(myRigidbody.velocity.x, -clampX / 2f, clampX / 2f), Mathf.Clamp(myRigidbody.velocity.y, -clampY, clampY));
        }
        else
        {
            myRigidbody.velocity = new Vector2(Mathf.Clamp(myRigidbody.velocity.x, -clampX, clampX), Mathf.Clamp(myRigidbody.velocity.y, -clampY, clampY));
        }
    }

    public void JumpRelease()
    {
        if(jumpTimerCoroutine != null)
        {
            StopCoroutine(jumpTimerCoroutine);
        }

        StopJumping();
    }

    public void Jump()
    {
        if(jumpTimerCoroutine != null)
        {
            StopCoroutine(jumpTimerCoroutine);
        }

        myRigidbody.velocity += Vector2.up * jumpForce;

        jumpTimerCoroutine = StartCoroutine(JumpTimerBehaviour());
    }

    public void FlipPlayer()
    {
        if(PlayerState.GetIsWallStuck())
        {
            return;
        }

        if(!PlayerState.GetIsFacingRight() && PlayerInput.GetMoveInput() > 0f)
        {
            Flip();
        }
        else if(PlayerState.GetIsFacingRight() && PlayerInput.GetMoveInput() < 0f)
        {
            Flip();
        }
    }

    public static void Flip(bool onWall = false)
    {
        if((PlayerState.GetState() == PlayerState.State.Parrying || PlayerState.GetState() == PlayerState.State.Attacking)
        && !onWall)
        {
            PlayerState.SetState(PlayerState.State.Idle);
        }

        Vector3 theScale = spriteRenderer.transform.localScale;

        PlayerState.SetIsFacingRight(!PlayerState.GetIsFacingRight());

        theScale.x *= -1;
        
        spriteRenderer.transform.localScale = theScale;
    }

    static public void StopMoving(bool noInput = false)
    {
        if(PlayerInput.GetMoveInput() == 0f)
        {
            PlayerMovement.SetVelocityX(0f);
        }
        else if(noInput)
        {
            PlayerMovement.SetVelocityX(0f);
        }
    }

    static public void StopJumping(bool whenFalling = false)
    {
        if(PlayerMovement.GetVelocity().y > 0f)
        {
            PlayerMovement.SetVelocityY(0f);
        }
        else if(whenFalling)
        {
            PlayerMovement.SetVelocityY(0f);
        }
    }

    static public void AddVelocity(Vector2 velocity)
    {
        myRigidbody.velocity += velocity;
    }
    #endregion

    #region Getters
    public float GetJumpForce()
    {
        return jumpForce;
    }

    public float GetMaxFallSpeed()
    {
        return maxFallSpeed;
    }
    #endregion

    #region Setters
    public void SetPlayerStats()
    {
        speed = playerStats.GetMovementSpeed();

        jumpForce = playerStats.GetJumpForce();

        jumpTime = playerStats.GetJumpTime();
    }
    #endregion

    #region Coroutines
    private IEnumerator JumpTimerBehaviour()
    {
        yield return new WaitForSeconds(jumpTime);

        StopJumping();
    }

    public IEnumerator LandingTimerBehaviour()
    {
        PlayerState.SetIsLanded(true);

        yield return new WaitForFixedUpdate();
        
        yield return new WaitForFixedUpdate();

        PlayerState.SetIsLanded(false);
    }
    #endregion
}
