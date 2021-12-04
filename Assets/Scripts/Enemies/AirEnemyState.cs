using UnityEditor;
using UnityEngine;

public class AirEnemyState : EnemyState
{
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
            case State.Charging:
                if(currentState != State.Charging && currentState != State.KnockedBack
                && currentState != State.Attacking && currentState != State.Staggered)
                {
                    currentState = State.Charging;

                    animController.SetTrigger(EnemyAnimationController.triggers.Charge);
                }
            break;

            case State.Following:
                if(currentState != State.Following && currentState != State.Charging
                && currentState != State.KnockedBack && currentState != State.Attacking
                && currentState != State.Staggered && currentState != State.Retreating)
                {
                    currentState = State.Following;

                    animController.SetTrigger(EnemyAnimationController.triggers.DefaultMove);
                }
            break;

            case State.Patroling:
                if(currentState != State.Patroling && currentState != State.Charging
                && currentState != State.KnockedBack && currentState != State.Attacking
                && currentState != State.Staggered)
                {
                    currentState = State.Patroling;

                    animController.SetTrigger(EnemyAnimationController.triggers.DefaultMove);
                }
            break;

            case State.Stationary:
                if(currentState != State.Stationary && !astarAI.GetIsTherePriorityDestination())
                {
                    currentState = State.Stationary;

                    animController.SetTrigger(EnemyAnimationController.triggers.Idle);
                }
            break;

            case State.Dead:
                currentState = State.Dead;
                astarAI.ZeroOutCommitedDirection();

                astarAI.ZeroOutVelocity();

                astarAI.SetIsUsingGravity(true);

                StartCoroutine(astarAI.KnockedBackBehaviour());

                astarAI.UnfreezeRigidbodyRotation();

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
                && currentState != State.KnockedBack && currentState != State.Patroling
                && currentState != State.Attacking && currentState != State.Staggered)
                {
                    currentState = State.Sleeping;

                    animController.SetTrigger(EnemyAnimationController.triggers.Idle);
                }
            break;

            case State.Attacking:
                if(currentState != State.Attacking && currentState != State.Charging
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
    #endregion
    #endregion
}
