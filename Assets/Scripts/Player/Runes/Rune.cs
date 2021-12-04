using System.Collections;
using UnityEngine;

//Base rune class, all other runes inherit it.
public abstract class Rune : MonoBehaviour
{
    #region Attributes
    protected PlayerHealth playerHealth;

    protected PlayerStats playerStats;

    private bool isWeapon;
    private bool isArmour;
    private bool isSpecial;
    private bool usingSpecialAbility;

    [SerializeField, Range(1f, 99f)] protected float costPercentage;
    #endregion

    #region  MonoBehaviour Methods
    protected void Awake() 
    {
        playerHealth = GetComponentInParent<PlayerHealth>();

        playerStats = GetComponentInParent<PlayerStats>();
    }
    #endregion

    #region Normal Methods
    //Base weapon passive ability method that is implemented in each individual rune script depending on effects
    public abstract void WeaponPassive();

    //Base armor passive ability method that is implemented in each individual rune script depending on effects
    public abstract void ArmourPassive();

    //Base special ability method that is implemented in each individual rune script depending on effects
    public abstract void SpecialAbility();
    #endregion

    #region Setters
    //Sets rune type to weapon and unsets all other types 
    public void SetIsWeapon(bool isWeapon)
    {
        if(this.isWeapon != isWeapon)
        {
            SetIsArmour(false);

            SetIsSpecial(false);

            this.isWeapon = isWeapon;
        }
    }

    //Sets rune type to armour and unsets all other types 
    public void SetIsArmour(bool isArmour)
    {
        if(this.isArmour != isArmour)
        {
            SetIsWeapon(false);

            SetIsSpecial(false);

            this.isArmour = isArmour;
        }
    }

    //Sets rune type to special and unsets all other types 
    public void SetIsSpecial(bool isSpecial)
    {
        if(this.isSpecial != isSpecial)
        {
            SetIsWeapon(false);

            SetIsArmour(false);
            
            this.isSpecial = isSpecial;
        }
    }

    //Turns bool to true when special ability is used and then after a short time it is set it back to false
    public void SetUsingSpecialAblity(bool usingSpecialAbility)
    {
        if(this.usingSpecialAbility != usingSpecialAbility)
        {
            this.usingSpecialAbility = usingSpecialAbility;
        }
    }
    #endregion

    #region Getters
    protected bool GetIsWeapon()
    {
        return isWeapon;
    }

    protected bool GetIsArmour()
    {
        return isArmour;
    }

    protected bool GetIsSpecial()
    {
        return isSpecial;
    }

    protected bool GetUsingSpecialAblity()
    {
        return usingSpecialAbility;
    }

    public virtual bool GetCanUseSpecialAbility()
    {
        return ((costPercentage * playerStats.GetMaxEnergyPoints()) / 100f < playerHealth.GetEnergyPoints() && isSpecial);
    }
    #endregion

    #region Coroutines
    protected abstract IEnumerator SpecialAbilityBehaviour();
    #endregion
}
