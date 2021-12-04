using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimationController))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerCombat : MonoBehaviour
{
    #region  Attributes
    private List<SpriteRenderer> attackSprites = new List<SpriteRenderer>();

    private GameObject visualCue;

    private PlayerHealth playerHealth;

    private PlayerStats playerStats;

    private PlayerAnimationController playerAnimationController;

    private AudioManager audioManager;

    private ComboAttack currentAttack;

    private float parryDuration;
    [SerializeField] float parryDamage = 5f;
    [SerializeField] float parryCooldown = 0.35f;
    [SerializeField] float spamAttackCooldown = 0.33f;

    private float attackDamageMultiplier;

    private int attackDirection;

    private Coroutine attackCoroutine;
    private Coroutine attackContinueCoroutine;

    [SerializeField] List<ComboAttackSideways> comboAttacksSideways = new List<ComboAttackSideways>(3);
    [SerializeField] List<ComboAttackUp> comboAttacksUp = new List<ComboAttackUp>(3);
    [SerializeField] List<ComboAttackDown> comboAttacksDown = new List<ComboAttackDown>(3);

    private bool canContinueCombo;
    private bool canContinueComboAfterParry;
    private bool shouldFirstAttack;
    private bool isComboPerfect;
    private bool isFirstComboWindow;
    #endregion

    #region  MonoBehaviour Methods
    private void Awake() 
    {
        Transform playerSprite = transform.Find("PlayerSprite");

        attackSprites.Add(playerSprite.Find("Attack Sideways").GetComponent<SpriteRenderer>());

        attackSprites.Add(playerSprite.Find("Attack Up").GetComponent<SpriteRenderer>());

        attackSprites.Add(playerSprite.Find("Attack Down").GetComponent<SpriteRenderer>());

        visualCue = playerSprite.Find("Sparke").gameObject;

        playerAnimationController = GetComponent<PlayerAnimationController>();

        playerHealth = GetComponent<PlayerHealth>();

        playerStats = GetComponent<PlayerStats>();

        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        canContinueCombo = false;

        canContinueComboAfterParry = false;

        shouldFirstAttack = true;

        isComboPerfect = false;

        isFirstComboWindow = false;

        attackDirection = 0;

        SetPlayerStats();
    }
    #endregion

    #region Normal Methods  
    //Gets callled when the state changes to parrying, stops the current attack combo and coroutine and starts parrying
    public void Parry(bool onWall = false)
    {
        if(attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);

            PlayerState.SetCanAttack(true);

            visualCue.SetActive(false);
        }

        if(attackContinueCoroutine != null)
        {
            StopCoroutine(attackContinueCoroutine);

            if(!canContinueCombo)
            {
                StopCombo(false, true);
            }
        }

        StartCoroutine(ParryBehaviour(onWall));
    }

    //The start logic behind attacking, gets called when the state changes in PlayerState
    public void Attack(bool onWall = false)
    {
        if(!PlayerState.GetCanAttack())
        {
            return;
        }

        //CheckDirectionChange();
        
        StartCombo();
    }

    /*Checks if the direction of the attack changed, if yes it resets the visual cue, stops the attack coroutine 
    and sets the combo index to -1, after that it gets the new direction */
    private void CheckDirectionChange()
    {
        int newAttackDirection = GetNewAttackDirection();

        if (attackDirection != newAttackDirection && PlayerState.GetCanAttack())
        {
            visualCue.SetActive(false);

            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }
        }

        SetAttackDirection(newAttackDirection);
    }

    private void SetAttackDirection(int newAttackDirection)
    {
        attackDirection = newAttackDirection;

        //Sets the attack direction in the animator, 1 if it's up, -1 if it's down and 0 if it's sideways

        playerAnimationController.SetFloat("AttackDirection", attackDirection);
    }

    private int GetNewAttackDirection()
    {
        if(PlayerInput.GetAttackInput() > 0.69f)
        {
            return 1;
        }
        else if(PlayerInput.GetAttackInput() < -0.69f && !PlayerState.GetIsGrounded())
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }


    //If shouldFirstAttack is true when the combo is not stated, it starts the first attack
    private void StartCombo(bool onWall = false)
    {
        if(attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        if(onWall)
        {
            SetAttackDirection(1);

            attackCoroutine = StartCoroutine(AttackBehaviour(comboAttacksUp[0], onWall));  

            return;
        }

        if(attackDirection > 0)
        {
            attackCoroutine = StartCoroutine(AttackBehaviour(comboAttacksUp[0]));            
        }
        else if(attackDirection < 0)
        {
            attackCoroutine = StartCoroutine(AttackBehaviour(comboAttacksDown[0]));            
        }
        else
        {
            attackCoroutine = StartCoroutine(AttackBehaviour(comboAttacksSideways[0]));            
        }
    }

    //Stops the combo when necessary, by reseting the index and stoping the visual cue
    public void StopCombo(bool isDamaged = false, bool isParry = false)
    {
        if(isDamaged)
        {
            if(attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }

            shouldFirstAttack = true;

            PlayerState.SetCanAttack(true);

            canContinueCombo = false;

            canContinueComboAfterParry = false;

            visualCue.SetActive(false);

            return;
        }

        shouldFirstAttack = true;

        visualCue.SetActive(false);

        if(!isParry)
        {
            StartCombo();
        }
    }

    //Continues the combo by going trough a list of comboAttacks set in the inspector, and resests the coroutine and visual cue 
    private void ContinueCombo()
    {
        visualCue.SetActive(false);

        StopCoroutine(attackCoroutine);

        if(attackContinueCoroutine != null)
        {
            StopCoroutine(attackContinueCoroutine);
        }

        int comboIndex = 1;

        if(isComboPerfect)
        {
            comboIndex = 2;
        }

        canContinueCombo = false;

        canContinueComboAfterParry = false;

        //Checks the direction to see what attack type to do
        if(attackDirection > 0f)
        {
            foreach(ComboAttackUp attack in comboAttacksUp)
            {
                if(comboAttacksUp.IndexOf(attack) == comboIndex)
                {
                    attackCoroutine = StartCoroutine(AttackBehaviour(attack));
                }
            }
        }
        else if(attackDirection < 0f)
        {
            foreach(ComboAttackDown attack in comboAttacksDown)
            {
                if(comboAttacksDown.IndexOf(attack) == comboIndex)
                {
                    attackCoroutine = StartCoroutine(AttackBehaviour(attack));
                }
            }
        }
        else
        {
            foreach(ComboAttackSideways attack in comboAttacksSideways)
            {
                if(comboAttacksSideways.IndexOf(attack) == comboIndex)
                {
                    attackCoroutine = StartCoroutine(AttackBehaviour(attack));
                }
            }
        }
    }

    public void SuccessfulAttack(Vector2 collisionPossition, bool isWall = false, bool isEnemy = false)
    {
        playerHealth.SuccessfulAttack(collisionPossition, attackDirection, currentAttack.regenMultiplier, isWall);

        if(isEnemy)
        {
            currentAttack.hitEnemy = true;
        }
    }

    public void SuccessfulParry()
    {
        visualCue.GetComponent<SpriteRenderer>().color = Color.blue;

        StartCoroutine(VisualCueBehaviour());

        playerHealth.Regenerate(true);

        audioManager.Play("SuccessfulParry");
    }

    public void SetPlayerStats()
    {
        parryDuration = playerStats.GetParryDuration();

        attackDamageMultiplier = playerStats.GetAttackDamageMultiplier();
    }

    public float GetDamage(bool isParry = false)
    {
        if(isParry)
        {
            return parryDamage;
        }

        return 10;
    }

    public float GetAttackDirection()
    {
        return attackDirection;
    }

    public bool GetShouldFirstAttack()
    {
        return shouldFirstAttack;
    }
    #endregion

    #region Coroutines
    //The animation, state changes and combo stops of the parry
    private IEnumerator ParryBehaviour(bool onWall = false)
    {
        if(onWall)
        {   
            PlayerState.SetIsWallStuck(false);

            PlayerState.SetShouldWallStuck(true);

            PlayerState.SetState(PlayerState.State.WallSliding);

            playerAnimationController.SetFloat("OnWall", 1f);
        }
        else
        {
            playerAnimationController.SetFloat("OnWall", -1f);
        }

        playerAnimationController.SetTrigger("Parry");

        audioManager.Play("Parry");

        PlayerState.SetCanParry(false);

        visualCue.SetActive(false);

        yield return new WaitForSeconds(parryDuration);

        if(canContinueComboAfterParry)
        {
            visualCue.SetActive(true);

            canContinueComboAfterParry = false;

            attackContinueCoroutine = StartCoroutine(AttackContinueBehaviour(currentAttack.comboTime));
        }
        else
        {
            shouldFirstAttack = true;
        }

        if (!onWall)
        {
            PlayerState.SetState(PlayerState.State.Idle);
        }

        yield return new WaitForSeconds(parryCooldown);
        
        PlayerState.SetCanParry(true);
    }
    
    //Where all the attack really happends. Animations, states, combo stops
    private IEnumerator AttackBehaviour(ComboAttack attack, bool onWall = false)
    {
        foreach(SpriteRenderer sprite in attackSprites)
        {
            if(!shouldFirstAttack)
            {
                if(!isComboPerfect)
                {
                    sprite.color = Color.yellow;
                }
                else if(isComboPerfect)
                {
                    sprite.color = Color.red;
                }
            }
            else
            {
                sprite.color = Color.gray;
            }
        }

        canContinueCombo = false;

        currentAttack = attack;

        currentAttack.hitEnemy = false;

        if(onWall)
        {   
            canContinueComboAfterParry = false;

            PlayerState.SetIsWallStuck(false);

            PlayerState.SetShouldWallStuck(true);

            PlayerState.SetState(PlayerState.State.WallSliding);
        }

        playerAnimationController.SetTrigger("Attack");

        audioManager.Play("Attack");

        PlayerState.SetCanAttack(false);

        yield return new WaitForSeconds(spamAttackCooldown);

        PlayerState.SetCanAttack(true);

        //Stops the attack coroutine here if the player attacks while on the wall
        if(onWall)
        {
            yield break;
        }

        if(PlayerState.GetState() != PlayerState.State.Dashing)
        {
            PlayerState.SetState(PlayerState.State.Idle);
        }

        if(currentAttack.hitEnemy)
        {
            attackCoroutine = StartCoroutine(ComboCueBehaviour());
        }
    }

    private IEnumerator ComboCueBehaviour()
    {
        yield return new WaitForSeconds(currentAttack.timeNormalCombo);

        shouldFirstAttack = false;

        canContinueCombo = true;

        isComboPerfect = false;

        isFirstComboWindow = true;

        canContinueComboAfterParry = true;

        visualCue.SetActive(true);

        visualCue.GetComponent<SpriteRenderer>().color = Color.yellow;

        yield return new WaitForSeconds(currentAttack.timeNormalCombo);

        isComboPerfect = true;

        isFirstComboWindow = false;

        visualCue.GetComponent<SpriteRenderer>().color = Color.red;

        yield return new WaitForSeconds(currentAttack.timePerfectCombo);
        
        isComboPerfect = false;

        visualCue.GetComponent<SpriteRenderer>().color = Color.yellow;

        attackContinueCoroutine = StartCoroutine(AttackContinueBehaviour(currentAttack.comboTime));
    }

    private IEnumerator AttackContinueBehaviour(float comboTime)
    {
        if(isFirstComboWindow)
        {
            yield return new WaitForSeconds(currentAttack.timeNormalCombo);

            isComboPerfect = true;

            isFirstComboWindow = false;

            visualCue.GetComponent<SpriteRenderer>().color = Color.red;
        }

        if(isComboPerfect)
        {
            yield return new WaitForSeconds(currentAttack.timePerfectCombo);
        
            isComboPerfect = false;

            visualCue.GetComponent<SpriteRenderer>().color = Color.yellow;
        }

        yield return new WaitForSeconds(comboTime);

        visualCue.SetActive(false);

        canContinueCombo = false;
        
        canContinueComboAfterParry = false;

        shouldFirstAttack = true;
    }

    private IEnumerator VisualCueBehaviour()
    {
        visualCue.SetActive(true);

        yield return new WaitForSeconds(1f);

        visualCue.SetActive(false);
    }
    #endregion

    #region  Classes
    public class ComboAttack
    {
        [Tooltip("Time untill the combo window opens and becomes normal after perfect in seconds")]
        public float timeNormalCombo;
        [Tooltip("Time untill the combo window becomes perfect in seconds")]
        public float timePerfectCombo;
        [Tooltip("Time untill the combo window closes in seconds")]
        public float comboTime;
        public float damage;

        [Tooltip("How much the basic regen amount from the health script gets multiplied")]
        public int regenMultiplier;

        [HideInInspector] public bool hitEnemy;

        [Tooltip("Placeholder for showing what part of the combo the attack is")]
        public Color attackColour;
    }

    [System.Serializable]
    public class ComboAttackSideways : ComboAttack {}

    [System.Serializable]
    public class ComboAttackUp : ComboAttack {}

    [System.Serializable]
    public class ComboAttackDown : ComboAttack {}
    #endregion
}