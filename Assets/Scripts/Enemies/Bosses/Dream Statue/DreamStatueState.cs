using UnityEngine;

public class DreamStatueState : EnemyState
{
    #region Attributes
    private bool isGrounded = false;
    #endregion

    #region MonoBehaviour Methods
    private void Update()
    {
        //Keeps track of when the enemy can and cannot attack.
        if(currentState != State.Attacking && !(astarAI as DreamStatueMovement).GetIsInChargeCycle()
        && !enemyCombat.GetIsInCycleAttackInProgress())
        {
            SetCanAttack(true);
        }
        else
        {
            SetCanAttack(false);
        }
    }
    #endregion

    #region Normal Methods
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
            case State.Stationary:
                if(currentState != State.Stationary)
                {
                    currentState = State.Stationary;

                    animController.SetTrigger(EnemyAnimationController.triggers.Idle);
                }
            break;

            case State.Dead:
                currentState = State.Dead;

                animController.SetTrigger(EnemyAnimationController.triggers.Death);
            break;

            case State.Attacking:
                if(currentState != State.Attacking && currentState != State.Charging
                && currentState != State.Teleporting)
                {
                    currentState = State.Attacking;

                    animController.SetTrigger(EnemyAnimationController.triggers.Attack);
                }
            break;

            case State.Charging:
                if(currentState != State.Charging && currentState != State.Attacking
                && currentState != State.Teleporting)
                {
                    currentState = State.Charging;

                    animController.SetTrigger(EnemyAnimationController.triggers.Charge);
                }
            break;

            case State.Following:
                if(GetIsGrounded() && currentState != State.Following
                && currentState != State.Teleporting && currentState != State.Attacking
                && currentState != State.Charging)
                {
                    currentState = State.Following;

                    animController.SetTrigger(EnemyAnimationController.triggers.DefaultMove);
                }
            break;

            case State.Teleporting:
                if(currentState != State.Teleporting && currentState != State.Attacking
                && currentState != State.Charging)
                {
                    currentState = State.Teleporting;

                    animController.SetTrigger(EnemyAnimationController.triggers.Teleport);
                }
            break;

            case State.Falling:
                if(!GetIsGrounded() && currentState != State.Falling
                && currentState != State.Teleporting && currentState != State.Attacking
                && currentState != State.Charging)
                {
                    currentState = State.Falling;

                    animController.SetTrigger(EnemyAnimationController.triggers.Fall);
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

            //So that an out of cycle attack doesn't trigger straight after a charge.
            StartCoroutine(enemyCombat.ResetOutOfCycleAttackCheck());
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
    #endregion

    #region Getters
    public bool GetIsGrounded()
    {
        return isGrounded;
    }
    #endregion
    #endregion

    #region Coroutines

    #endregion
}
