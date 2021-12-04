using UnityEditor;
using UnityEngine;

public class GroundEnemyState : EnemyState
{
    #region Attributes
    private bool isGrounded = false;
    private bool canJump = true;
    #endregion

    #region MonoBehaviour Methods
    private void Update()
    {
        //Keeps track of when the enemy can and cannot attack.
        if(currentState == State.Following || (currentState == State.Stationary && astarAI.IsPlayerInRange()))
        {
            SetCanAttack(true);
        }
        else
        {
            SetCanAttack(false);
        }
    }
    #endregion

    #region Normal Menthods
    #region Setters
    public override bool SetState(State state)
    {
        State tempPrevState = currentState;

        if(currentState == State.Dead)
        {
            return false;
        }

        switch(state)
        {
            case State.Patroling:
                if(GetIsGrounded() && currentState != State.Patroling
                && currentState != State.Charging && currentState != State.Jumping
                && currentState != State.Falling && currentState != State.KnockedBack
                && currentState != State.Attacking && currentState != State.Retreating
                && currentState != State.Staggered && currentState != State.Jumping)
                {
                    currentState = State.Patroling;

                    animController.SetTrigger(EnemyAnimationController.triggers.DefaultMove);
                }
            break;

            case State.Jumping:
                if(GetIsGrounded() && currentState != State.Jumping
                && currentState != State.Falling && currentState != State.Charging
                && currentState != State.KnockedBack && currentState != State.Attacking
                && currentState != State.Staggered && currentState != State.Jumping)
                {
                    currentState = State.Jumping;

                    animController.SetTrigger(EnemyAnimationController.triggers.Jump);
                }
            break;

            case State.Stationary:
                if(GetIsGrounded() && currentState != State.Stationary
                && currentState != State.Jumping && !astarAI.GetIsTherePriorityDestination())
                {
                    currentState = State.Stationary;

                    animController.SetTrigger(EnemyAnimationController.triggers.Idle);
                }
            break;

            case State.Falling:
                if((!GetIsGrounded() && currentState != State.Falling)
                && currentState != State.CirclingPlatrform)
                {
                    currentState = State.Falling;

                    animController.SetTrigger(EnemyAnimationController.triggers.Fall);
                }
            break;

            case State.Charging:
                if(GetIsGrounded() && GetCanCharge()
                && currentState != State.Charging && currentState != State.Jumping
                && currentState != State.Falling && currentState !=  State.KnockedBack
                && currentState != State.Attacking && currentState != State.Staggered
                && currentState != State.Jumping)
                {
                    currentState = State.Charging;

                    animController.SetTrigger(EnemyAnimationController.triggers.Charge);
                }
            break;

            case State.Following:
                if(GetIsGrounded() && currentState != State.Following
                && currentState != State.Jumping && currentState != State.Falling
                && currentState != State.Charging && currentState !=  State.KnockedBack
                && currentState != State.Attacking && currentState != State.Staggered
                && currentState != State.Jumping)
                {
                    currentState = State.Following;

                    animController.SetTrigger(EnemyAnimationController.triggers.DefaultMove);
                }
            break;

            case State.Dead:
                currentState = State.Dead;

                animController.SetTrigger(EnemyAnimationController.triggers.Death);
            break;

            case State.KnockedBack:
                if(currentState != State.KnockedBack)
                {
                    currentState = State.KnockedBack;

                    animController.SetTrigger(EnemyAnimationController.triggers.Idle);
                }
            break;

            case State.Staggered:
                if(currentState != State.Staggered)
                {
                    currentState = State.Staggered;

                    animController.SetTrigger(EnemyAnimationController.triggers.Idle);
                }
            break;

            case State.Sleeping:
                if(currentState != State.Sleeping && currentState != State.Charging
                && currentState != State.KnockedBack && currentState != State.Staggered
                && currentState != State.Jumping && currentState != State.Patroling
                && currentState != State.Attacking && currentState != State.Following
                && currentState != State.Jumping)
                {
                    currentState = State.Sleeping;

                    animController.SetTrigger(EnemyAnimationController.triggers.Idle);
                }
            break;

            case State.CirclingPlatrform:
                if(currentState != State.CirclingPlatrform && currentState != State.Charging
                && currentState != State.Following && currentState != State.Jumping
                && currentState != State.Patroling && currentState != State.KnockedBack
                && currentState != State.Attacking && currentState != State.Staggered)
                {
                    currentState = State.CirclingPlatrform;

                    animController.SetTrigger(EnemyAnimationController.triggers.DefaultMove);
                }
            break;

            case State.Attacking:
                if(currentState != State.Attacking && currentState != State.Charging
                && currentState != State.Jumping && currentState != State.CirclingPlatrform
                && currentState != State.Falling && currentState != State.Jumping
                && currentState != State.Sleeping && currentState != State.KnockedBack
                && currentState != State.Staggered)
                {
                    currentState = State.Attacking;

                    animController.SetTrigger(EnemyAnimationController.triggers.Attack);
                }
            break;

            case State.Retreating:
                if(currentState != State.Retreating && currentState != State.Charging
                && currentState != State.KnockedBack && currentState != State.Attacking
                && currentState != State.Sleeping && currentState != State.Falling
                && currentState != State.Jumping && currentState != State.CirclingPlatrform
                && currentState != State.Staggered)
                {
                    currentState = State.Retreating;

                    animController.SetTrigger(EnemyAnimationController.triggers.DefaultMove);
                }
            break;

            default:
            break;
        }

        if(tempPrevState != currentState)
        {
            previousState = tempPrevState;
        }

        if(previousState == State.Charging && currentState != State.KnockedBack)
        {
            astarAI.ZeroOutCommitedDirection();
        }

        if(previousState != currentState)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetIsGrounded(bool state)
    {
        if(isGrounded != state)
        {
            isGrounded = state;
        }
    }

    public void SetCanJump(bool state)
    {
        if(canJump != state)
        {
            canJump = state;
        }
    }
    #endregion

    #region Getters
    public bool GetIsGrounded()
    {
        return isGrounded;
    }

    public bool GetCanJump()
    {
        return canJump;
    }
    #endregion
    #endregion
}
