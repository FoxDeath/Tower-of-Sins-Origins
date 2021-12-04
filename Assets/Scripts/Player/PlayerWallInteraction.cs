using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerWallInteraction : MonoBehaviour
{
    #region Attributes
    private PlayerMovement playerMovement;

    private GameObject currentPlatform;

    [SerializeField] float wallSlideSpeed = 10f;
    [SerializeField] float wallJumpTime = 0.29f;

    [SerializeField] Vector2 wallJumpSpeed = new Vector2(30f, 45f);

    [SerializeField] bool upgradedWallJump;

    private Coroutine wallStuckCoroutine;
    #endregion

    #region  MonoBehaviour Methods
    private void Awake() 
    {
        playerMovement = GetComponent<PlayerMovement>();    
    }
    #endregion

    #region Normal Methods
    public void WallJump()
    {
        StopAllCoroutines();

        StartCoroutine(WallJumpBehaviour());
    }

    public void WallStuck(bool isAttacking = false)
    {
        wallStuckCoroutine = StartCoroutine(WallStuckBehaviour(isAttacking));
    }

    public void StopWallStuck()
    {
        if(wallStuckCoroutine != null)
        {
            StopCoroutine(wallStuckCoroutine);
        }

        PlayerState.SetIsWallStuck(false);
    }

    public void WallSlide()
    {
        PlayerMovement.StopJumping();

        if(PlayerMovement.GetVelocity().y < wallSlideSpeed)
        {
            PlayerMovement.AddVelocity(Vector2.down * wallSlideSpeed * Time.deltaTime);
        }
    }

    public Vector2 GetWallJumpSpeed()
    {
        return wallJumpSpeed;
    }
    #endregion

    #region Coroutines
    private IEnumerator WallStuckBehaviour(bool isAttacking = false)
    {
        if(PlayerInput.GetMoveInput() != 0f)
        {
            PlayerInput.SetMoveInput(0f);
        }

        PlayerState.SetIsWallStuck(true);

        PlayerState.SetShouldWallStuck(false);

        if(!isAttacking)
        {
            PlayerMovement.StopJumping(true);
        }

        PlayerMovement.StopMoving(true);

        yield return new WaitForSeconds(0.5f);
        
        PlayerState.SetIsWallStuck(false);
    }

    private IEnumerator WallJumpBehaviour()
    {
        StopCoroutine(wallStuckCoroutine);

        PlayerState.SetCanMoveWallJump(false);
        
        PlayerState.SetIsWallStuck(false);

        PlayerMovement.Flip();

        PlayerMovement.StopJumping(true);
        
        PlayerMovement.AddVelocity(new Vector2(-PlayerMovement.facing * wallJumpSpeed.x, wallJumpSpeed.y));

        if(upgradedWallJump)
        {
            StartCoroutine(UpgradedWallJumpBehaviour());
        }

        yield return new WaitForSeconds(wallJumpTime);

        PlayerState.SetCanMoveWallJump(false);

        PlayerMovement.StopJumping();
    }

    private IEnumerator UpgradedWallJumpBehaviour()
    {
        yield return new WaitForSeconds(wallJumpTime / 2);

        PlayerState.SetCanMoveWallJump(true);
    }
    #endregion
}
