using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(EnemyState))]
[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(AstarAI))]
[RequireComponent(typeof(EnemyAnimationController))]
public class EnemyCombat : MonoBehaviour
{
    #region Attributes
    protected dynamic enemyState;

    private EnemyHealth enemyHealth;

    protected AstarAI astarAI;

    private EnemyAnimationController animController;

    private Coroutine OutOfCycleAttackCheckCoroutine;

    [SerializeField] List<Phase> phases = new List<Phase>();

    private Phase currentPhase;

    private Attack currentAttack;

    [Tooltip("The duration between an 'out of cycle' attack and the next round of range checks for these kinds of attacks.")]
    [SerializeField] float timeBetweenOutOfCycleAttacks = 1f;

    [Tooltip("The duration between reseting the 'out of cycle' attack range checks (ResetOutOfCycleAttackCheck() - happens automatically after 'in cycle' attacks and other, enemy type specific situations) and the next 'out of cycle' attack.")]
    [SerializeField] float outOfCycleAttackResetDuration = 0.5f;

    private int currentAttackNumber = 0;
    private int currentMeleeAttackNumber = 0;

    [Tooltip("If the attacks in the cycle should follow each other in order or randomly.")]
    [SerializeField] bool isInRandomOrder = false;
    protected bool isAuxilieryBehaviourDone = false;
    private bool isInCycleAttackInProgress = false;
    #endregion

    #region MonoBehaviour Methods
    protected void Awake()
    {
        enemyState = GetComponent<EnemyState>();

        enemyHealth = GetComponent<EnemyHealth>();

        astarAI = GetComponent<AstarAI>();

        animController = GetComponent<EnemyAnimationController>();
    }

    protected void Start()
    {
        SetUpPhases();
        
        OutOfCycleAttackCheckCoroutine = StartCoroutine(OutOfCycleAttackCheck());
    }
    
    protected void Update()
    {
        if(currentPhase == null)
        {
            return;
        }
        
        //Changes to next phase when applicable.
        if(phases.Count > phases.IndexOf(currentPhase) + 1)
        {
            if(enemyHealth.GetHealthPercentage() <= phases[phases.IndexOf(currentPhase) + 1].triggerHealthPercentage)
            {
                currentPhase = phases[phases.IndexOf(currentPhase) + 1];

                RestartAttackCycle();
            }
        }
    }
    #endregion

    #region Normal Methods
    //This method determines what the next attack in the cycle is.
    private Attack GetNextAttackInCycle()
    {
        if(isInRandomOrder)
        {
            return new Attack(currentPhase.attacksInCycle[UnityEngine.Random.Range(0, currentPhase.attacksInCycle.Count)]);
        }
        else
        {
            currentAttackNumber++;
            
            if(currentAttackNumber >= currentPhase.attacksInCycle.Count)
            {
                currentAttackNumber = 0;
            }

            return new Attack(currentPhase.attacksInCycle[currentAttackNumber]);
        }
    }

    //All attacks are distributed into three groups. In cycle, out of cycle an the attacks that are done whenever possible.
    private void SetUpPhases()
    {
        if(phases.Count > 0)
        {
            currentPhase = phases[0];
            
            foreach(Phase phase in phases)
            {
                foreach(Attack attack in phase.allAttacks)
                {
                    if(attack.isInCycle)
                    {
                        phase.attacksInCycle.Add(attack);
                    }
                    else
                    {
                        phase.attacksOutOfCycle.Add(attack);
                    }
                }

                phase.attacksOutOfCycle = phase.attacksOutOfCycle.OrderByDescending(attack => attack.range).ToList();
            }

            RestartAttackCycle();
        }
        else
        {
            currentPhase = null;
        }
    }

    //Resets the current attack to the phase's first attack. Used on phase change.
    private void RestartAttackCycle()
    {
        StopAllCoroutines();
        
        if(enemyState.GetState() == EnemyState.State.Attacking)
        {
            enemyState.SetState(EnemyState.State.Stationary);
        }

        if(isInCycleAttackInProgress)
        {
            isInCycleAttackInProgress = false;
        }

        if(currentPhase.attacksInCycle.Count > 0)
        {
            StartCoroutine(CommenceInCycleAttack(new Attack(currentPhase.attacksInCycle[0])));
        }

        StartCoroutine(ResetOutOfCycleAttackCheck());
    }

