using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AstarAI))]
[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(EnemyAnimationController))]
public abstract class EnemyState : MonoBehaviour
{
    #region Attributes
    //All the states of all the enemies are collected here. This makes state handling a lot easier across the board.
    public enum State
    {
        Stationary,
        Jumping,
        Patroling,
        Falling,
        Charging,
        Following,
        Dead,
        KnockedBack,
        Staggered,
        //Sleeping has been completely abandoned. It hasn't been tested if it works together with new mechanics,
        //so if we ever decide to bring it back, we gotta test it thoroughly.
        Sleeping,
        CirclingPlatrform,
        Attacking,
        Teleporting,
        Retreating,
    }
    
    protected State currentState = State.Stationary;

    protected State previousState = State.Stationary;

    protected EnemyHealth enemyHealth;
    
    protected EnemyCombat enemyCombat;
    
    protected AstarAI astarAI;

    protected EnemyAnimationController animController;

    protected bool canCharge = true;
    protected bool canAttack = false;
    protected bool canDealDamage = true;
    protected bool isWaitingToSleep = false;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        astarAI = GetComponent<AstarAI>();

        enemyHealth = GetComponent<EnemyHealth>();

        if(GetComponent<EnemyCombat>() != null)
        {
            enemyCombat = GetComponent<EnemyCombat>();
        }

        animController = GetComponent<EnemyAnimationController>();
    }

    // //This shows the enemy's current state in the editor.
    // private void OnDrawGizmos()
    // {
    //     Handles.Label(new Vector3(transform.position.x, transform.position.y + 5, transform.position.z), currentState.ToString());
    // }
    #endregion

    #region Normal Methods
    public abstract bool SetState(State state);

    #region Setters
    public void SetCanCharge(bool state)
    {
        if(canCharge != state)
        {
            canCharge = state;
        }
    }
    
    public void SetCanAttack(bool state)
    {
        if(canAttack != state)
        {
            canAttack = state;
        }
    }
    
    public void SetCanDealDamage(bool state)
    {
        if(canDealDamage != state)
        {
            canDealDamage = state;
        }
    }

    public void SetIsWaitingToSleep(bool state)
    {
        if(isWaitingToSleep != state)
        {
            isWaitingToSleep = state;
        }
    }
    #endregion

    #region Getters
    public State GetState()
    {
        return currentState;
    }
    
    public State GetPreviousState()
    {
        return previousState;
    }

    public bool GetCanCharge()
    {
        return canCharge;
    }
    
    public bool GetCanAttack()
    {
        return canAttack;
    }
    
    public bool GetCanDealDamage()
    {
        return canDealDamage;
    }

    public bool GetIsWaitingToSleep()
    {
        return isWaitingToSleep;
    }
    #endregion

    #endregion
}
