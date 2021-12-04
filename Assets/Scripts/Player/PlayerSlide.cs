using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerParticlesController))]
public class PlayerSlide : MonoBehaviour
{
    #region Attributes
    private PlayerMovement playerMovement;
    private PlayerParticlesController playerParticles;
    private PlayerInput playerInput;
    private ControlOverlay controlOverlay;
    
    [SerializeField] float slideSpeed = 65f;
    [SerializeField] float slideTime = 0.3f;
    [SerializeField] float slideCooldown = 3f;

    private Coroutine slideCoroutine;
    private Coroutine dashCoroutine;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        playerInput = GetComponent<PlayerInput>();

        playerParticles = GetComponent<PlayerParticlesController>();

        controlOverlay = FindObjectOfType<ControlOverlay>();
    }
    #endregion

    #region  Normal Methods
    public void Slide()
    {
        if(slideCoroutine == null)
        {
            slideCoroutine = StartCoroutine(SlideBehaviour(slideTime));
        }
    }

    public bool ContinueSlide()
    {
        if(slideCoroutine != null && PlayerPhysicsCalculations.hitSlide)
        {
            StopCoroutine(slideCoroutine);

            slideCoroutine = StartCoroutine(SlideBehaviour(0.02f));

            return true;
        }

        return false;
    }

    public void Dash(bool wallSliding)
    {
        if(dashCoroutine == null)
        {
            dashCoroutine= StartCoroutine(DashBehaviour(wallSliding, slideTime));
        }
    }

    public bool ContinueDash()
    {
        if(dashCoroutine != null && PlayerPhysicsCalculations.hitSlide)
        {
            StopCoroutine(dashCoroutine);

            dashCoroutine = StartCoroutine(DashBehaviour(false, 0.02f));

            return true;
        }

        return false;
    }

    public float GetSlideSpeed()
    {
        return slideSpeed;
    }
    #endregion

    #region Coroutines
    private IEnumerator SlideBehaviour(float slideTime)
    {
        PlayerState.SetCanSlide(false);

        PlayerInput.SetIsRecievingInput(false);

        playerParticles.SlideParticles();

        PlayerInput.SetMoveInput(0f);

        PlayerMovement.StopMoving();

        PlayerMovement.AddVelocity(Vector2.right * PlayerMovement.facing * slideSpeed);

        yield return new WaitForSeconds(slideTime);

        if(ContinueSlide())
        {
            yield break;
        }

        PlayerInput.SetIsRecievingInput(true);

        StartCoroutine(SlideCooldownBehaviour());

        playerInput.MovePerformedGameplay();

        playerParticles.SlideParticles();

        playerMovement.Movement();

        controlOverlay.SetImageState(false,controlOverlay.dashImage);
    }

    private IEnumerator SlideCooldownBehaviour()
    {
        yield return new WaitForSeconds(slideCooldown);

        PlayerState.SetCanSlide(true);

        slideCoroutine = null;
    }

    private IEnumerator DashBehaviour(bool wallSliding, float slideTime)
    {
        PlayerInput.SetIsRecievingInput(false);

        playerParticles.DashParticles();

        PlayerInput.SetMoveInput(0f);

        PlayerMovement.StopMoving();

        if(wallSliding)
        {
            PlayerMovement.Flip();

            PlayerMovement.AddVelocity(Vector2.right * -PlayerMovement.facing * slideSpeed);
        }
        else
        {
            PlayerMovement.AddVelocity(Vector2.right * PlayerMovement.facing * slideSpeed);
        }

        yield return new WaitForSeconds(slideTime);

        if(ContinueDash())
        {
            yield break;
        }

        PlayerInput.SetIsRecievingInput(true);

        PlayerState.SetState(PlayerState.State.Idle);

        playerInput.MovePerformedGameplay();

        playerParticles.DashParticles();

        playerMovement.Movement();

        PlayerState.SetCanDash(false);

        controlOverlay.SetImageState(false,controlOverlay.dashImage);

        dashCoroutine = null;
    }
    #endregion
}
