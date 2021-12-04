using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerPhysicsCalculations : MonoBehaviour
{
    #region Attributes
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;

    private PlayerInput playerInput;

    private CapsuleCollider2D playerCollider;

    static public RaycastHit2D hitV, hitH, hitHangUp, hitHangDown, hitSlide;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        playerCollider = GetComponent<CapsuleCollider2D>();

        playerInput = GetComponent<PlayerInput>();
    }

    private void FixedUpdate()
    {
        RayCastCalculation(out hitV, out hitH, out hitHangUp, out hitHangDown, out hitSlide);

        ColiderCalculation(hitV, hitH);
    }
    #endregion

    #region Normal Methods
    private void RayCastCalculation(out RaycastHit2D hitV,  out RaycastHit2D hitH, out RaycastHit2D hitHangUp, out RaycastHit2D hitHangDown, out RaycastHit2D hitSlide)
    {
        hitV = Physics2D.BoxCast(playerCollider.bounds.center, new Vector2(3.5f, 1f), 0f, Vector2.down, playerCollider.bounds.size.y - 4.8f * 0.5f, groundLayer);
        
        hitH = Physics2D.BoxCast(playerCollider.bounds.center, new Vector2(1f, 0.5f), 0f, Vector2.right * PlayerMovement.facing, playerCollider.bounds.size.x - 4f * 0.5f, wallLayer);
        
        hitHangUp = Physics2D.Raycast(transform.position + new Vector3(0f, 4f), new Vector2(1f, 0f) * PlayerMovement.facing, 4f, groundLayer);
        
        hitHangDown = Physics2D.Raycast(transform.position + new Vector3(0f, 2.5f), new Vector2(1f, 0f) * PlayerMovement.facing, 4f, groundLayer);
        
        hitSlide = Physics2D.Raycast(transform.position + new Vector3(0f, -0.4f), new Vector2(0f, 1f), 3.3f, wallLayer);    }

    private void ColiderCalculation(RaycastHit2D hitV, RaycastHit2D hitH)
    {
        if(hitV.collider)
        {
            PlayerState.SetIsGrounded(true);

            PlayerState.SetShouldWallStuck(true);

            if(PlayerState.GetState() == PlayerState.State.Falling || PlayerState.GetState() == PlayerState.State.WallJumpFalling
            || PlayerState.GetState() == PlayerState.State.WallSliding)
            {
                playerInput.MovePerformedGameplay();
            }
        }
        else
        {
            PlayerState.SetIsGrounded(false);

            if(PlayerState.GetState() != PlayerState.State.WallSliding && PlayerState.GetState() != PlayerState.State.Hanging
            && PlayerState.GetState() != PlayerState.State.Parrying && PlayerState.GetState() != PlayerState.State.Attacking
            && PlayerState.GetState() != PlayerState.State.Staggered && PlayerState.GetState() != PlayerState.State.Dashing
            && PlayerState.GetState() != PlayerState.State.Sliding)
            {
                PlayerState.SetState(PlayerState.State.Falling);
            }
        }

        if(!hitV.collider && hitH.collider)
        {
            if((int)(hitH.normal.x + PlayerInput.GetMoveInput()) == 0f || PlayerState.GetState() == PlayerState.State.WallJumping
            || PlayerState.GetState() == PlayerState.State.WallJumpFalling || PlayerState.GetState() == PlayerState.State.Dashing)
            {
                PlayerState.SetState(PlayerState.State.WallSliding);
            }
        }

        if(!hitV.collider && !hitH.collider)
        {
            PlayerState.SetShouldWallStuck(true);
            
            PlayerState.SetIsGrounded(false);
            
            if(PlayerState.GetState() != PlayerState.State.Parrying && PlayerState.GetState() != PlayerState.State.Attacking
            && PlayerState.GetState() != PlayerState.State.Hanging && PlayerState.GetState() != PlayerState.State.Staggered
            && PlayerState.GetState() != PlayerState.State.Dashing && PlayerState.GetState() != PlayerState.State.Sliding)
            {
                PlayerState.SetState(PlayerState.State.Falling);
            }
        }

        if(!hitHangUp && hitHangDown)
        {
            PlayerState.SetState(PlayerState.State.Hanging);
        }
        else
        {
            if(PlayerState.GetState() == PlayerState.State.Hanging)
            {
                PlayerState.SetState(PlayerState.State.Idle);
            }

            if(!PlayerState.GetIsOnMovingPlatform())
            {
                transform.SetParent(null);
            }
        }
    }
}
    #endregion
