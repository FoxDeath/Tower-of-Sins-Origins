using UnityEngine;

public class PlayerRuneController : MonoBehaviour
{
    //To-Do: Remove serialized fields and make setters after an inventory system exists
    #region Attributes
    [SerializeField] Rune weaponRune;
    [SerializeField] Rune armourRune;
    [SerializeField] Rune specialRune;
    #endregion

    #region  MonoBehaviour Methods
    private void Start()
    {
        SetRunes();
    }
    #endregion

    #region Normal Methods

    //To-Do: Use when inventory system exists
    private void EquipRune(Rune rune, Rune runeSlot)
    {
        UnequipRune(rune);

        if(runeSlot == weaponRune)
        {
            weaponRune = rune;
        }
        else if(runeSlot == armourRune)
        {
            armourRune = rune;
        }
        else if(runeSlot == specialRune)
        {
            specialRune = rune;
        }

        SetRunes();
    }

    private void SetRunes()
    {
        if(weaponRune)
        {
            weaponRune.SetIsWeapon(true);
        }

        if(armourRune)
        {
            armourRune.SetIsArmour(true);
        }

        if(specialRune)
        {
            specialRune.SetIsSpecial(true);
        }
    }

    private void UnequipRune(Rune rune)
    {
        if(rune == weaponRune)
        {
            weaponRune = null;
        }
        else if(rune == armourRune)
        {
            armourRune = null;
        }
        else if(rune == specialRune)
        {
            specialRune = null;
        }
    }

    public void SpecialAbility()
    {
        specialRune.SpecialAbility();
    }

    public bool GetCanUseSpecialAbility()
    {
        return specialRune.GetCanUseSpecialAbility();
    }
    #endregion
}
