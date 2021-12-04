using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    #region Attributes
    [Header("Combat")]
    private PlayerCombat playerCombat;

    [SerializeField] float energyRegenerated = 5f;
    [SerializeField] float parryDuration = 0.25f;
    [SerializeField] float attackDamageMultiplier = 1f;

    [Header("Health")]
    private PlayerHealth playerHealth;

    [SerializeField] float knockBackStrenght = 40f;
    [SerializeField] float recoveryTime = 2f;
    [SerializeField] float maxEnergyPoints = 100f;

    [Header("Movement")]
    private PlayerMovement playerMovement;

    [SerializeField] float movementSpeed = 30f;
    [SerializeField] float jumpForce = 65f;
    [SerializeField] float jumpTime = 0.45f;
    #endregion

    #region  MonoBehaviour Methods
    private void Awake() 
    {
        playerCombat = GetComponent<PlayerCombat>();

        playerHealth = GetComponent<PlayerHealth>();

        playerMovement = GetComponent<PlayerMovement>();    
    }

    private void OnValidate() 
    {
        if(playerCombat == null || playerHealth == null || playerMovement == null)
        {
            return;
        }

        playerCombat.SetPlayerStats();

        playerHealth.SetPlayerStats();

        playerMovement.SetPlayerStats();
    }
    #endregion

    #region  Setters
    public void SetParryDuration(float parryDuration)
    {
        this.parryDuration = parryDuration;
        
        playerCombat.SetPlayerStats();
    }

    public void SetEnergyRegenerated(float energyRegenerated)
    {
        this.energyRegenerated = energyRegenerated;

        playerCombat.SetPlayerStats();
    }

    public void SetAttackDamageMultiplier(float attackDamageMultiplier)
    {
        this.attackDamageMultiplier = attackDamageMultiplier;

        playerCombat.SetPlayerStats();
    }

    public void SetKnockBackStrenght(float knockBackStrenght)
    {
        this.knockBackStrenght = knockBackStrenght;

        playerHealth.SetPlayerStats();
    }

    public void SetRecoveryTime(float recoveryTime)
    {
        this.recoveryTime = recoveryTime;
        
        playerHealth.SetPlayerStats();
    }

    public void SetMaxEnergyPoints(float maxEnergyPoints)
    {
        this.maxEnergyPoints = maxEnergyPoints;

        playerHealth.SetPlayerStats();
    }

    public void SetMovementSpeed(float movementSpeed)
    {
        this.movementSpeed = movementSpeed;

        playerMovement.SetPlayerStats();
    }
    
    public void SetJumpForce(float jumpForce)
    {
        this.jumpForce = jumpForce;

        playerMovement.SetPlayerStats();
    }
    
    public void SetJumpTime(float jumpTime)
    {
        this.jumpTime = jumpTime;

        playerMovement.SetPlayerStats();
    }
    #endregion

    #region Getters
    public float GetParryDuration()
    {
        return parryDuration;
    }

    public float GetAttackDamageMultiplier()
    {
        return attackDamageMultiplier;
    }

    public float GetKnockBackStrenght()
    {
        return knockBackStrenght;
    }

    public float GetRecoveryTime()
    {
        return recoveryTime;
    }

    public float GetMaxEnergyPoints()
    {
        return maxEnergyPoints;
    }
    
    public float GetMovementSpeed()
    {
        return movementSpeed;
    }
    
    public float GetJumpForce()
    {
        return jumpForce;
    }
    
    public float GetJumpTime()
    {
        return jumpTime;
    }
    #endregion
}
