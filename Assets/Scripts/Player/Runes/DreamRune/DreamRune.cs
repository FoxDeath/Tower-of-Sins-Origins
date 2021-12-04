using System.Collections;
using UnityEngine;

//To-Do: The two passive abilities are not done yet, but we also do not need them for quite some time.
public class DreamRune : Rune
{
    #region Attributes
    private Transform closestEnemy = null;

    [SerializeField] GameObject dreamRuneLightning;

    private PlayerNearbyEnemies playerNearbyEnemies;

    [SerializeField] LayerMask groundAndWallMask;
    #endregion

    #region MonoBehaviour Methods
    private new void Awake() 
    {
        base.Awake();

        playerNearbyEnemies = FindObjectOfType<PlayerNearbyEnemies>();
    }
    #endregion

    #region Normal Methods
    public override void WeaponPassive()
    {
        if(GetIsWeapon())
        {
            print("IsWeapon");   
        }
    }

    public override void ArmourPassive()
    {
        if(GetIsArmour())
        {
            print("IsArmour");
        }
    }

    /*Gets called by PlayerRuneController when the input for the special ability is pressed and starts 
    SpecialAbilityBehaviour if using the special ability is possible.*/
    public override void SpecialAbility()
    {
        if(GetIsSpecial() && !GetUsingSpecialAblity()
        && GetCanUseSpecialAbility() && closestEnemy != null)
        {
            StartCoroutine(SpecialAbilityBehaviour());
        }
        else
        {
            PlayerState.SetState(PlayerState.State.Idle);
        }
    }

    /*Calculates distance to the closest enemy and assigns the closest enemy
    and it returns true if the ability can be used*/
    public override bool GetCanUseSpecialAbility()
    {
        RaycastHit2D raycastHit2D = new RaycastHit2D();

        if(playerNearbyEnemies.GetEnemies().Count <= 0)
        {
            return false;
        }

        foreach(EnemyHealth enemy in playerNearbyEnemies.GetEnemies())
        {
            if(enemy != null)
            {
                if(closestEnemy == null)
                {
                    closestEnemy = enemy.transform;
                }
                else if(Vector2.Distance(closestEnemy.position, transform.position) > Vector2.Distance(enemy.transform.position, transform.position))
                {
                    closestEnemy = enemy.transform;
                }
            }
            else
            {
                closestEnemy = null;
            }
        }

        if(closestEnemy != null)
        {
            raycastHit2D = Physics2D.Raycast(transform.parent.position, (closestEnemy.position - transform.parent.position).normalized, Vector3.Distance(closestEnemy.position, transform.parent.position), groundAndWallMask);
        }

        return !raycastHit2D.collider && base.GetCanUseSpecialAbility();
    }
    #endregion

    #region Coroutines
    /*Damages player depending on the costPrecentage, calculates the possition the lightning should be spawned
    and then spawns it and sets state back to idle*/
    protected override IEnumerator SpecialAbilityBehaviour()
    {
        SetUsingSpecialAblity(true);

        playerHealth.Hurt((costPercentage * playerStats.GetMaxEnergyPoints()) / 100f, false, false);

        Vector3 ligtningPossition = Physics2D.Raycast(closestEnemy.position, Vector2.down, 8f, groundAndWallMask).point;


        if(ligtningPossition != Vector3.zero)
        {
            Instantiate(dreamRuneLightning, ligtningPossition, Quaternion.identity);
        }
        else
        {
            Instantiate(dreamRuneLightning, closestEnemy.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.1f);

        SetUsingSpecialAblity(false);

        PlayerState.SetState(PlayerState.State.Idle);
    }
    #endregion
}