    #region Getters and Setters
    public void FinishAuxiliaryBehaviour()
    {
        isAuxilieryBehaviourDone = true;
    }

    public void ChangeAttackDamage(float percentage)
    {
        foreach(Phase phase in phases)
        {
            foreach(Attack attack in phase.allAttacks)
            {
                attack.damage *= percentage;
            }
        }
    }
    
    public void ChangeAttackRange(float percentage)
    {
        foreach(Phase phase in phases)
        {
            foreach(Attack attack in phase.allAttacks)
            {
                attack.range *= percentage;
            }
        }
    }
    
    public void ChangeAttackBuildUpDuration(float percentage)
    {
        foreach(Phase phase in phases)
        {
            foreach(Attack attack in phase.allAttacks)
            {
                attack.buildUpDuration *= percentage;
            }
        }
    }
    
    public void ChangeAttackParriable(bool state)
    {
        foreach(Phase phase in phases)
        {
            foreach(Attack attack in phase.allAttacks)
            {
                attack.isParriable = state;
            }
        }
    }

    public void SetCurrentParriable(bool state)
    {
        if(currentAttack.isParriable != state)
        {
            currentAttack.isParriable = state;
        }
    }

    public bool GetIsInCycleAttackInProgress()
    {
        return isInCycleAttackInProgress;
    }

    public float GetCurrentDamage()
    {
        return currentAttack.damage;
    }
    
    public bool GetCurrentParriable()
    {
        return currentAttack.isParriable;
    }

    public float GetPhaseTriggerHealth(int phase)
    {
        if(phases.Count - 1 >= phase)
        {
            return phases[phase].triggerHealthPercentage;
        }
        else
        {
            return -1;
        }
    }

    public Attack GetAttackFromCurrentPhase(int attackNr)
    {
        if(attackNr < currentPhase.allAttacks.Count)
        {
            return currentPhase.allAttacks[attackNr];
        }
        else
        {
            return null;
        }
    }

    public Attack GetAttackFromAllPhases(int phaseNr, int attackNr) 
    { 
        if(phaseNr < phases.Count && attackNr < phases[phaseNr].allAttacks.Count) 
        { 
            return phases[phaseNr].allAttacks[attackNr]; 
        } 
        else 
        { 
            return null; 
        } 
    }
    #endregion
    #endregion

    #region Coroutines
    //If any special behaviour takes place before an attack, this method is overwritten in a sub class.
    protected virtual IEnumerator AuxiliaryBehaviourBeforeCombat(Attack currentAttack)
    {
        FinishAuxiliaryBehaviour();

        yield return null;
    }

    //If any special behaviour takes place during an attack, this method is overwritten in a sub class.
    protected virtual IEnumerator AuxiliaryBehaviourDuringCombat(Attack currentAttack)
    {
        FinishAuxiliaryBehaviour();

        yield return null;
    }

    //This is the only way attacks are started in and out of cycle.
    public IEnumerator CommenceInCycleAttack(Attack attack)
    {
        if(attack != null && attack.isInCycle)
        {
            yield return new WaitUntil(() => (!isInCycleAttackInProgress));

            yield return new WaitForSeconds(attack.buildUpDuration);

            yield return new WaitUntil(() => enemyState.GetCanAttack() && astarAI.IsPlayerInRange(attack.range));

            isInCycleAttackInProgress = true;

            StartCoroutine(AttackBehaviour(attack));
        }
    }

