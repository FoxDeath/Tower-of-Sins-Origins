using UnityEngine;

public class PlayerParticlesController : MonoBehaviour
{
    #region Attributes
    private ParticleSystem slideParticles;
    private ParticleSystem fallHitParticles;
    private ParticleSystem fallParticles;
    private ParticleSystem moveParticles;
    private ParticleSystem dashParticles;
    private ParticleSystem wallJumpParticles;
    #endregion 

    #region MonoBehaviour Methods
    private void Awake()
    {
        slideParticles = GameObject.Find("SlideParticles").GetComponent<ParticleSystem>();

        fallHitParticles = GameObject.Find("FallHitParticles").GetComponent<ParticleSystem>();

        fallParticles = GameObject.Find("FallParticles").GetComponent<ParticleSystem>();

        moveParticles = GameObject.Find("MoveParticles").GetComponent<ParticleSystem>();

        dashParticles = GameObject.Find("DashParticles").GetComponent<ParticleSystem>();

        wallJumpParticles = GameObject.Find("WallJumpParticles").GetComponent<ParticleSystem>();
    }
    private void Update()
    {
        FallHitParticles();

        FallParticles();

        MoveParticles();
    }
    #endregion

    #region Normal Methods
    public void SlideParticles()
    {
        if(PlayerState.GetState() == PlayerState.State.Sliding)
        {
            slideParticles.Play();
        }
        else
        {
            slideParticles.Stop();
        }
    }

    public void DashParticles()
    {
        if(PlayerState.GetState() == PlayerState.State.Dashing)
        {
            dashParticles.Play();
        }
        else
        {
            dashParticles.Stop();
        }
    }

    public void WallJumpParticles()
    {
        if(PlayerState.GetState() == PlayerState.State.WallJumping)
        {
            wallJumpParticles.Play();
        }
        else
        {
            wallJumpParticles.Stop();
        }
    }

    private void FallHitParticles()
    {
        if(PlayerState.GetIsLanded())
        {       
            fallHitParticles.Play();         
        }
        else
        {
            fallHitParticles.Stop();
        }
    }

    private void FallParticles()
    {
        if(PlayerState.GetState() == PlayerState.State.Falling || PlayerState.GetState() == PlayerState.State.WallJumpFalling)
        {
            fallParticles.Play();
        }
        else
        {
            fallParticles.Stop();
        }
    }

    private void MoveParticles()
    {
        if(PlayerState.GetState() == PlayerState.State.Moving && !PlayerState.GetIsWalking())
        {
            moveParticles.Play();
        }
        else
        {
            moveParticles.Stop();
        }
    }
    #endregion
}