    //Initiates and attack.
    private IEnumerator AttackBehaviour(Attack attack)
    {
        //CurrentAttack gets updated because some of the getters and setters need to know which is the current attack.
        currentAttack = attack;

        //Any unique additional behaviour that is meant to happen before the attack is called here.
        StartCoroutine(AuxiliaryBehaviourBeforeCombat(attack));
        
        yield return new WaitUntil(() => isAuxilieryBehaviourDone);

        isAuxilieryBehaviourDone = false;

        //Puts the animator in the right state for the attack.
        animController.SetFloat(EnemyAnimationController.floats.AttackNumber, attack.animationNumber);

        enemyState.SetState(EnemyState.State.Stationary);
        
        yield return new WaitUntil(() => enemyState.SetState(EnemyState.State.Attacking));

        //Any unique additional behaviour that is meant to happen during the attack is called here.
        StartCoroutine(AuxiliaryBehaviourDuringCombat(attack));

        yield return new WaitUntil(() => isAuxilieryBehaviourDone);

        isAuxilieryBehaviourDone = false;

        //Waits until attack finishes.
        yield return new WaitForSeconds(attack.attackDuration);
        
        //If the attack was in cycle, it looks for the next one in the cycle.
        if(attack.isInCycle)
        {
            isInCycleAttackInProgress = false;

            StartCoroutine(CommenceInCycleAttack(GetNextAttackInCycle()));

            StartCoroutine(ResetOutOfCycleAttackCheck());
        }

        //Stops the attack animation.
        if(enemyState.GetState() != EnemyState.State.Staggered && enemyState.GetState() != EnemyState.State.KnockedBack)
        {
            enemyState.SetState(EnemyState.State.Stationary);
        }
    }
    
    public IEnumerator OutOfCycleAttackCheck()
    {
        List<Attack> possibleAttacks = currentPhase.attacksOutOfCycle.Where(attack => astarAI.IsPlayerInRange(attack.range) && attack.range > 0f).ToList();
        
        if(possibleAttacks.Count > 0 && enemyState.GetCanAttack())
        {
            StartCoroutine(AttackBehaviour(possibleAttacks[0]));

            yield return new WaitForSeconds(timeBetweenOutOfCycleAttacks);
        }
        else
        {
            yield return new WaitForEndOfFrame();
        }

        OutOfCycleAttackCheckCoroutine = StartCoroutine(OutOfCycleAttackCheck());
    }

    public IEnumerator ResetOutOfCycleAttackCheck()
    {   
        if(OutOfCycleAttackCheckCoroutine != null)
        {
            StopCoroutine(OutOfCycleAttackCheckCoroutine);
        }

        yield return new WaitForSeconds(outOfCycleAttackResetDuration);

        OutOfCycleAttackCheckCoroutine = StartCoroutine(OutOfCycleAttackCheck());
    }
    #endregion

    //The attack helper class.
    [System.Serializable]
    public class Attack
    {
        [Tooltip("Naming Convention: Start with P[number of the phase]A[number of the attack].")]
        public string name;

        [Tooltip("The number of the corresponding attack animation in the Animator Blend Tree.")]
        public int animationNumber = 0;

        [Tooltip("How close the player has to be for the attack to be triggered. If the attack is not part of the cycle and this is set to 0, then it will never trigger automatically.")]
        public float range = 13f;
        [Tooltip("How much damage does the attack inflict on the player.")]
        public float damage = 20f;
        [Tooltip("How much time has to pass between the end of the previous attack and the start of this one.")]
        public float buildUpDuration = 1f;
        [Tooltip("How long the attack animation lasts.")]
        public float attackDuration = 1f;

        [Tooltip("If the player can parry it or not.")]
        public bool isParriable = true;
        [Tooltip("If the attack should automatically be triggered as part of the cycle or not. If this is false, the attack will trigger whenever it's in range and the if the 'out of timer delay' allows.")]
        public bool isInCycle = true;

        public Attack(Attack attack)
        {
            this.name = attack.name;
            
            this.animationNumber = attack.animationNumber;

            if(range != 0f)
            {
                this.range = attack.range;
            }
            else
            {
                this.range = float.MaxValue;
            }

            this.damage = attack.damage;

            this.buildUpDuration = attack.buildUpDuration;

            this.attackDuration = attack.attackDuration;

            this.isParriable = attack.isParriable;

            this.isInCycle = attack.isInCycle;
        }
    }

    //The phase helper class.
    [System.Serializable]
    public class Phase
    {
        public string name;

        [Tooltip("What percentage of the enemy's health does it have to reach for this phase to start.")]
        public float triggerHealthPercentage = 100f;

        public List<Attack> allAttacks = new List<Attack>();
        [HideInInspector] public List<Attack> attacksInCycle = new List<Attack>();
        [HideInInspector] public List<Attack> attacksOutOfCycle = new List<Attack>();
    }
}
